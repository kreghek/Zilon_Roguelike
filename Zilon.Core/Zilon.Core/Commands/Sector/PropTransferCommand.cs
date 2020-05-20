using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на трансфер предметов между хранилищами.
    /// </summary>
    public class PropTransferCommand : SpecialActorCommandBase
    {
        [ExcludeFromCodeCoverage]
        public PropTransferCommand(
            ISectorManager sectorManager,
            ISectorUiState playerState) :
            base(sectorManager, playerState)
        {
        }

        public PropTransferMachine TransferMachine { get; set; }

        public override bool CanExecute()
        {
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            var inventoryTransfer = new PropTransfer(TransferMachine.Inventory.PropStore,
                TransferMachine.Inventory.PropAdded,
                TransferMachine.Inventory.PropRemoved);

            var containerTransfer = new PropTransfer(TransferMachine.Container.PropStore,
                TransferMachine.Container.PropAdded,
                TransferMachine.Container.PropRemoved);

            var intention = new Intention<TransferPropsTask>(a => new TransferPropsTask(a, new[] { inventoryTransfer, containerTransfer }));
            PlayerState.TaskSource.Intent(intention);
        }
    }
}