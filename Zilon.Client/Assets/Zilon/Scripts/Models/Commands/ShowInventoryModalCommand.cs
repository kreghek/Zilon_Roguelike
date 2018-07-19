using Assets.Zilon.Scripts.Models.SectorScene;
using Assets.Zilon.Scripts.Services;
using Zilon.Core.Client;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Команда на отображение модала для отображения контента контейнера.
    /// </summary>
    class ShowInventoryModalCommand : ShowModalCommandBase
    {
        private readonly IPlayerState _playerState;

        public ShowInventoryModalCommand(ISectorModalManager sectorManager, IPlayerState playerState) :
            base(sectorManager)
        {
            _playerState = playerState;
        }
        
        public override void Execute()
        {
            ModalManager.ShowInventoryModal(_playerState.ActiveActor.Actor);
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}