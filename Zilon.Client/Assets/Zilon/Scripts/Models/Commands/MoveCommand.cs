using Zilon.Logic.Tactics;

namespace Assets.Zilon.Scripts.Models.Commands
{
    class MoveCommand : CombatCommandBase
    {
        private readonly CombatSquadVM squadVM;
        private readonly CombatLocationVM nodeVM;

        public MoveCommand(Combat combat, CombatSquadVM squadVM, CombatLocationVM nodeVM): base(combat)
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
            combat.Move(squadVM.ActorSquad, nodeVM.Node);
        }
    }
}