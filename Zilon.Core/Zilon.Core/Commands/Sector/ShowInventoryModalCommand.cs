using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Client;
using Zilon.Core.Client.Windows;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на отображение модала для отображения контента контейнера.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ShowInventoryModalCommand : ShowModalCommandBase
    {
        private readonly ISectorUiState _playerState;

        [PublicAPI]
        public ShowInventoryModalCommand(ISectorModalManager modalManager, ISectorUiState playerState) :
            base(modalManager)
        {
            _playerState = playerState;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            ModalManager.ShowInventoryModal(_playerState.ActiveActor.Actor);
        }
    }
}