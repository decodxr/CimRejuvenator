using System.Collections.Generic;
using Game;
using Game.Prefabs;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;

namespace CimRejuvenator
{
    public partial class BirthRateControlSystem : GameSystemBase
    {
        public static int LastAppliedBirthRatePercent { get; private set; } = 100;

        private readonly Dictionary<Entity, CitizenParametersData> m_OriginalValues =
            new Dictionary<Entity, CitizenParametersData>();

        private SimulationSystem m_SimulationSystem;
        private EntityQuery m_ParametersQuery;
        private EntityQuery m_TimeDataQuery;
        private bool m_IsApplied;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();
            m_ParametersQuery = GetEntityQuery(ComponentType.ReadWrite<CitizenParametersData>());
            m_TimeDataQuery = GetEntityQuery(ComponentType.ReadOnly<TimeData>());

            RequireForUpdate(m_ParametersQuery);
            RequireForUpdate(m_TimeDataQuery);
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return 512;
        }

        protected override void OnUpdate()
        {
            var setting = Mod.Setting;
            if (!PopulationTrendSystem.ShouldControlBirths(setting))
            {
                RestoreOriginalValues();
                LastAppliedBirthRatePercent = 100;
                return;
            }

            var timeData = m_TimeDataQuery.GetSingleton<TimeData>();
            var day = TimeSystem.GetDay(m_SimulationSystem.frameIndex, timeData);
            PopulationFlowSystem.EnsureDay(day);

            var appliedPercent = PopulationTrendSystem.GetEffectiveBirthRatePercent(setting);

            if (setting.EnableBirthControl &&
                setting.UseBirthDailyCap &&
                PopulationFlowSystem.BirthsToday >= setting.MaxBirthsPerDay)
            {
                appliedPercent = 0;
            }

            if (setting.EnableBirthControl &&
                setting.BirthsRespectChildTarget &&
                ChildTargetReached(setting))
            {
                appliedPercent = 0;
            }

            var factor = appliedPercent / 100f;
            var entities = m_ParametersQuery.ToEntityArray(Allocator.Temp);

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                CitizenParametersData baseline;

                if (!m_OriginalValues.TryGetValue(entity, out baseline))
                {
                    baseline = EntityManager.GetComponentData<CitizenParametersData>(entity);
                    m_OriginalValues[entity] = baseline;
                }

                var data = baseline;
                data.m_BaseBirthRate = baseline.m_BaseBirthRate * factor;
                data.m_AdultFemaleBirthRateBonus = baseline.m_AdultFemaleBirthRateBonus * factor;
                EntityManager.SetComponentData(entity, data);
            }

            entities.Dispose();
            m_IsApplied = true;
            LastAppliedBirthRatePercent = appliedPercent;
        }

        protected override void OnDestroy()
        {
            RestoreOriginalValues();
            base.OnDestroy();
        }

        private bool ChildTargetReached(CimRejuvenatorSetting setting)
        {
            var total =
                System.Math.Max(0, setting.TargetChildPercent) +
                System.Math.Max(0, setting.TargetTeenPercent) +
                System.Math.Max(0, setting.TargetAdultPercent) +
                System.Math.Max(0, setting.TargetSeniorPercent);

            if (total <= 0 || PopulationManagementSystem.ResidentCount <= 0)
            {
                return false;
            }

            var childTarget = System.Math.Max(0, setting.TargetChildPercent) * 100.0 / total;
            return PopulationManagementSystem.ChildPercent >= childTarget;
        }

        private void RestoreOriginalValues()
        {
            if (!m_IsApplied || m_OriginalValues.Count == 0)
            {
                return;
            }

            foreach (var pair in m_OriginalValues)
            {
                if (EntityManager.Exists(pair.Key) && EntityManager.HasComponent<CitizenParametersData>(pair.Key))
                {
                    EntityManager.SetComponentData(pair.Key, pair.Value);
                }
            }

            m_IsApplied = false;
        }
    }
}
