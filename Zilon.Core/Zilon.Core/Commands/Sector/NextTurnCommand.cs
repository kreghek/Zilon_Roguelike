using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Core.Commands
{
    public class NextTurnCommand : ActorCommandBase
    {
        private readonly IPlayer _player;

        public NextTurnCommand(
            IPlayer player,
            ISectorUiState playerState) : base(playerState)
        {
            _player = player;
        }

        public override bool CanExecute()
        {
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            var taskContext = new ActorTaskContext(_player.SectorNode.Sector);

            var intention = new Intention<IdleTask>(actor => new IdleTask(actor, taskContext, GlobeMetrics.OneIterationLength));
            PlayerState.TaskSource.Intent(intention, PlayerState.ActiveActor.Actor);
        }
    }
}