using Zilon.Core.Client;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Core.Commands
{
    public class NextTurnCommand : ActorCommandBase
    {
        private readonly IDecisionSource _decisionSource;

        public override bool CanExecute()
        {
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            var intention = new Intention<IdleTask>(actor => new IdleTask(actor, _decisionSource));
            _playerState.TaskSource.Intent(intention);
        }

        public NextTurnCommand(IDecisionSource decisionSource, IPlayerState playerState) :
            base(playerState)
        {
            _decisionSource = decisionSource;
        }
    }
}