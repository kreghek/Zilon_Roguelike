using Assets.Zilon.Scripts.Models.SectorScene;
using Assets.Zilon.Scripts.Services;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Команда на отображение модала для отображения контента контейнера.
    /// </summary>
    class ShowContainerModalCommand : ShowModalCommandBase
    {
        private readonly IPlayerState _playerState;

        public ShowContainerModalCommand(ISectorModalManager sectorManager, IPlayerState playerState) :
            base(sectorManager)
        {
            _playerState = playerState;
        }
        
        public override void Execute()
        {
            var container = _playerState.SelectedContainer.Container;
            var props = container.Content.Items;
            ModalManager.ShowContainerModal(props);
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}