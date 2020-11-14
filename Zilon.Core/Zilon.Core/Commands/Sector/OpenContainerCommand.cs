using Zilon.Core.Client;
using Zilon.Core.Graphs;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Commands
{
    /// <summary>
    ///     Команда открытие контейнера.
    /// </summary>
    public class OpenContainerCommand : ActorCommandBase
    {
        private readonly IPlayer _player;

        [ExcludeFromCodeCoverage]
        public OpenContainerCommand(
            IPlayer player,
            ISectorUiState playerState) :
            base(playerState)
        {
            _player = player;
        }

        public override bool CanExecute()
        {
            ISectorMap map = _player.SectorNode.Sector.Map;

            IGraphNode currentNode = PlayerState.ActiveActor.Actor.Node;

            IContainerViewModel targetContainerViewModel = GetSelectedNodeViewModel();
            if (targetContainerViewModel == null)
            {
                return false;
            }

            IStaticObject container = targetContainerViewModel.StaticObject;
            var requiredDistance = 1;

            IGraphNode targetNode = container.Node;

            var distance = map.DistanceBetween(currentNode, targetNode);
            if (distance > requiredDistance)
            {
                return false;
            }

            var containerIsOnLine = map.TargetIsOnLine(currentNode, targetNode);
            if (!containerIsOnLine)
            {
                return false;
            }

            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            IContainerViewModel targetContainerViewModel = GetSelectedNodeViewModel();
            if (targetContainerViewModel == null)
            {
                throw new InvalidOperationException("Невозможно выполнить команду. Целевой контейнер не выбран.");
            }

            IStaticObject staticObject = targetContainerViewModel.StaticObject;
            if (staticObject == null)
            {
                throw new InvalidOperationException(
                    "Невозможно выполнить команду. Целевая модель представления не содержит ссылки на контейнер.");
            }

            Intention<OpenContainerTask> intetion =
                new Intention<OpenContainerTask>(actor => CreateTask(actor, staticObject));
            PlayerState.TaskSource.Intent(intetion, PlayerState.ActiveActor.Actor);
        }

        private OpenContainerTask CreateTask(IActor actor, IStaticObject staticObject)
        {
            HandOpenContainerMethod openMethod = new HandOpenContainerMethod();

            ActorTaskContext taskContext = new ActorTaskContext(_player.SectorNode.Sector);

            return new OpenContainerTask(actor, taskContext, staticObject, openMethod);
        }

        private IContainerViewModel GetSelectedNodeViewModel()
        {
            return PlayerState.HoverViewModel as IContainerViewModel;
        }
    }
}