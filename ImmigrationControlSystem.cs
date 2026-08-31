using Game;
using Game.Simulation;
using Unity.Entities;

namespace CimRejuvenator
{
    public partial class ImmigrationControlSystem : GameSystemBase
    {
        public static string Status { get; private set; } = "Vanilla";

        private HouseholdSpawnSystem m_HouseholdSpawnSystem;
        private SimulationSystem m_SimulationSystem;
        private EntityQuery m_TimeDataQuery;
        private bool m_WasControlling;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_HouseholdSpawnSystem = World.GetOrCreateSystemManaged<HouseholdSpawnSystem>();
            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();
            m_TimeDataQuery = GetEntityQuery(ComponentType.ReadOnly<TimeData>());
            RequireForUpdate(m_TimeDataQuery);
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return 16;
        }

        protected override void OnUpdate()
        {
            var setting = Mod.Setting;
            if (setting == null || !setting.EnableMod || !setting.EnableImmigrationControl)
            {
                ReleaseControl();
                return;
            }

            var timeData = m_TimeDataQuery.GetSingleton<TimeData>();
            var day = TimeSystem.GetDay(m_SimulationSystem.frameIndex, timeData);
            PopulationFlowSystem.EnsureDay(day);

            var dailyCapReached =
                setting.UseImmigrationDailyCap &&
                PopulationFlowSystem.NewResidentsToday >= setting.MaxNewResidentsPerDay;

            var populationCeilingReached =
                setting.UsePopulationCeiling &&
                PopulationManagementSystem.ResidentCount >= setting.PopulationCeiling;

            var intensity = PopulationManagementSystem.Clamp(setting.ImmigrationIntensity, 0, 100);
            var intensityAllowsSpawn = PassesIntensity(m_SimulationSystem.frameIndex, intensity);
            var enabled = !dailyCapReached && !populationCeilingReached && intensityAllowsSpawn;

            m_HouseholdSpawnSystem.Enabled = enabled;
            m_WasControlling = true;

            if (dailyCapReached)
            {
                Status = "Paused: daily resident cap";
            }
            else if (populationCeilingReached)
            {
                Status = "Paused: population ceiling";
            }
            else if (intensity <= 0)
            {
                Status = "Paused: 0% intensity";
            }
            else if (intensity >= 100)
            {
                Status = "Open: 100% intensity";
            }
            else
            {
                Status = $"Throttled: {intensity}% intensity";
            }
        }

        protected override void OnDestroy()
        {
            ReleaseControl();
            base.OnDestroy();
        }

        private void ReleaseControl()
        {
            if (m_WasControlling && m_HouseholdSpawnSystem != null)
            {
                m_HouseholdSpawnSystem.Enabled = true;
            }

            m_WasControlling = false;
            Status = "Vanilla";
        }

        private static bool PassesIntensity(uint frame, int intensity)
        {
            if (intensity <= 0)
            {
                return false;
            }

            if (intensity >= 100)
            {
                return true;
            }

            unchecked
            {
                var x = frame / 16u;
                x ^= x >> 16;
                x *= 0x7FEB352Du;
                x ^= x >> 15;
                x *= 0x846CA68Bu;
                x ^= x >> 16;
                return (x % 100u) < (uint)intensity;
            }
        }
    }
}
