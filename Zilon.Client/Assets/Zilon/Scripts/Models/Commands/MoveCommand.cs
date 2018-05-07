using Zilon.Logic.Services;
using Zilon.Logic.Tactics;

namespace Assets.Zilon.Scripts.Models.Commands
{
    class MoveCommand : CombatCommandBase
    {
        private readonly CombatSquadVM squadVM;
        private readonly CombatLocationVM nodeVM;
        private readonly ICombatService combatService;

        public MoveCommand(ICombatService combatService, Combat combat, CombatSquadVM squadVM, CombatLocationVM nodeVM): base(combat)
        {
            if (squadVM == null)
            {
                throw new System.ArgumentNullException(nameof(squadVM));
            }

            if (nodeVM == null)
            {
                throw new System.ArgumentNullException(nameof(nodeVM));
            }

            this.squadVM = squadVM;
            this.nodeVM = nodeVM;
        }

        public override void Execute()
        {
            combatService.MoveCommand(combat, squadVM.ActorSquad, nodeVM.Node);
            combat.Move(squadVM.ActorSquad, nodeVM.Node);
        }
    }
}