using System.Collections.Generic;
using Game;
using Game.Citizens;
using Game.Common;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace CimRejuvenator
{
    public partial class PopulationFlowSystem : GameSystemBase
    {
        public const int FramesPerDay = 262144;
        public const int ChecksPerDay = 512;

        public static int BirthsToday { get; private set; }
        public static int BirthsSession { get; private set; }
        public static int NewResidentsToday { get; private set; }
        public static int NewResidentsSession { get; private set; }

        private static int s_CurrentDay = int.MinValue;

        private readonly HashSet<long> m_KnownCitizens = new HashSet<long>();
        private SimulationSystem m_SimulationSystem;
        private EntityQuery m_CitizenQuery;
        private EntityQuery m_TimeDataQuery;
        private bool m_BaselineReady;

        public static void EnsureDay(int day)
        {
            if (day == s_CurrentDay)
            {
                return;
            }

            s_CurrentDay = day;
            BirthsToday = 0;
            NewResidentsToday = 0;
        }

        public static void ResetStatistics()
        {
            BirthsToday = 0;
            BirthsSession = 0;
            NewResidentsToday = 0;
            NewResidentsSession = 0;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();
            m_CitizenQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<Citizen>() },
                None = new[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                },
            });
            m_TimeDataQuery = GetEntityQuery(ComponentType.ReadOnly<TimeData>());

            RequireForUpdate(m_CitizenQuery);
            RequireForUpdate(m_TimeDataQuery);
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return FramesPerDay / ChecksPerDay;
        }

        protected override void OnUpdate()
        {
            var timeData = m_TimeDataQuery.GetSingleton<TimeData>();
            var day = TimeSystem.GetDay(m_SimulationSystem.frameIndex, timeData);
            EnsureDay(day);

            var entities = m_CitizenQuery.ToEntityArray(Allocator.Temp);
            var citizens = m_CitizenQuery.ToComponentDataArray<Citizen>(Allocator.Temp);

            if (!m_BaselineReady)
            {
                for (var i = 0; i < entities.Length; i++)
                {
                    m_KnownCitizens.Add(EntityKey(entities[i]));
                }

                m_BaselineReady = true;
                citizens.Dispose();
                entities.Dispose();
                Mod.Log.Info($"Population flow baseline initialized with {m_KnownCitizens.Count:N0} citizens.");
                return;
            }

            var setting = Mod.Setting;

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                if (!m_KnownCitizens.Add(EntityKey(entity)))
                {
                    continue;
                }

                var citizen = citizens[i];
                if (!IsLivingResident(entity, citizen))
                {
                    continue;
                }

                var ageInDays = day - citizen.m_BirthDay;
                var isNewborn = citizen.GetAge() == CitizenAge.Child && ageInDays == 0;

                if (isNewborn)
                {
                    BirthsToday++;
                    BirthsSession++;
                    continue;
                }

                NewResidentsToday++;
                NewResidentsSession++;

                if (setting != null &&
                    setting.EnableMod &&
                    setting.EnableImmigrationControl &&
                    setting.ShapeNewResidentAges)
                {
                    ShapeIncomingAge(setting, entity, ref citizen, day);
                    EntityManager.SetComponentData(entity, citizen);
                }
            }

            citizens.Dispose();
            entities.Dispose();
        }

        private void ShapeIncomingAge(
            CimRejuvenatorSetting setting,
            Entity entity,
            ref Citizen citizen,
            int day)
        {
            var weights = new[]
            {
                System.Math.Max(0, setting.IncomingChildWeight),
                System.Math.Max(0, setting.IncomingTeenWeight),
                System.Math.Max(0, setting.IncomingAdultWeight),
                System.Math.Max(0, setting.IncomingSeniorWeight),
            };

            var total = weights[0] + weights[1] + weights[2] + weights[3];
            if (total <= 0)
            {
                return;
            }

            var roll = (int)(PopulationManagementSystem.StableHash(entity, 0xC1u) % (uint)total);
            var cumulative = 0;
            var selected = 2;

            for (var i = 0; i < 4; i++)
            {
                cumulative += weights[i];
                if (roll < cumulative)
                {
                    selected = i;
                    break;
                }
            }

            var targetAge = (CitizenAge)selected;
            if (targetAge != CitizenAge.Adult && EntityManager.HasComponent<Worker>(entity))
            {
                EntityManager.RemoveComponent<Worker>(entity);
            }

            citizen.SetAge(targetAge);
            citizen.m_BirthDay = PopulationManagementSystem.ToBirthDay(
                day,
                PopulationManagementSystem.AgeForStage(entity, targetAge));
        }

        private bool IsLivingResident(Entity entity, Citizen citizen)
        {
            if ((citizen.m_State & (CitizenFlags.Tourist | CitizenFlags.Commuter)) != 0)
            {
                return false;
            }

            if (!EntityManager.HasComponent<HealthProblem>(entity))
            {
                return true;
            }

            var problem = EntityManager.GetComponentData<HealthProblem>(entity);
            return (problem.m_Flags & HealthProblemFlags.Dead) == 0;
        }

        private static long EntityKey(Entity entity)
        {
            return ((long)entity.Version << 32) | (uint)entity.Index;
        }
    }
}
