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
    public partial class PopulationManagementSystem : GameSystemBase
    {
        public const int FramesPerDay = 262144;
        public const int SchedulerChecksPerDay = 512;

        public static int ResidentCount { get; private set; }
        public static int ChildCount { get; private set; }
        public static int TeenCount { get; private set; }
        public static int AdultCount { get; private set; }
        public static int SeniorCount { get; private set; }

        public static int RejuvenatedLastSweep { get; private set; }
        public static int RejuvenatedToday { get; private set; }
        public static int RejuvenatedSession { get; private set; }
        public static int AgeConvertedLastSweep { get; private set; }
        public static int AgeConvertedSession { get; private set; }
        public static int SweepsSession { get; private set; }
        public static int LastSimulationDay { get; private set; } = -1;

        public static double ChildPercent => Percent(ChildCount);
        public static double TeenPercent => Percent(TeenCount);
        public static double AdultPercent => Percent(AdultCount);
        public static double SeniorPercent => Percent(SeniorCount);

        private static int s_ImmediateRejuvenationRequested;
        private static int s_ImmediateBalanceRequested;

        private SimulationSystem m_SimulationSystem;
        private EntityQuery m_CitizenQuery;
        private EntityQuery m_TimeDataQuery;
        private int m_CurrentDay = int.MinValue;
        private uint m_LastSweepFrame;
        private bool m_HasSwept;

        public static void RequestImmediateRejuvenation()
        {
            Interlocked.Exchange(ref s_ImmediateRejuvenationRequested, 1);
        }

        public static void RequestImmediateBalance()
        {
            Interlocked.Exchange(ref s_ImmediateBalanceRequested, 1);
        }

        public static void ResetStatistics()
        {
            ResidentCount = 0;
            ChildCount = 0;
            TeenCount = 0;
            AdultCount = 0;
            SeniorCount = 0;
            RejuvenatedLastSweep = 0;
            RejuvenatedToday = 0;
            RejuvenatedSession = 0;
            AgeConvertedLastSweep = 0;
            AgeConvertedSession = 0;
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
                    ComponentType.ReadOnly<HouseholdMember>(),
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

            Mod.Log.Info("PopulationManagementSystem initialized.");
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

            if (m_HasSwept && frame < m_LastSweepFrame)
            {
                m_HasSwept = false;
                m_LastSweepFrame = frame;
                RejuvenatedToday = 0;
            }

            var manualRejuvenation = Interlocked.Exchange(ref s_ImmediateRejuvenationRequested, 0) != 0;
            var manualBalance = Interlocked.Exchange(ref s_ImmediateBalanceRequested, 0) != 0;
            var sweepsPerDay = Clamp(setting.SweepsPerDay, 8, 256);
            var sweepInterval = (uint)Math.Max(1, FramesPerDay / sweepsPerDay);

            if (!manualRejuvenation && !manualBalance && m_HasSwept && frame - m_LastSweepFrame < sweepInterval)
            {
                return;
            }

            m_HasSwept = true;
            m_LastSweepFrame = frame;
            RunSweep(setting, day, manualRejuvenation, manualBalance);
        }

        private void RunSweep(
            CimRejuvenatorSetting setting,
            int day,
            bool manualRejuvenation,
            bool manualBalance)
        {
            var entities = m_CitizenQuery.ToEntityArray(Allocator.Temp);
            var citizens = m_CitizenQuery.ToComponentDataArray<Citizen>(Allocator.Temp);

            var counts = new int[4];
            var residentCount = 0;

            for (var i = 0; i < entities.Length; i++)
            {
                var citizen = citizens[i];
                if (!IsEstablishedLivingResident(entities[i], citizen))
                {
                    continue;
                }

                residentCount++;
                var ageIndex = (int)citizen.GetAge();
                if (ageIndex >= 0 && ageIndex < counts.Length)
                {
                    counts[ageIndex]++;
                }
            }

            ResidentCount = residentCount;
            ChildCount = counts[(int)CitizenAge.Child];
            TeenCount = counts[(int)CitizenAge.Teen];
            AdultCount = counts[(int)CitizenAge.Adult];
            SeniorCount = counts[(int)CitizenAge.Elderly];
            RejuvenatedLastSweep = 0;
            AgeConvertedLastSweep = 0;
            LastSimulationDay = day;
            SweepsSession++;

            if ((setting.EnableRejuvenation || manualRejuvenation) && residentCount > 0)
            {
                ApplyRejuvenation(setting, day, entities, citizens, counts, residentCount);
            }

            if ((setting.EnableDemographicBalancer || manualBalance) && residentCount > 0)
            {
                ApplyDemographicBalance(setting, day, entities, citizens, counts, residentCount);
            }

            ChildCount = counts[(int)CitizenAge.Child];
            TeenCount = counts[(int)CitizenAge.Teen];
            AdultCount = counts[(int)CitizenAge.Adult];
            SeniorCount = counts[(int)CitizenAge.Elderly];

            citizens.Dispose();
            entities.Dispose();

            var mode = manualRejuvenation || manualBalance ? "manual" : "automatic";
            Mod.Log.Info(
                $"Completed {mode} population sweep: residents={ResidentCount}, " +
                $"ages={ChildCount}/{TeenCount}/{AdultCount}/{SeniorCount}, " +
                $"rejuvenated={RejuvenatedLastSweep}, balanced={AgeConvertedLastSweep}.");
        }

        private void ApplyRejuvenation(
            CimRejuvenatorSetting setting,
            int day,
            NativeArray<Entity> entities,
            NativeArray<Citizen> citizens,
            int[] counts,
            int residentCount)
        {
            var chance = Clamp(setting.RejuvenationChance, 0, 100);
            if (chance <= 0)
            {
                return;
            }

            var dailyLimit = Clamp(setting.MaxRejuvenationsPerDay, 100, 250000);
            var perSweepLimit = Clamp(setting.MaxRejuvenationsPerSweep, 100, 100000);
            var budget = Math.Min(perSweepLimit, Math.Max(0, dailyLimit - RejuvenatedToday));

            if (setting.KeepMinimumSeniorShare)
            {
                var minimumSeniorPercent = Clamp(setting.MinimumSeniorPercent, 0, 50);
                var minimumSeniors = (int)Math.Ceiling(residentCount * (minimumSeniorPercent / 100.0));
                budget = Math.Min(budget, Math.Max(0, counts[(int)CitizenAge.Elderly] - minimumSeniors));
            }

            if (budget <= 0)
            {
                return;
            }

            var resetAge = Clamp(setting.ResetAgeDays, 36, 70);

            for (var i = 0; i < entities.Length && budget > 0; i++)
            {
                var citizen = citizens[i];
                var entity = entities[i];

                if (!IsEstablishedLivingResident(entity, citizen) || citizen.GetAge() != CitizenAge.Elderly)
                {
                    continue;
                }

                if (!PassesChance(entity, citizen.m_BirthDay, chance))
                {
                    continue;
                }

                citizen.SetAge(CitizenAge.Adult);
                citizen.m_BirthDay = ToBirthDay(day, resetAge);

                if (setting.RestoreHealth && citizen.m_Health < 80)
                {
                    citizen.m_Health = 80;
                }

                EntityManager.SetComponentData(entity, citizen);
                citizens[i] = citizen;

                counts[(int)CitizenAge.Elderly]--;
                counts[(int)CitizenAge.Adult]++;
                budget--;
                RejuvenatedLastSweep++;
                RejuvenatedToday++;
                RejuvenatedSession++;
            }
        }

        private void ApplyDemographicBalance(
            CimRejuvenatorSetting setting,
            int day,
            NativeArray<Entity> entities,
            NativeArray<Citizen> citizens,
            int[] counts,
            int residentCount)
        {
            var weights = new[]
            {
                Math.Max(0, setting.TargetChildPercent),
                Math.Max(0, setting.TargetTeenPercent),
                Math.Max(0, setting.TargetAdultPercent),
                Math.Max(0, setting.TargetSeniorPercent),
            };

            var totalWeight = weights[0] + weights[1] + weights[2] + weights[3];
            if (totalWeight <= 0)
            {
                return;
            }

            var desired = BuildDesiredCounts(residentCount, weights, totalWeight);
            var budget = Clamp(setting.MaxAgeConversionsPerSweep, 100, 100000);

            for (var i = 0; i < entities.Length && budget > 0; i++)
            {
                var entity = entities[i];
                var citizen = citizens[i];
                if (!IsEstablishedLivingResident(entity, citizen))
                {
                    continue;
                }

                var sourceAge = citizen.GetAge();
                var sourceIndex = (int)sourceAge;
                if (sourceIndex < 0 || sourceIndex >= 4 || counts[sourceIndex] <= desired[sourceIndex])
                {
                    continue;
                }

                var targetIndex = LargestDeficit(counts, desired);
                if (targetIndex < 0 || targetIndex == sourceIndex)
                {
                    break;
                }

                // Avoid rewriting life stage while the citizen is in an active trip or enrolled.
                // Those transitions have additional vanilla side effects that should be allowed to finish normally.
                if (EntityManager.HasComponent<TravelPurpose>(entity) || EntityManager.HasComponent<Student>(entity))
                {
                    continue;
                }

                var targetAge = (CitizenAge)targetIndex;
                var hasWorker = EntityManager.HasComponent<Worker>(entity);
                if (hasWorker && targetAge != CitizenAge.Adult && setting.ProtectWorkersWhenBalancing)
                {
                    continue;
                }

                if (hasWorker && targetAge != CitizenAge.Adult)
                {
                    EntityManager.RemoveComponent<Worker>(entity);
                }

                if (targetAge == CitizenAge.Child || targetAge == CitizenAge.Teen)
                {
                    citizen.m_State &= ~CitizenFlags.LookingForPartner;
                }

                citizen.SetAge(targetAge);
                citizen.m_BirthDay = ToBirthDay(day, AgeForStage(entity, targetAge));
                EntityManager.SetComponentData(entity, citizen);
                citizens[i] = citizen;

                counts[sourceIndex]--;
                counts[targetIndex]++;
                budget--;
                AgeConvertedLastSweep++;
                AgeConvertedSession++;
            }
        }

        private static int[] BuildDesiredCounts(int population, int[] weights, int totalWeight)
        {
            var desired = new int[4];
            var assigned = 0;

            for (var i = 0; i < 4; i++)
            {
                desired[i] = (int)Math.Floor(population * (weights[i] / (double)totalWeight));
                assigned += desired[i];
            }

            while (assigned < population)
            {
                var bestIndex = 0;
                var bestRemainder = double.MinValue;
                for (var i = 0; i < 4; i++)
                {
                    var exact = population * (weights[i] / (double)totalWeight);
                    var remainder = exact - desired[i];
                    if (remainder > bestRemainder)
                    {
                        bestRemainder = remainder;
                        bestIndex = i;
                    }
                }

                desired[bestIndex]++;
                assigned++;
            }

            return desired;
        }

        private static int LargestDeficit(int[] counts, int[] desired)
        {
            var bestIndex = -1;
            var bestDeficit = 0;

            for (var i = 0; i < 4; i++)
            {
                var deficit = desired[i] - counts[i];
                if (deficit > bestDeficit)
                {
                    bestDeficit = deficit;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private bool IsEstablishedLivingResident(Entity entity, Citizen citizen)
        {
            if ((citizen.m_State & (CitizenFlags.Tourist | CitizenFlags.Commuter)) != 0)
            {
                return false;
            }

            if (EntityManager.HasComponent<HealthProblem>(entity))
            {
                var problem = EntityManager.GetComponentData<HealthProblem>(entity);
                if ((problem.m_Flags & HealthProblemFlags.Dead) != 0)
                {
                    return false;
                }
            }

            if (!EntityManager.HasComponent<HouseholdMember>(entity))
            {
                return false;
            }

            var member = EntityManager.GetComponentData<HouseholdMember>(entity);
            if (!EntityManager.Exists(member.m_Household) || !EntityManager.HasComponent<Household>(member.m_Household))
            {
                return false;
            }

            var household = EntityManager.GetComponentData<Household>(member.m_Household);
            return (household.m_Flags & HouseholdFlags.MovedIn) != 0;
        }

        internal static short ToBirthDay(int day, int ageInDays)
        {
            var value = Clamp(day - ageInDays, short.MinValue, short.MaxValue);
            return (short)value;
        }

        internal static int AgeForStage(Entity entity, CitizenAge age)
        {
            switch (age)
            {
                case CitizenAge.Child:
                    return 1 + (int)(StableHash(entity, 0x31u) % 20u);
                case CitizenAge.Teen:
                    return 21 + (int)(StableHash(entity, 0x53u) % 15u);
                case CitizenAge.Adult:
                    return 36 + (int)(StableHash(entity, 0x79u) % 35u);
                case CitizenAge.Elderly:
                    return 84 + (int)(StableHash(entity, 0xA7u) % 5u);
                default:
                    return 40;
            }
        }

        internal static uint StableHash(Entity entity, uint salt)
        {
            unchecked
            {
                uint x = (uint)entity.Index;
                x ^= (uint)entity.Version * 0x9E3779B9u;
                x ^= salt * 0x85EBCA6Bu;
                x ^= x >> 16;
                x *= 0x7FEB352Du;
                x ^= x >> 15;
                x *= 0x846CA68Bu;
                x ^= x >> 16;
                return x;
            }
        }

        private static bool PassesChance(Entity entity, int birthDay, int chance)
        {
            if (chance >= 100)
            {
                return true;
            }

            unchecked
            {
                var x = StableHash(entity, (uint)birthDay);
                return (x % 100u) < (uint)chance;
            }
        }

        private static double Percent(int count)
        {
            return ResidentCount <= 0 ? 0.0 : count * 100.0 / ResidentCount;
        }

        internal static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }
    }
}
