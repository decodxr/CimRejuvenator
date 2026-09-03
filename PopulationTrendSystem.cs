using System;
using System.Threading;
using Game;
using Game.Agents;
using Game.Citizens;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace CimRejuvenator
{
    /// <summary>
    /// Controls the established resident population trend.
    /// Adaptive mode steers immigration and birth rates. Direct mode additionally injects
    /// normal vanilla household entities when the previous complete simulation day missed
    /// the selected target, so losses can be compensated without waiting for demand to react.
    /// </summary>
    public partial class PopulationTrendSystem : GameSystemBase
    {
        public const int FramesPerDay = 262144;
        public const int ChecksPerDay = 64;
        private const int MaxDirectHouseholdsPerCorrection = 50000;

        public static int ActualChangeLastDay { get; private set; }
        public static double SmoothedChangePerDay { get; private set; }
        public static int EffectiveImmigrationIntensity { get; private set; } = 100;
        public static int EffectiveBirthRatePercent { get; private set; } = 100;
        public static int ForcedOutflowToday { get; private set; }
        public static int ForcedOutflowSession { get; private set; }
        public static int DirectCorrectionRequestedLastDay { get; private set; }
        public static int DirectResidentsInjectedToday { get; private set; }
        public static int DirectResidentsInjectedSession { get; private set; }
        public static int DirectHouseholdsInjectedToday { get; private set; }
        public static int DirectHouseholdsInjectedSession { get; private set; }
        public static string Status { get; private set; } = "Disabled";

        private static int s_ResetRequested;

        private SimulationSystem m_SimulationSystem;
        private EndFrameBarrier m_EndFrameBarrier;
        private EntityQuery m_TimeDataQuery;
        private EntityQuery m_HouseholdQuery;
        private EntityQuery m_HouseholdPrefabQuery;
        private EntityQuery m_OutsideConnectionQuery;

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
            DirectCorrectionRequestedLastDay = 0;
            DirectResidentsInjectedToday = 0;
            DirectResidentsInjectedSession = 0;
            DirectHouseholdsInjectedToday = 0;
            DirectHouseholdsInjectedSession = 0;
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

        public static bool CanShapeIncomingAges(CimRejuvenatorSetting setting)
        {
            if (setting == null || !setting.EnableMod || !setting.ShapeNewResidentAges)
            {
                return false;
            }

            return setting.EnableImmigrationControl ||
                (setting.EnablePopulationTrendControl && setting.DirectTrendMode);
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
            m_EndFrameBarrier = World.GetOrCreateSystemManaged<EndFrameBarrier>();
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

            // Matches the vanilla HouseholdSpawnSystem prefab source: normal household prefabs only.
            m_HouseholdPrefabQuery = GetEntityQuery(
                ComponentType.ReadOnly<ArchetypeData>(),
                ComponentType.ReadOnly<HouseholdData>(),
                ComponentType.Exclude<DynamicHousehold>());

            // Matches the vanilla resident household spawn connection filter.
            m_OutsideConnectionQuery = GetEntityQuery(
                ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
                ComponentType.Exclude<Game.Objects.ElectricityOutsideConnection>(),
                ComponentType.Exclude<Game.Objects.WaterPipeOutsideConnection>(),
                ComponentType.Exclude<Temp>(),
                ComponentType.Exclude<Deleted>());

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
            DirectResidentsInjectedToday = 0;
            DirectHouseholdsInjectedToday = 0;
            DirectCorrectionRequestedLastDay = 0;

            if (setting.DirectTrendMode)
            {
                AdjustDirectController(setting, population, day);
            }
            else
            {
                AdjustAdaptiveController(setting, population, day);
            }

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
            DirectCorrectionRequestedLastDay = 0;
            DirectResidentsInjectedToday = 0;
            DirectHouseholdsInjectedToday = 0;

            EffectiveImmigrationIntensity = setting.EnableImmigrationControl
                ? PopulationManagementSystem.Clamp(setting.ImmigrationIntensity, 0, 100)
                : 100;

            EffectiveBirthRatePercent = setting.EnableBirthControl
                ? PopulationManagementSystem.Clamp(setting.BirthRatePercent, 0, 500)
                : 100;

            Status = setting.DirectTrendMode
                ? "Direct mode: learning baseline"
                : "Learning baseline";
        }

        private void ResetRuntime(CimRejuvenatorSetting setting)
        {
            m_LastDay = int.MinValue;
            m_LastPopulation = 0;
            m_HasTrendSample = false;
            ActualChangeLastDay = 0;
            SmoothedChangePerDay = 0;
            ForcedOutflowToday = 0;
            DirectCorrectionRequestedLastDay = 0;
            DirectResidentsInjectedToday = 0;
            DirectHouseholdsInjectedToday = 0;

            EffectiveImmigrationIntensity = setting != null && setting.EnableImmigrationControl
                ? PopulationManagementSystem.Clamp(setting.ImmigrationIntensity, 0, 100)
                : 100;

            EffectiveBirthRatePercent = setting != null && setting.EnableBirthControl
                ? PopulationManagementSystem.Clamp(setting.BirthRatePercent, 0, 500)
                : 100;
        }

        private void AdjustDirectController(CimRejuvenatorSetting setting, int population, int day)
        {
            var target = PopulationManagementSystem.Clamp(
                setting.TargetNetPopulationChangePerDay,
                -100000,
                100000);
            var deadband = PopulationManagementSystem.Clamp(setting.TrendDeadband, 0, 10000);
            var correctionStrength = PopulationManagementSystem.Clamp(
                setting.DirectTrendCorrectionStrength,
                10,
                100) / 100.0;
            var error = target - ActualChangeLastDay;

            // Rate controls become an assist layer in direct mode. The direct household injection below
            // is the part that compensates a real population shortfall immediately after a completed day.
            if (setting.TrendUseImmigration)
            {
                if (error > deadband)
                {
                    EffectiveImmigrationIntensity = 100;
                }
                else if (error < -deadband)
                {
                    EffectiveImmigrationIntensity = 0;
                }
            }

            if (setting.TrendUseBirths)
            {
                var maxBirth = PopulationManagementSystem.Clamp(setting.TrendMaximumBirthRatePercent, 100, 500);
                if (error > deadband)
                {
                    EffectiveBirthRatePercent = maxBirth;
                }
                else if (error < -deadband)
                {
                    EffectiveBirthRatePercent = 0;
                }
            }

            if (Math.Abs(error) <= deadband)
            {
                Status = "Direct mode: target met";
                return;
            }

            if (error > deadband)
            {
                var requested = (int)Math.Ceiling((error - deadband) * correctionStrength);
                var cap = PopulationManagementSystem.Clamp(
                    setting.DirectTrendMaxInjectedResidentsPerDay,
                    100,
                    250000);
                requested = Math.Min(requested, cap);

                if (setting.EnableImmigrationControl && setting.UsePopulationCeiling)
                {
                    var remainingCapacity = Math.Max(0, setting.PopulationCeiling - population);
                    requested = Math.Min(requested, remainingCapacity);
                }

                DirectCorrectionRequestedLastDay = requested;

                if (requested <= 0)
                {
                    Status = "Direct mode: correction blocked by population ceiling";
                    return;
                }

                var result = InjectVanillaHouseholds(requested, day);
                DirectResidentsInjectedToday = result.residents;
                DirectResidentsInjectedSession += result.residents;
                DirectHouseholdsInjectedToday = result.households;
                DirectHouseholdsInjectedSession += result.households;

                if (result.residents > 0)
                {
                    Status = $"Direct mode: injected ~{result.residents:N0} residents in {result.households:N0} households";
                    Mod.Log.Info(
                        $"Direct trend correction: actual={ActualChangeLastDay:+0;-0;0}/day, " +
                        $"target={target:+0;-0;0}/day, requested={requested:N0}, " +
                        $"scheduledResidents={result.residents:N0}, households={result.households:N0}.");
                }
                else
                {
                    Status = "Direct mode: no valid household prefab or outside connection";
                }

                return;
            }

            // The city grew faster than requested. Direct removal remains opt-in because it changes
            // household state. Without it, the controller only closes the inflow/birth-rate assist.
            if (setting.TrendAllowForcedOutflow)
            {
                var requested = (int)Math.Ceiling((-error - deadband) * correctionStrength);
                var cap = PopulationManagementSystem.Clamp(setting.TrendMaxForcedOutflowPerDay, 100, 100000);
                requested = Math.Min(requested, cap);
                ForcedOutflowToday = ForceHouseholdsOut(requested, day);
                ForcedOutflowSession += ForcedOutflowToday;
                Status = $"Direct mode: forced outflow ~{ForcedOutflowToday:N0} residents";
            }
            else
            {
                Status = "Direct mode: above target; inflow throttled";
            }
        }

        private void AdjustAdaptiveController(CimRejuvenatorSetting setting, int population, int day)
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

        private (int residents, int households) InjectVanillaHouseholds(int residentBudget, int day)
        {
            if (residentBudget <= 0)
            {
                return (0, 0);
            }

            var prefabs = m_HouseholdPrefabQuery.ToEntityArray(Allocator.Temp);
            var outsideConnections = m_OutsideConnectionQuery.ToEntityArray(Allocator.Temp);

            if (prefabs.Length == 0 || outsideConnections.Length == 0)
            {
                prefabs.Dispose();
                outsideConnections.Dispose();
                return (0, 0);
            }

            var commandBuffer = m_EndFrameBarrier.CreateCommandBuffer();
            var scheduledResidents = 0;
            var scheduledHouseholds = 0;

            while (scheduledResidents < residentBudget && scheduledHouseholds < MaxDirectHouseholdsPerCorrection)
            {
                var remaining = residentBudget - scheduledResidents;
                var prefab = SelectHouseholdPrefab(prefabs, remaining, day, scheduledHouseholds, out var householdSize);
                if (prefab == Entity.Null || householdSize <= 0)
                {
                    break;
                }

                var archetypeData = EntityManager.GetComponentData<ArchetypeData>(prefab);
                var connectionIndex = (int)(DirectHash(day, scheduledHouseholds, 0x91u) % (uint)outsideConnections.Length);
                var outsideConnection = outsideConnections[connectionIndex];

                var householdEntity = commandBuffer.CreateEntity(archetypeData.m_Archetype);
                commandBuffer.SetComponent(householdEntity, new PrefabRef { m_Prefab = prefab });
                commandBuffer.AddComponent(
                    householdEntity,
                    new CurrentBuilding { m_CurrentBuilding = outsideConnection });

                scheduledResidents += householdSize;
                scheduledHouseholds++;
            }

            prefabs.Dispose();
            outsideConnections.Dispose();
            return (scheduledResidents, scheduledHouseholds);
        }

        private Entity SelectHouseholdPrefab(
            NativeArray<Entity> prefabs,
            int remaining,
            int day,
            int sequence,
            out int selectedSize)
        {
            selectedSize = 0;
            var hasFit = false;
            var smallestSize = int.MaxValue;

            for (var i = 0; i < prefabs.Length; i++)
            {
                var data = EntityManager.GetComponentData<HouseholdData>(prefabs[i]);
                var size = HouseholdResidentCount(data);
                if (size <= 0)
                {
                    continue;
                }

                if (size <= remaining)
                {
                    hasFit = true;
                }

                if (size < smallestSize)
                {
                    smallestSize = size;
                }
            }

            if (smallestSize == int.MaxValue)
            {
                return Entity.Null;
            }

            var totalWeight = 0;
            for (var i = 0; i < prefabs.Length; i++)
            {
                var data = EntityManager.GetComponentData<HouseholdData>(prefabs[i]);
                var size = HouseholdResidentCount(data);
                if (size <= 0)
                {
                    continue;
                }

                var eligible = hasFit ? size <= remaining : size == smallestSize;
                if (eligible)
                {
                    totalWeight += Math.Max(1, data.m_Weight);
                }
            }

            if (totalWeight <= 0)
            {
                return Entity.Null;
            }

            var roll = (int)(DirectHash(day, sequence, 0xC7u) % (uint)totalWeight);
            for (var i = 0; i < prefabs.Length; i++)
            {
                var data = EntityManager.GetComponentData<HouseholdData>(prefabs[i]);
                var size = HouseholdResidentCount(data);
                if (size <= 0)
                {
                    continue;
                }

                var eligible = hasFit ? size <= remaining : size == smallestSize;
                if (!eligible)
                {
                    continue;
                }

                roll -= Math.Max(1, data.m_Weight);
                if (roll < 0)
                {
                    selectedSize = size;
                    return prefabs[i];
                }
            }

            return Entity.Null;
        }

        private static int HouseholdResidentCount(HouseholdData data)
        {
            return Math.Max(0, data.m_ChildCount) +
                Math.Max(0, data.m_AdultCount) +
                Math.Max(0, data.m_ElderCount) +
                Math.Max(0, data.m_StudentCount);
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

        private static uint DirectHash(int day, int sequence, uint salt)
        {
            unchecked
            {
                uint x = (uint)day * 0x9E3779B9u;
                x ^= (uint)sequence * 0x85EBCA6Bu;
                x ^= salt;
                x ^= x >> 16;
                x *= 0x7FEB352Du;
                x ^= x >> 15;
                x *= 0x846CA68Bu;
                x ^= x >> 16;
                return x;
            }
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
