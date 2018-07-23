using Assets.Zilon.Scripts.Models.SectorScene;
using Zilon.Core.Client;
using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Команда на трансфер предметов между хранилищами.
    /// </summary>
    class PropTrasferCommand : ActorCommandBase
    {
        private readonly PropTransferMachine _transferMachine;

        public PropTrasferCommand(ISectorManager sectorManager,
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
            var sector = _sectorManager.CurrentSector;

            var inventoryTransfer = new PropTransfer(_transferMachine.Inventory.PropStore,
                _transferMachine.Inventory.PropAdded,
                _transferMachine.Inventory.PropRemoved);

            var containerTransfer = new PropTransfer(_transferMachine.Container.PropStore,
                _transferMachine.Container.PropAdded,
                _transferMachine.Container.PropRemoved);

            _playerState.TaskSource.IntentTransferProps(new[] {inventoryTransfer, containerTransfer});
            sector.Update();
        }
    }
}