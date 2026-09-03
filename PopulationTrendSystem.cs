using System;
using System.Threading;
using Game;
using Game.Agents;
using Game.Citizens;
using Game.Common;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace CimRejuvenator
{
    /// <summary>
    /// Feedback controller for the resident population trend.
    /// Positive and neutral targets are pursued by adapting immigration and birth rates.
    /// Negative targets can optionally mark resident households to move away.
    /// </summary>
    public partial class PopulationTrendSystem : GameSystemBase
    {
        public const int FramesPerDay = 262144;
        public const int ChecksPerDay = 64;

        public static int ActualChangeLastDay { get; private set; }
        public static double SmoothedChangePerDay { get; private set; }
        public static int EffectiveImmigrationIntensity { get; private set; } = 100;
        public static int EffectiveBirthRatePercent { get; private set; } = 100;
        public static int ForcedOutflowToday { get; private set; }
        public static int ForcedOutflowSession { get; private set; }
        public static string Status { get; private set; } = "Disabled";

        private static int s_ResetRequested;

        private SimulationSystem m_SimulationSystem;
        private EntityQuery m_TimeDataQuery;
        private EntityQuery m_HouseholdQuery;

        private int m_LastDay = int.MinValue;
        private int m_LastPopulation;
        private bool m_HasTrendSample;
        private bool m_WasEnabled;

        public static void RequestReset()
        {
            Interlocked.Exchange(ref s_ResetRequested, 1);
        }

        public static void ResetStatistics()
        {
            ActualChangeLastDay = 0;
            SmoothedChangePerDay = 0;
            ForcedOutflowToday = 0;
            ForcedOutflowSession = 0;
        }

        public static bool ShouldControlImmigration(CimRejuvenatorSetting setting)
        {
            if (setting == null || !setting.EnableMod)
            {
                return false;
            }

            return setting.EnableImmigrationControl ||
                (setting.EnablePopulationTrendControl && setting.TrendUseImmigration);
        }

        public static bool ShouldControlBirths(CimRejuvenatorSetting setting)
        {
            if (setting == null || !setting.EnableMod)
            {
                return false;
            }

            return setting.EnableBirthControl ||
                (setting.EnablePopulationTrendControl && setting.TrendUseBirths);
        }

        public static int GetEffectiveImmigrationIntensity(CimRejuvenatorSetting setting)
        {
            if (setting != null &&
                setting.EnableMod &&
                setting.EnablePopulationTrendControl &&
                setting.TrendUseImmigration)
            {
                return PopulationManagementSystem.Clamp(EffectiveImmigrationIntensity, 0, 100);
            }

            if (setting != null && setting.EnableImmigrationControl)
            {
                return PopulationManagementSystem.Clamp(setting.ImmigrationIntensity, 0, 100);
            }

            return 100;
        }

        public static int GetEffectiveBirthRatePercent(CimRejuvenatorSetting setting)
        {
            if (setting != null &&
                setting.EnableMod &&
                setting.EnablePopulationTrendControl &&
                setting.TrendUseBirths)
            {
                return PopulationManagementSystem.Clamp(
                    EffectiveBirthRatePercent,
                    0,
                    PopulationManagementSystem.Clamp(setting.TrendMaximumBirthRatePercent, 100, 500));
            }

            if (setting != null && setting.EnableBirthControl)
            {
                return PopulationManagementSystem.Clamp(setting.BirthRatePercent, 0, 500);
            }

            return 100;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();
            m_TimeDataQuery = GetEntityQuery(ComponentType.ReadOnly<TimeData>());
            m_HouseholdQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<Household>(),
                    ComponentType.ReadOnly<HouseholdCitizen>(),
                },
                None = new[]
                {
                    ComponentType.ReadOnly<TouristHousehold>(),
                    ComponentType.ReadOnly<CommuterHousehold>(),
                    ComponentType.ReadOnly<MovingAway>(),
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                },
            });

            RequireForUpdate(m_TimeDataQuery);
            Mod.Log.Info("PopulationTrendSystem initialized.");
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return FramesPerDay / ChecksPerDay;
        }

        protected override void OnUpdate()
        {
            var setting = Mod.Setting;
            var enabled = setting != null && setting.EnableMod && setting.EnablePopulationTrendControl;

            if (!enabled)
            {
                if (m_WasEnabled)
                {
                    ResetRuntime(setting);
                }

                m_WasEnabled = false;
                Status = "Disabled";
                return;
            }

            m_WasEnabled = true;

            if (Interlocked.Exchange(ref s_ResetRequested, 0) != 0)
            {
                ResetRuntime(setting);
            }

            var population = PopulationManagementSystem.ResidentCount;
            if (population <= 0)
            {
                Status = "Waiting for population census";
                return;
            }

            var timeData = m_TimeDataQuery.GetSingleton<TimeData>();
            var day = TimeSystem.GetDay(m_SimulationSystem.frameIndex, timeData);

            if (m_LastDay == int.MinValue || day < m_LastDay)
            {
                InitializeBaseline(setting, day, population);
                return;
            }

            if (day == m_LastDay)
            {
                return;
            }

            var elapsedDays = Math.Max(1, day - m_LastDay);
            var delta = population - m_LastPopulation;
            ActualChangeLastDay = (int)Math.Round(delta / (double)elapsedDays);

            if (!m_HasTrendSample)
            {
                SmoothedChangePerDay = ActualChangeLastDay;
                m_HasTrendSample = true;
            }
            else
            {
                const double smoothing = 0.35;
                SmoothedChangePerDay =
                    SmoothedChangePerDay * (1.0 - smoothing) +
                    ActualChangeLastDay * smoothing;
            }

            ForcedOutflowToday = 0;
            AdjustController(setting, population, day);

            m_LastDay = day;
            m_LastPopulation = population;
        }

        private void InitializeBaseline(CimRejuvenatorSetting setting, int day, int population)
        {
            m_LastDay = day;
            m_LastPopulation = population;
            m_HasTrendSample = false;
            ActualChangeLastDay = 0;
            SmoothedChangePerDay = 0;
            ForcedOutflowToday = 0;

            EffectiveImmigrationIntensity = setting.EnableImmigrationControl
                ? PopulationManagementSystem.Clamp(setting.ImmigrationIntensity, 0, 100)
                : 100;

            EffectiveBirthRatePercent = setting.EnableBirthControl
                ? PopulationManagementSystem.Clamp(setting.BirthRatePercent, 0, 500)
                : 100;

            Status = "Learning baseline";
        }

        private void ResetRuntime(CimRejuvenatorSetting setting)
        {
            m_LastDay = int.MinValue;
            m_LastPopulation = 0;
            m_HasTrendSample = false;
            ActualChangeLastDay = 0;
            SmoothedChangePerDay = 0;
            ForcedOutflowToday = 0;

            EffectiveImmigrationIntensity = setting != null && setting.EnableImmigrationControl
                ? PopulationManagementSystem.Clamp(setting.ImmigrationIntensity, 0, 100)
                : 100;

            EffectiveBirthRatePercent = setting != null && setting.EnableBirthControl
                ? PopulationManagementSystem.Clamp(setting.BirthRatePercent, 0, 500)
                : 100;
        }

        private void AdjustController(CimRejuvenatorSetting setting, int population, int day)
        {
            var target = PopulationManagementSystem.Clamp(
                setting.TargetNetPopulationChangePerDay,
                -100000,
                100000);
            var deadband = PopulationManagementSystem.Clamp(setting.TrendDeadband, 0, 10000);
            var response = PopulationManagementSystem.Clamp(setting.TrendResponseStrength, 10, 100) / 100.0;
            var error = target - SmoothedChangePerDay;

            if (target < 0)
            {
                if (setting.TrendUseImmigration)
                {
                    EffectiveImmigrationIntensity = 0;
                }

                if (setting.TrendUseBirths)
                {
                    EffectiveBirthRatePercent = 0;
                }

                if (setting.TrendAllowForcedOutflow && SmoothedChangePerDay > target + deadband)
                {
                    var gap = SmoothedChangePerDay - target - deadband;
                    var requested = (int)Math.Ceiling(gap * response);
                    var cap = PopulationManagementSystem.Clamp(setting.TrendMaxForcedOutflowPerDay, 100, 100000);
                    requested = Math.Min(requested, cap);

                    if (requested > 0)
                    {
                        ForcedOutflowToday = ForceHouseholdsOut(requested, day);
                        ForcedOutflowSession += ForcedOutflowToday;
                    }
                }

                Status = setting.TrendAllowForcedOutflow
                    ? $"Negative target: forced outflow {ForcedOutflowToday:N0} residents"
                    : "Negative target: inflow suppressed; forced outflow disabled";
                return;
            }

            if (Math.Abs(error) <= deadband)
            {
                Status = "Holding near target";
                return;
            }

            var scale = Math.Max(1000.0, population * 0.01);
            var normalizedError = Math.Max(-1.0, Math.Min(1.0, error / scale));

            if (setting.TrendUseImmigration)
            {
                var step = SignedStep(normalizedError * 50.0 * response);
                EffectiveImmigrationIntensity = PopulationManagementSystem.Clamp(
                    EffectiveImmigrationIntensity + step,
                    0,
                    100);
            }

            if (setting.TrendUseBirths)
            {
                var maxBirth = PopulationManagementSystem.Clamp(setting.TrendMaximumBirthRatePercent, 100, 500);
                var step = SignedStep(normalizedError * 100.0 * response);
                EffectiveBirthRatePercent = PopulationManagementSystem.Clamp(
                    EffectiveBirthRatePercent + step,
                    0,
                    maxBirth);
            }

            Status = error > 0
                ? "Increasing population inflow"
                : "Reducing population inflow";
        }

        private int ForceHouseholdsOut(int residentBudget, int day)
        {
            if (residentBudget <= 0)
            {
                return 0;
            }

            var households = m_HouseholdQuery.ToEntityArray(Allocator.Temp);
            if (households.Length == 0)
            {
                households.Dispose();
                return 0;
            }

            var forcedResidents = 0;
            var start = (int)(((uint)day * 2654435761u) % (uint)households.Length);

            for (var n = 0; n < households.Length && forcedResidents < residentBudget; n++)
            {
                var index = (start + n) % households.Length;
                var householdEntity = households[index];
                var household = EntityManager.GetComponentData<Household>(householdEntity);

                if ((household.m_Flags & HouseholdFlags.MovedIn) == 0)
                {
                    continue;
                }

                var members = EntityManager.GetBuffer<HouseholdCitizen>(householdEntity, true);
                if (members.Length <= 0)
                {
                    continue;
                }

                EntityManager.AddComponentData(
                    householdEntity,
                    new MovingAway { m_Reason = MoveAwayReason.NoSuitableProperty });
                forcedResidents += members.Length;
            }

            households.Dispose();

            if (forcedResidents > 0)
            {
                Mod.Log.Info($"Population trend controller marked households containing {forcedResidents:N0} residents to move away.");
            }

            return forcedResidents;
        }

        private static int SignedStep(double value)
        {
            var rounded = (int)Math.Round(value);
            if (rounded != 0 || Math.Abs(value) < 0.0001)
            {
                return rounded;
            }

            return value > 0 ? 1 : -1;
        }
    }
}
