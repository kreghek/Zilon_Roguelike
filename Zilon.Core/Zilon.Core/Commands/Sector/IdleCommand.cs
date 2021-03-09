using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Command to wait some time.
    /// </summary>
    public class IdleCommand : ActorCommandBase
    {
        private readonly IPlayer _player;

        public IdleCommand(
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
            var intention = new Intention<IdleTask>(actor => CreateIdleTask(actor));
            var activeActor = PlayerState.ActiveActor?.Actor;
            if (activeActor is null)
            {
                throw new System.InvalidOperationException();
            }

            PlayerState.TaskSource.Intent(intention, activeActor);
        }

        private IdleTask CreateIdleTask(IActor actor)
        {
            var taskContext = new ActorTaskContext(_player.SectorNode.Sector);

            return new IdleTask(actor, taskContext, GlobeMetrics.OneIterationLength);
        }
    }
}