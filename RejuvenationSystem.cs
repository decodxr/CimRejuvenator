using System;
using System.Threading;
using Game;
using Game.Citizens;
using Game.Common;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace CimRejuvenator
{
    /// <summary>
    /// Rejuvenates a configurable percentage of living elderly cims back to Adult.
    /// The same cim is kept: household, education, relationships and identity stay intact.
    /// </summary>
    public partial class RejuvenationSystem : GameSystemBase
    {
        public const int FramesPerDay = 262144;

        // The system wakes up frequently so a manual "Rejuvenate now" request is handled quickly.
        // Full population scans are still throttled by the user's SweepsPerDay setting.
        public const int SchedulerChecksPerDay = 512;

        public static int SeniorsLastScan { get; private set; }
        public static int CitizensLastScan { get; private set; }
        public static int RejuvenatedLastSweep { get; private set; }
        public static int RejuvenatedToday { get; private set; }
        public static int RejuvenatedSession { get; private set; }
        public static int SweepsSession { get; private set; }
        public static int LastSimulationDay { get; private set; } = -1;

        private static int s_ImmediateSweepRequested;

        private SimulationSystem m_SimulationSystem;
        private EntityQuery m_CitizenQuery;
        private EntityQuery m_TimeDataQuery;
        private int m_CurrentDay = int.MinValue;
        private uint m_LastSweepFrame;
        private bool m_HasSwept;

        public static double ElderlyPercentLastScan
        {
            get
            {
                return CitizensLastScan <= 0
                    ? 0.0
                    : SeniorsLastScan * 100.0 / CitizensLastScan;
            }
        }

        public static void RequestImmediateSweep()
        {
            Interlocked.Exchange(ref s_ImmediateSweepRequested, 1);
        }

        public static void ResetStatistics()
        {
            SeniorsLastScan = 0;
            CitizensLastScan = 0;
            RejuvenatedLastSweep = 0;
            RejuvenatedToday = 0;
            RejuvenatedSession = 0;
            SweepsSession = 0;
            LastSimulationDay = -1;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();

            m_CitizenQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<Citizen>(),
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                },
            });

            m_TimeDataQuery = GetEntityQuery(ComponentType.ReadOnly<TimeData>());
            RequireForUpdate(m_CitizenQuery);
            RequireForUpdate(m_TimeDataQuery);

            Mod.Log.Info(
                "RejuvenationSystem created. Scheduler checks: " +
                $"{SchedulerChecksPerDay}/day; first sweep will run as soon as simulation updates.");
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return Math.Max(1, FramesPerDay / SchedulerChecksPerDay);
        }

        protected override void OnUpdate()
        {
            var setting = Mod.Setting;
            if (setting == null || !setting.EnableMod)
            {
                return;
            }

            var frame = m_SimulationSystem.frameIndex;
            var timeData = m_TimeDataQuery.GetSingleton<TimeData>();
            var day = TimeSystem.GetDay(frame, timeData);

            if (day != m_CurrentDay)
            {
                m_CurrentDay = day;
                RejuvenatedToday = 0;
            }

            // Loading an older save can move the simulation frame backwards.
            if (m_HasSwept && frame < m_LastSweepFrame)
            {
                m_HasSwept = false;
                m_LastSweepFrame = frame;
                RejuvenatedToday = 0;
            }

            var immediate = Interlocked.Exchange(ref s_ImmediateSweepRequested, 0) != 0;
            var sweepsPerDay = Clamp(setting.SweepsPerDay, 1, 256);
            var sweepInterval = (uint)Math.Max(1, FramesPerDay / sweepsPerDay);

            // First scan happens immediately after the simulation begins.
            if (!immediate && m_HasSwept && frame - m_LastSweepFrame < sweepInterval)
            {
                return;
            }

            m_HasSwept = true;
            m_LastSweepFrame = frame;
            RunSweep(setting, day, immediate);
        }

        private void RunSweep(CimRejuvenatorSetting setting, int day, bool immediate)
        {
            var chance = Clamp(setting.RejuvenationChance, 0, 100);
            var dailyLimit = Clamp(setting.MaxRejuvenationsPerDay, 100, 250000);
            var perSweepLimit = Clamp(setting.MaxRejuvenationsPerSweep, 100, 100000);
            var remainingToday = Math.Max(0, dailyLimit - RejuvenatedToday);

            var entities = m_CitizenQuery.ToEntityArray(Allocator.Temp);
            var citizens = m_CitizenQuery.ToComponentDataArray<Citizen>(Allocator.Temp);

            var seniorCount = 0;
            var citizenCount = entities.Length;

            // Pass 1: get the current elderly count before changing anybody.
            for (var i = 0; i < entities.Length; i++)
            {
                var citizen = citizens[i];
                if (citizen.GetAge() != CitizenAge.Elderly)
                {
                    continue;
                }

                if (IsDead(entities[i]))
                {
                    continue;
                }

                seniorCount++;
            }

            SeniorsLastScan = seniorCount;
            CitizensLastScan = citizenCount;
            RejuvenatedLastSweep = 0;
            LastSimulationDay = day;
            SweepsSession++;

            var demographicAllowance = seniorCount;
            if (setting.KeepMinimumSeniorShare && citizenCount > 0)
            {
                var targetPercent = Clamp(setting.MinimumSeniorPercent, 0, 50);
                var minimumSeniors = (int)Math.Ceiling(citizenCount * (targetPercent / 100.0));
                demographicAllowance = Math.Max(0, seniorCount - minimumSeniors);
            }

            var sweepBudget = Math.Min(perSweepLimit, remainingToday);
            sweepBudget = Math.Min(sweepBudget, demographicAllowance);

            if (chance > 0 && sweepBudget > 0)
            {
                var resetAge = Clamp(setting.ResetAgeDays, 36, 70);

                // Pass 2: apply rejuvenation up to the configured budget.
                for (var i = 0; i < entities.Length && sweepBudget > 0; i++)
                {
                    var citizen = citizens[i];
                    if (citizen.GetAge() != CitizenAge.Elderly)
                    {
                        continue;
                    }

                    var entity = entities[i];
                    if (IsDead(entity))
                    {
                        continue;
                    }

                    if (!PassesChance(entity, citizen.m_BirthDay, chance))
                    {
                        continue;
                    }

                    citizen.SetAge(CitizenAge.Adult);

                    // Current game builds store m_BirthDay as a 16-bit integer.
                    // Clamp before casting so very long-running saves cannot overflow it.
                    var newBirthDay = Clamp(day - resetAge, short.MinValue, short.MaxValue);
                    citizen.m_BirthDay = (short)newBirthDay;

                    if (setting.RestoreHealth && citizen.m_Health < 80)
                    {
                        citizen.m_Health = 80;
                    }

                    EntityManager.SetComponentData(entity, citizen);

                    sweepBudget--;
                    RejuvenatedToday++;
                    RejuvenatedSession++;
                    RejuvenatedLastSweep++;
                }
            }

            citizens.Dispose();
            entities.Dispose();

            var mode = immediate ? "manual" : "automatic";
            Mod.Log.Info(
                $"Completed {mode} rejuvenation sweep: " +
                $"scanned={CitizensLastScan}, seniors={SeniorsLastScan} " +
                $"({ElderlyPercentLastScan:F1}%), rejuvenated={RejuvenatedLastSweep}, " +
                $"today={RejuvenatedToday}/{dailyLimit}, session={RejuvenatedSession}."
            );
        }

        private bool IsDead(Entity entity)
        {
            if (!EntityManager.HasComponent<HealthProblem>(entity))
            {
                return false;
            }

            var problem = EntityManager.GetComponentData<HealthProblem>(entity);
            return (problem.m_Flags & HealthProblemFlags.Dead) != 0;
        }

        private static bool PassesChance(Entity entity, int birthDay, int chance)
        {
            if (chance >= 100)
            {
                return true;
            }

            unchecked
            {
                uint x = (uint)entity.Index;
                x ^= (uint)entity.Version * 0x9E3779B9u;
                x ^= (uint)birthDay * 0x85EBCA6Bu;
                x ^= x >> 16;
                x *= 0x7FEB352Du;
                x ^= x >> 15;
                x *= 0x846CA68Bu;
                x ^= x >> 16;

                return (x % 100u) < (uint)chance;
            }
        }

        private static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }
    }
}
