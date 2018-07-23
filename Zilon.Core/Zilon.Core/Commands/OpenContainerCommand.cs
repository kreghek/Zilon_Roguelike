using Zilon.Core.Client;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    //TODO Добавить тесты
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
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