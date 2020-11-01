using Zilon.Core.Client;
using Zilon.Core.Client.Windows;
using Zilon.Core.PersonModules;
using Zilon.Core.Props;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    ///     Команда на отображение модального окна для отображения контента контейнера.
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
            IInventoryModule inventory = _playerState.ActiveActor.Actor.Person.GetModule<IInventoryModule>();
            IContainerViewModel targetContainerViewModel = (IContainerViewModel)_playerState.HoverViewModel;
            IStaticObject container = targetContainerViewModel.StaticObject;
            IPropStore containerContent = container.GetModule<IPropContainer>().Content;
            PropTransferMachine transferMachine = new PropTransferMachine(inventory, containerContent);

            ModalManager.ShowContainerModal(transferMachine);
        }

        public override bool CanExecute()
        {
            IInventoryModule inventory = _playerState.ActiveActor.Actor.Person.GetModule<IInventoryModule>();

            IContainerViewModel targetContainerViewModel = _playerState.HoverViewModel as IContainerViewModel;
            IStaticObject container = targetContainerViewModel?.StaticObject;
            IPropStore containerContent = container?.GetModule<IPropContainer>().Content;

            return inventory != null && containerContent != null;
        }
    }
}