using System;
using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    //TODO Добавить тесты
    /// <summary>
    /// Команда открытие контейнера.
    /// </summary>
    public class OpenContainerCommand : ActorCommandBase
    {
        public OpenContainerCommand(ISectorManager sectorManager,
            IPlayerState playerState) :
            base(sectorManager, playerState)
        {

        }

        public override bool CanExecute()
        {
            var map = _sectorManager.CurrentSector.Map;

            var currentNode = _playerState.ActiveActor.Actor.Node;

            var selectedActorViewModel = _playerState.SelectedActor;
            var targetNode = selectedActorViewModel.Actor.Node;

            var canExecute = MapHelper.CheckNodeAvailability(map, currentNode, targetNode);

            //TODO добавить проверки:
            // 1. Можно ли открыть контейнер.
            // 2. Способен ли актёр открывать контейнеры.
            // 3. Находится ли контейнера в зоне видимости.

            return canExecute;
        }

        protected override void ExecuteTacticCommand()
        {
            if (!CanExecute())
            {
                throw new InvalidOperationException("Попытка выполнить команду, которая сейчас запредена.");
            }

            var sector = _sectorManager.CurrentSector;
            
            var openMethod = new HandOpenContainerMethod();

            var container = _playerState.SelectedContainer.Container;
            _playerState.TaskSource.IntentOpenContainer(container, openMethod);
            
            sector.Update();
        }
    }
}