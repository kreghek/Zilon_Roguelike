using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на отображение модала для отображения контента контейнера.
    /// </summary>
    public class ShowPerksModalCommand : ShowModalCommandBase
    {
        private readonly IPlayerState _playerState;

        public ShowPerksModalCommand(ISectorModalManager sectorManager, IPlayerState playerState) :
            base(sectorManager)
        {
            _playerState = playerState;
        }
        
        public override void Execute()
        {
            ModalManager.ShowPerksModal(_playerState.ActiveActor.Actor);
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}