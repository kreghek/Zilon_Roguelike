namespace Assets.Zilon.Scripts.Models.Commands
{
    class MoveCommand : CombatCommandBase
    {
        private readonly CombatSquadVM squadVM;
        private readonly CombatLocationVM nodeVM;

        public MoveCommand(CombatSquadVM squadVM, CombatLocationVM nodeVM)
        {
            this.squadVM = squadVM;
            this.nodeVM = nodeVM;
        }

        public override void Execute(ICombatCommandContext context)
        {
            context.Combat.Move(squadVM.ActorSquad, nodeVM.Node);
            squadVM.MoveActors(nodeVM);
        }
    }
}