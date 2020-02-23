using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда открытие контейнера.
    /// </summary>
    public class OpenContainerCommand : ActorCommandBase
    {
        [ExcludeFromCodeCoverage]
        public OpenContainerCommand(IGameLoop gameLoop,
            ISectorManager sectorManager,
            ISectorUiState playerState) :
            base(gameLoop, sectorManager, playerState)
        {
        }

        public override bool CanExecute()
        {
            var map = SectorManager.CurrentSector.Map;

            var currentNode = PlayerState.ActiveActor.Actor.Node;

            var targetContainerViewModel = GetSelectedNodeViewModel();
            if (targetContainerViewModel == null)
            {
                return false;
            }

            var container = targetContainerViewModel.Container;
            var requiredDistance = container.Purpose == PropContainerPurpose.Loot ? 0 : 1;

            var targetNode = container.Node;

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
            var targetContainerViewModel = GetSelectedNodeViewModel();
            if (targetContainerViewModel == null)
            {
                throw new InvalidOperationException("Невозможно выполнить команду. Целевой контейнер не выбран.");
            }

            var container = targetContainerViewModel.Container;
            if (container == null)
            {
                throw new InvalidOperationException("Невозможно выполнить команду. Целевая модель представления не содержит ссылки на контейнер.");
            }

            var intetion = new Intention<OpenContainerTask>(actor => CreateTask(actor, container));
            PlayerState.TaskSource.Intent(intetion);
        }

        private OpenContainerTask CreateTask(IActor actor, IPropContainer container)
        {
            var openMethod = new HandOpenContainerMethod();
            return new OpenContainerTask(actor, container, openMethod, SectorManager.CurrentSector.Map);
        }

        private IContainerViewModel GetSelectedNodeViewModel()
        {
            return PlayerState.HoverViewModel as IContainerViewModel;
        }
    }
}