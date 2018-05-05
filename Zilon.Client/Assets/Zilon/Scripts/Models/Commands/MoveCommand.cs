using Zilon.Logic.Tactics;

namespace Assets.Zilon.Scripts.Commands
{
    class MoveCommand : CombatCommandBase
    {
        private readonly ActorSquad squad;

        public MoveCommand(Combat combat, ActorSquad squad): base(combat)
        {
            this.squad = squad;
        }

        public override void Execute()
        {
            
        }
    }
}