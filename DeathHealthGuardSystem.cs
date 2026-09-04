using Game;
using Game.Citizens;
using Game.Common;
using Game.Tools;
using Unity.Entities;

namespace CimRejuvenator
{
    /// <summary>
    /// Runs after HealthProblemSystem and clears any fatal state that was produced by late health,
    /// danger, trapped, or disaster-related processing during the main simulation phase.
    /// </summary>
    public partial class DeathHealthGuardSystem : GameSystemBase
    {
        private EntityQuery m_Query;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Query = GetEntityQuery(new EntityQueryDesc
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

        public override int GetUpdateInterval(SystemUpdatePhase phase) => 1;

        protected override void OnUpdate()
        {
            if (Mod.Setting != null && Mod.Setting.EnableMod)
            {
                DeathProtectionCore.RescueDeadResidents(EntityManager, m_Query, "late health death guard");
            }
        }
    }
}
