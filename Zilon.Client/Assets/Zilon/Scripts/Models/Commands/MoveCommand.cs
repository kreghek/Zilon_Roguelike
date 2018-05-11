using Zilon.Core.Services;
using Zilon.Core.Services.CombatEvents;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Models.Commands
{
    class MoveCommand : CombatCommandBase
    {
        private readonly CombatSquadVM squadVM;
        private readonly CombatLocationVM nodeVM;
        private readonly ICombatService combatService;
        private readonly IEventManager eventManager;

        public MoveCommand(IEventManager eventManager, ICombatService combatService, Combat combat, CombatSquadVM squadVM, CombatLocationVM nodeVM): base(combat)
        {
            if (squadVM == null)
            {
                throw new System.ArgumentNullException(nameof(squadVM));
            }

            if (nodeVM == null)
            {
                throw new System.ArgumentNullException(nameof(nodeVM));
            }

            this.combatService = combatService;
            this.eventManager = eventManager;
            this.squadVM = squadVM;
            this.nodeVM = nodeVM;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            var events = combatService.MoveCommand(combat, squadVM.ActorSquad, nodeVM.Node);
            eventManager.SetEvents(events.Events);
        }
    }
}