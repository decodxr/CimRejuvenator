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
        public const int SweepsPerDay = 8;

        public static int SeniorsLastScan { get; private set; }
        public static int RejuvenatedToday { get; private set; }
        public static int RejuvenatedSession { get; private set; }

        private SimulationSystem m_SimulationSystem;
        private EntityQuery m_CitizenQuery;
        private EntityQuery m_TimeDataQuery;
        private int m_CurrentDay = int.MinValue;

        public static void ResetStatistics()
        {
            SeniorsLastScan = 0;
            RejuvenatedToday = 0;
            RejuvenatedSession = 0;
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
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return FramesPerDay / SweepsPerDay;
        }

        protected override void OnUpdate()
        {
            var setting = Mod.Setting;
            if (setting == null || !setting.EnableMod)
            {
                return;
            }

            var chance = Clamp(setting.RejuvenationChance, 0, 100);
            if (chance <= 0)
            {
                return;
            }

            var timeData = m_TimeDataQuery.GetSingleton<TimeData>();
            var day = TimeSystem.GetDay(m_SimulationSystem.frameIndex, timeData);

            if (day != m_CurrentDay)
            {
                m_CurrentDay = day;
                RejuvenatedToday = 0;
            }

            var dailyLimit = Clamp(setting.MaxRejuvenationsPerDay, 100, 20000);
            var remainingToday = dailyLimit - RejuvenatedToday;

            var entities = m_CitizenQuery.ToEntityArray(Allocator.Temp);
            var citizens = m_CitizenQuery.ToComponentDataArray<Citizen>(Allocator.Temp);

            var seniorCount = 0;
            var rejuvenatedThisSweep = 0;

            for (var i = 0; i < entities.Length; i++)
            {
                var citizen = citizens[i];
                if (citizen.GetAge() != CitizenAge.Elderly)
                {
                    continue;
                }

                var entity = entities[i];

                if (EntityManager.HasComponent<HealthProblem>(entity))
                {
                    var problem = EntityManager.GetComponentData<HealthProblem>(entity);
                    if ((problem.m_Flags & HealthProblemFlags.Dead) != 0)
                    {
                        continue;
                    }
                }

                seniorCount++;

                if (!PassesChance(entity, citizen.m_BirthDay, chance))
                {
                    continue;
                }

                if (remainingToday <= 0)
                {
                    continue;
                }

                var resetAge = Clamp(setting.ResetAgeDays, 36, 70);

                citizen.SetAge(CitizenAge.Adult);
                citizen.m_BirthDay = day - resetAge;
                citizen.m_State |= CitizenFlags.NeedsNewJob;

                if (setting.RestoreHealth && citizen.m_Health < 80)
                {
                    citizen.m_Health = 80;
                }

                EntityManager.SetComponentData(entity, citizen);

                remainingToday--;
                RejuvenatedToday++;
                RejuvenatedSession++;
                rejuvenatedThisSweep++;
            }

            SeniorsLastScan = seniorCount;

            citizens.Dispose();
            entities.Dispose();

            if (rejuvenatedThisSweep > 0)
            {
                Mod.Log.Info(
                    $"Rejuvenated {rejuvenatedThisSweep} cim(s) this sweep. " +
                    $"Today: {RejuvenatedToday}/{dailyLimit}; session: {RejuvenatedSession}.");
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
