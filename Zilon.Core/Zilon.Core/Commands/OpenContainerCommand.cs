using System;
using Assets.Zilon.Scripts.Models.SectorScene;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    class OpenContainerCommand : ActorCommandBase
    {
        public OpenContainerCommand(ISectorManager sectorManager,
            IPlayerState playerState) :
            base(sectorManager, playerState)
        {

        }

        public override bool CanExecute()
        {
            //TODO Здесь должна быть проверка
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            var sector = _sectorManager.CurrentSector;
            
            var openMethod = new HandOpenContainerMethod();

            var container = _playerState.SelectedContainer.Container;
            _playerState.TaskSource.IntentOpenContainer(container, openMethod);
            
            sector.Update();
        }
    }
}