using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.World;

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
            PlayerState.TaskSource.Intent(intention);
        }

        [ExcludeFromCodeCoverage]
        public NextTurnCommand(IGameLoop gameLoop,
            ISectorManager sectorManager,
            ISectorUiState playerState,
            IDecisionSource decisionSource) :
            base(gameLoop, sectorManager, playerState)
        {
            _decisionSource = decisionSource;
        }
    }
}