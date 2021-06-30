using Zilon.Core.PersonModules;

namespace Zilon.Core.Tactics.Behaviour
{
    public class SwitchToCombatModeTask : OneTurnActorTaskBase
    {
        public SwitchToCombatModeTask(IActor actor, IActorTaskContext context) : base(actor, context)
        {
        }

        protected override void ExecuteTask()
        {
            Actor.Person.GetModule<ICombatActModule>().BeginCombat();
        }
    }

    public class SwitchToIdleModeTask : OneTurnActorTaskBase
    {
        public SwitchToIdleModeTask(IActor actor, IActorTaskContext context) : base(actor, context)
        {
        }

        protected override void ExecuteTask()
        {
            Actor.Person.GetModule<ICombatActModule>().EndCombat();
        }
    }
}
