using Assets.Zilon.Scripts.Models.SectorScene;
using Assets.Zilon.Scripts.Services;
using UnityEngine;

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
            Debug.Log($"Открыт контейнер {_playerState.SelectedContainer}.");
            var container = _playerState.SelectedContainer.Container;
            var props = container.Props;
            ModalManager.ShowContainerModal(props);
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}