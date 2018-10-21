using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Команда на отображение модала для отображения контента контейнера.
    /// </summary>
    public class ShowContainerModalCommand : ShowModalCommandBase
    {
        private readonly IPlayerState _playerState;

        [ExcludeFromCodeCoverage]
        public ShowContainerModalCommand(ISectorModalManager modalManager, IPlayerState playerState) :
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