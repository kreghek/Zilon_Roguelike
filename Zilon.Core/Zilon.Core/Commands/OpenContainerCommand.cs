using System;

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
        private readonly ISectorManager _sectorManager;

        public OpenContainerCommand(ISectorManager sectorManager,
            IPlayerState playerState) :
            base(playerState)
        {
            _sectorManager = sectorManager;
        }

        public override bool CanExecute()
        {
            var map = _sectorManager.CurrentSector.Map;

            var currentNode = _playerState.ActiveActor.Actor.Node;

            var targetContainerViewModel = GetSelectedNodeViewModel();
            if (targetContainerViewModel == null)
            {
                return false;
            }

            var targetNode = targetContainerViewModel.Container.Node;

            var canExecute = MapHelper.CheckNodeAvailability(map, currentNode, targetNode);

            //TODO добавить проверки:
            // 1. Можно ли открыть контейнер.
            // 2. Способен ли актёр открывать контейнеры.
            // 3. Находится ли контейнера в зоне видимости. Проверяется.

            return canExecute;
        }

        protected override void ExecuteTacticCommand()
        {
            var openMethod = new HandOpenContainerMethod();

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

            var intetion = new Intention<OpenContainerTask>(a => new OpenContainerTask(a, container, openMethod));
            _playerState.TaskSource.Intent(intetion);
        }

        private IContainerViewModel GetSelectedNodeViewModel()
        {
            return _playerState.HoverViewModel as IContainerViewModel;
        }
    }
}