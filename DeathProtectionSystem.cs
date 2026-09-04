using Game;
using Game.Citizens;
using Game.Common;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace CimRejuvenator
{
    internal static class DeathProtectionCore
    {
        internal static int PreventedLastPass { get; private set; }
        internal static int PreventedSession { get; private set; }
        internal static string Status { get; private set; } = "Disabled";

        internal static EntityQuery CreateQuery(GameSystemBase system)
        {
            // Kept here only as documentation. GetEntityQuery is protected, so each guard creates
            // the same query in its own OnCreate method.
            return default;
        }

        internal static int RescueDeadResidents(EntityManager entityManager, EntityQuery query, string guardName)
        {
            if (query.IsEmptyIgnoreFilter)
            {
                PreventedLastPass = 0;
                Status = "Death lock active";
                return 0;
            }

            var entities = query.ToEntityArray(Allocator.Temp);
            var citizens = query.ToComponentDataArray<Citizen>(Allocator.Temp);
            var problems = query.ToComponentDataArray<HealthProblem>(Allocator.Temp);
            var members = query.ToComponentDataArray<HouseholdMember>(Allocator.Temp);

            var rescued = 0;

            for (var i = 0; i < entities.Length; i++)
            {
                var problem = problems[i];
                if ((problem.m_Flags & HealthProblemFlags.Dead) == 0)
                {
                    continue;
                }

                var entity = entities[i];
                var citizen = citizens[i];
                var householdEntity = members[i].m_Household;

                if (!IsProtectedResident(entityManager, citizen, householdEntity))
                {
                    continue;
                }

                // Revive in place. The citizen entity, household membership, identity, education,
                // and other components are preserved. Fatal/transport states are cleared and the
                // resident is healed so the same health problem cannot immediately kill it again.
                problem.m_Flags &= ~(
                    HealthProblemFlags.Dead |
                    HealthProblemFlags.RequireTransport |
                    HealthProblemFlags.InDanger |
                    HealthProblemFlags.Trapped |
                    HealthProblemFlags.Sick |
                    HealthProblemFlags.Injured);
                problem.m_Timer = 0;
                problem.m_Event = Entity.Null;
                citizen.m_Health = 100;

                entityManager.SetComponentData(entity, problem);
                entityManager.SetComponentData(entity, citizen);
                rescued++;
            }

            members.Dispose();
            problems.Dispose();
            citizens.Dispose();
            entities.Dispose();

            PreventedLastPass = rescued;
            if (rescued > 0)
            {
                PreventedSession += rescued;
                Status = $"Death lock active: rescued {rescued:N0} resident(s)";
                Mod.Log.Info(
                    $"{guardName}: cleared Dead state from {rescued:N0} established resident(s); " +
                    $"session prevented={PreventedSession:N0}.");
            }
            else
            {
                Status = "Death lock active";
            }

            return rescued;
        }

        internal static void SetDisabled()
        {
            PreventedLastPass = 0;
            Status = "Disabled";
        }

        private static bool IsProtectedResident(EntityManager entityManager, Citizen citizen, Entity householdEntity)
        {
            if ((citizen.m_State & (CitizenFlags.Tourist | CitizenFlags.Commuter)) != 0)
            {
                return false;
            }

            if (householdEntity == Entity.Null ||
                !entityManager.Exists(householdEntity) ||
                !entityManager.HasComponent<Household>(householdEntity))
            {
                return false;
            }

            if (entityManager.HasComponent<TouristHousehold>(householdEntity) ||
                entityManager.HasComponent<CommuterHousehold>(householdEntity))
            {
                return false;
            }

            var household = entityManager.GetComponentData<Household>(householdEntity);
            return (household.m_Flags & HouseholdFlags.MovedIn) != 0;
        }
    }

    /// <summary>
    /// Main death lock. It disables the vanilla DeathCheckSystem, preventing the normal old-age
    /// and sickness/injury death rolls from committing. A late simulation rescue also catches
    /// deaths produced by HealthProblemSystem.
    /// </summary>
    public partial class DeathProtectionSystem : GameSystemBase
    {
        public static int PreventedLastPass => DeathProtectionCore.PreventedLastPass;
        public static int PreventedSession => DeathProtectionCore.PreventedSession;
        public static string Status => DeathProtectionCore.Status;

        private DeathCheckSystem m_DeathCheckSystem;
        private EntityQuery m_DeadResidentQuery;
        private bool m_HasDeathCheckControl;
        private bool m_PreviousDeathCheckEnabled = true;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_DeathCheckSystem = World.GetOrCreateSystemManaged<DeathCheckSystem>();
            m_DeadResidentQuery = CreateDeadResidentQuery();

            // The setting object exists before systems are registered in Mod.OnLoad. Taking
            // control here prevents DeathCheckSystem from getting one extra fatal pass on startup.
            if (Mod.Setting != null && Mod.Setting.EnableMod)
            {
                TakeDeathCheckControl();
            }

            Mod.Log.Info("DeathProtectionSystem initialized.");
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase) => 1;

        protected override void OnUpdate()
        {
            var enabled = Mod.Setting != null && Mod.Setting.EnableMod;
            if (!enabled)
            {
                ReleaseDeathCheckControl();
                DeathProtectionCore.SetDisabled();
                return;
            }

            TakeDeathCheckControl();
            DeathProtectionCore.RescueDeadResidents(EntityManager, m_DeadResidentQuery, "health death guard");
        }

        protected override void OnDestroy()
        {
            ReleaseDeathCheckControl();
            base.OnDestroy();
        }

        private EntityQuery CreateDeadResidentQuery()
        {
            return GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<Citizen>(),
                    ComponentType.ReadWrite<HealthProblem>(),
                    ComponentType.ReadOnly<HouseholdMember>(),
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                },
            });
        }

        private void TakeDeathCheckControl()
        {
            if (!m_HasDeathCheckControl)
            {
                m_PreviousDeathCheckEnabled = m_DeathCheckSystem.Enabled;
                m_HasDeathCheckControl = true;
                Mod.Log.Info(
                    $"Death lock took control of vanilla DeathCheckSystem; previous enabled state={m_PreviousDeathCheckEnabled}.");
            }

            m_DeathCheckSystem.Enabled = false;
        }

        private void ReleaseDeathCheckControl()
        {
            if (!m_HasDeathCheckControl || m_DeathCheckSystem == null)
            {
                return;
            }

            m_DeathCheckSystem.Enabled = m_PreviousDeathCheckEnabled;
            m_HasDeathCheckControl = false;
            Mod.Log.Info(
                $"Death lock released vanilla DeathCheckSystem; restored enabled state={m_PreviousDeathCheckEnabled}.");
        }
    }

    /// <summary>
    /// Runs immediately after SicknessCheckSystem so a fatal sickness/health event cannot reach
    /// later health processing with the resident still marked dead.
    /// </summary>
    public partial class DeathSicknessGuardSystem : GameSystemBase
    {
        private EntityQuery m_Query;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Query = CreateQuery();
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase) => 1;

        protected override void OnUpdate()
        {
            if (Mod.Setting != null && Mod.Setting.EnableMod)
            {
                DeathProtectionCore.RescueDeadResidents(EntityManager, m_Query, "sickness death guard");
            }
        }

        private EntityQuery CreateQuery()
        {
            return GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<Citizen>(),
                    ComponentType.ReadWrite<HealthProblem>(),
                    ComponentType.ReadOnly<HouseholdMember>(),
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                },
            });
        }
    }

    /// <summary>
    /// Catches disaster/event deaths after AddHealthProblemSystem has materialized the health
    /// problem, before the next removal phase can consume the resident as a corpse.
    /// </summary>
    public partial class DeathEventGuardSystem : GameSystemBase
    {
        private EntityQuery m_Query;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Query = CreateQuery();
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase) => 1;

        protected override void OnUpdate()
        {
            if (Mod.Setting != null && Mod.Setting.EnableMod)
            {
                DeathProtectionCore.RescueDeadResidents(EntityManager, m_Query, "event death guard");
            }
        }

        private EntityQuery CreateQuery()
        {
            return GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<Citizen>(),
                    ComponentType.ReadWrite<HealthProblem>(),
                    ComponentType.ReadOnly<HouseholdMember>(),
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                },
            });
        }
    }

    /// <summary>
    /// Final safety net before HouseholdAndCitizenRemoveSystem. This is deliberately separate
    /// from the simulation guards so an already-dead resident loaded from a save is revived before
    /// vanilla removal has a chance to delete it.
    /// </summary>
    public partial class DeathRemovalGuardSystem : GameSystemBase
    {
        private EntityQuery m_Query;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Query = CreateQuery();
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase) => 1;

        protected override void OnUpdate()
        {
            if (Mod.Setting != null && Mod.Setting.EnableMod)
            {
                DeathProtectionCore.RescueDeadResidents(EntityManager, m_Query, "pre-removal death guard");
            }
        }

        private EntityQuery CreateQuery()
        {
            return GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite<Citizen>(),
                    ComponentType.ReadWrite<HealthProblem>(),
                    ComponentType.ReadOnly<HouseholdMember>(),
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                },
            });
        }
    }
}
