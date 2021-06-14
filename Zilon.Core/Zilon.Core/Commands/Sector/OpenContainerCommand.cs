using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда открытие контейнера.
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

        public override CanExecuteCheckResult CanExecute()
        {
            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var map = sector.Map;

            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            var currentNode = actor.Node;

            var targetContainerViewModel = GetSelectedNodeViewModel();
            if (targetContainerViewModel == null)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            var container = targetContainerViewModel.StaticObject;
            var requiredDistance = 1;

            var targetNode = container.Node;

            var distance = map.DistanceBetween(currentNode, targetNode);
            if (distance > requiredDistance)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            var containerIsOnLine = map.TargetIsOnLine(currentNode, targetNode);
            if (!containerIsOnLine)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            return new CanExecuteCheckResult { IsSuccess = true };
        }

        protected override void ExecuteTacticCommand()
        {
            var targetContainerViewModel = GetSelectedNodeViewModel();
            if (targetContainerViewModel is null)
            {
                throw new InvalidOperationException("Невозможно выполнить команду. Целевой контейнер не выбран.");
            }

            var staticObject = targetContainerViewModel.StaticObject;
            if (staticObject == null)
            {
                throw new InvalidOperationException(
                    "Невозможно выполнить команду. Целевая модель представления не содержит ссылки на контейнер.");
            }

            var intetion = new Intention<OpenContainerTask>(actor => CreateTask(actor, staticObject));
            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var taskSource = PlayerState?.TaskSource;
            if (taskSource is null)
            {
                throw new InvalidOperationException();
            }

            taskSource.Intent(intetion, actor);
        }

        private OpenContainerTask CreateTask(IActor actor, IStaticObject staticObject)
        {
            var openMethod = new HandOpenContainerMethod();

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var taskContext = new ActorTaskContext(sector);

            return new OpenContainerTask(actor, taskContext, staticObject, openMethod);
        }

        private IContainerViewModel? GetSelectedNodeViewModel()
        {
            return PlayerState.HoverViewModel as IContainerViewModel;
        }
    }
}