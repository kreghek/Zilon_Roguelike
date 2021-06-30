using System;

using Zilon.Core.PersonModules;

namespace Zilon.Core.Tactics.Behaviour
{
    class SwitchToCombatModeTask : OneTurnActorTaskBase
    {
        public SwitchToCombatModeTask(IActor actor, IActorTaskContext context) : base(actor, context)
        {
        }

        protected override void ExecuteTask()
        {
            Actor.Person.GetModule<ICombatActModule>().IsCombatMode = true;
        }
    }
}
