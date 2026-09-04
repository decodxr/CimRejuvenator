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
    /// Prevents established residents from being lost to death while Cim Rejuvenator is enabled.
    /// The vanilla DeathCheckSystem is suspended so normal old-age and sickness/injury death rolls
    /// never complete. Additional passes clear Dead flags created by event and health-problem paths
    /// before resident removal can consume the corpse. Households may still leave the city normally.
    /// </summary>
    public partial class DeathProtectionSystem : GameSystemBase
    {
        public static int PreventedLastPass { get; private set; }
        public static int PreventedSession { get; private set; }
        public static string Status { get; private set; } = "Disabled";

        private DeathCheckSystem m_DeathCheckSystem;
        private EntityQuery m_DeadResidentQuery;
        private bool m_HasDeathCheckControl;
        private bool m_PreviousDeathCheckEnabled = true;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_DeathCheckSystem = World.GetOrCreateSystemManaged<DeathCheckSystem>();
            m_DeadResidentQuery = GetEntityQuery(new EntityQueryDesc
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

            Mod.Log.Info("DeathProtectionSystem initialized.");
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            // This system is intentionally cheap: its query only contains citizens that already
            // have HealthProblem. Running every scheduled pass minimizes the window in which a
            // death flag can survive long enough to reach corpse removal.
            return 1;
        }

        protected override void OnUpdate()
        {
            var setting = Mod.Setting;
            var enabled = setting != null && setting.EnableMod;

            if (!enabled)
            {
                ReleaseDeathCheckControl();
                PreventedLastPass = 0;
                Status = "Disabled";
                return;
            }

            TakeDeathCheckControl();
            PreventedLastPass = RescueDeadResidents();

            if (PreventedLastPass > 0)
            {
                PreventedSession += PreventedLastPass;
                Status = $"Death lock active: rescued {PreventedLastPass:N0} resident(s) this pass";
                Mod.Log.Info(
                    $"Death lock cleared Dead state from {PreventedLastPass:N0} established resident(s). " +
                    $"Session total={PreventedSession:N0}.");
            }
            else
            {
                Status = "Death lock active";
            }
        }

        protected override void OnDestroy()
        {
            ReleaseDeathCheckControl();
            base.OnDestroy();
        }

        private void TakeDeathCheckControl()
        {
            if (!m_HasDeathCheckControl)
            {
                m_PreviousDeathCheckEnabled = m_DeathCheckSystem.Enabled;
                m_HasDeathCheckControl = true;
                Mod.Log.Info(
                    $"Death lock took control of vanilla DeathCheckSystem. Previous enabled state={m_PreviousDeathCheckEnabled}.");
            }

            // DeathCheckSystem is where normal old-age and sickness/injury deaths are committed.
            // Keep it disabled for as long as the master mod switch is enabled.
            if (m_DeathCheckSystem.Enabled)
            {
                m_DeathCheckSystem.Enabled = false;
            }
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
                $"Death lock released vanilla DeathCheckSystem. Restored enabled state={m_PreviousDeathCheckEnabled}.");
        }

        private int RescueDeadResidents()
        {
            if (m_DeadResidentQuery.IsEmptyIgnoreFilter)
            {
                return 0;
            }

            var entities = m_DeadResidentQuery.ToEntityArray(Allocator.Temp);
            var citizens = m_DeadResidentQuery.ToComponentDataArray<Citizen>(Allocator.Temp);
            var problems = m_DeadResidentQuery.ToComponentDataArray<HealthProblem>(Allocator.Temp);
            var members = m_DeadResidentQuery.ToComponentDataArray<HouseholdMember>(Allocator.Temp);

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

                if (!IsProtectedResident(entity, citizen, householdEntity))
                {
                    continue;
                }

                // A protected resident is revived in-place. Preserve the citizen entity and
                // household links, but clear all states that would immediately route it back into
                // deathcare or another fatal health cycle.
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

                EntityManager.SetComponentData(entity, problem);
                EntityManager.SetComponentData(entity, citizen);
                rescued++;
            }

            members.Dispose();
            problems.Dispose();
            citizens.Dispose();
            entities.Dispose();

            return rescued;
        }

        private bool IsProtectedResident(Entity citizenEntity, Citizen citizen, Entity householdEntity)
        {
            if ((citizen.m_State & (CitizenFlags.Tourist | CitizenFlags.Commuter)) != 0)
            {
                return false;
            }

            if (householdEntity == Entity.Null ||
                !EntityManager.Exists(householdEntity) ||
                !EntityManager.HasComponent<Household>(householdEntity))
            {
                return false;
            }

            if (EntityManager.HasComponent<TouristHousehold>(householdEntity) ||
                EntityManager.HasComponent<CommuterHousehold>(householdEntity))
            {
                return false;
            }

            var household = EntityManager.GetComponentData<Household>(householdEntity);
            return (household.m_Flags & HouseholdFlags.MovedIn) != 0;
        }
    }
}
