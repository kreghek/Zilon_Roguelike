using Zilon.Core.Client;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на трансфер предметов между хранилищами.
    /// </summary>
    public class PropTransferCommand : SpecialActorCommandBase
    {
        private readonly PropTransferMachine _transferMachine;

        public PropTransferCommand(ISectorManager sectorManager,
            IPlayerState playerState,
            PropTransferMachine transferMachine) :
            base(sectorManager, playerState)
        {
            _transferMachine = transferMachine;
        }

        public override bool CanExecute()
        {
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            var inventoryTransfer = new PropTransfer(_transferMachine.Inventory.PropStore,
                _transferMachine.Inventory.PropAdded,
                _transferMachine.Inventory.PropRemoved);

            var containerTransfer = new PropTransfer(_transferMachine.Container.PropStore,
                _transferMachine.Container.PropAdded,
                _transferMachine.Container.PropRemoved);

            //_playerState.TaskSource.IntentTransferProps(new[] {inventoryTransfer, containerTransfer});
            _playerState.TaskSource.Intent(null);
        }
    }
}