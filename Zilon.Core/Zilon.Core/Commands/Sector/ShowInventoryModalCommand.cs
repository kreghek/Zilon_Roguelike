using Zilon.Core.Client;
using Zilon.Core.Client.Windows;

namespace Zilon.Core.Commands
{
    /// <summary>
    ///     Команда на отображение модала для отображения контента контейнера.
    /// </summary>
    public class ShowInventoryModalCommand : ShowModalCommandBase
    {
        private readonly ISectorUiState _playerState;

        [PublicAPI]
        [ExcludeFromCodeCoverage]
        public ShowInventoryModalCommand(ISectorModalManager modalManager, ISectorUiState playerState) :
            base(modalManager)
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