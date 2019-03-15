using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Команда на отображение модального окна для отображения контента контейнера.
    /// </summary>
    [PublicAPI]
    public class ShowContainerModalCommand : ShowModalCommandBase
    {
        private readonly ISectorUiState _playerState;

        [ExcludeFromCodeCoverage]
        public ShowContainerModalCommand(ISectorModalManager modalManager, ISectorUiState playerState) :
            base(modalManager)
        {
            _playerState = playerState;
        }
        
        public override void Execute()
        {
            var inventory = _playerState.ActiveActor.Actor.Person.Inventory;
            var targetContainerViewModel = (IContainerViewModel)_playerState.HoverViewModel;
            var container = targetContainerViewModel.Container;
            var containerContent = container.Content;
            var transferMachine = new PropTransferMachine(inventory, containerContent);
            
            ModalManager.ShowContainerModal(transferMachine);
        }

        public override bool CanExecute()
        {
            var inventory = _playerState.ActiveActor.Actor.Person.Inventory;

            var targetContainerViewModel = _playerState.HoverViewModel as IContainerViewModel;
            var container = targetContainerViewModel?.Container;
            var containerContent = container?.Content;

            return inventory != null && containerContent != null;
        }
    }
}