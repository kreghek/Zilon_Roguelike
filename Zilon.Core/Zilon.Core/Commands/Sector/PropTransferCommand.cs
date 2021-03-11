using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на трансфер предметов между хранилищами.
    /// </summary>
    public class PropTransferCommand : SpecialActorCommandBase
    {
        private readonly IPlayer _player;

        [ExcludeFromCodeCoverage]
        public PropTransferCommand(
            IPlayer player,
            ISectorUiState playerState) :
            base(playerState)
        {
            _player = player;
        }

        public PropTransferMachine? TransferMachine { get; set; }

        public override bool CanExecute()
        {
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            if (TransferMachine is null)
            {
                throw new InvalidOperationException();
            }

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var inventoryTransfer = new PropTransfer(TransferMachine.Inventory.PropStore,
                TransferMachine.Inventory.PropAdded,
                TransferMachine.Inventory.PropRemoved);

            var containerTransfer = new PropTransfer(TransferMachine.Container.PropStore,
                TransferMachine.Container.PropAdded,
                TransferMachine.Container.PropRemoved);
            
            var taskContext = new ActorTaskContext(sector);

            var intention = new Intention<TransferPropsTask>(actor =>
                new TransferPropsTask(actor, taskContext, new[] { inventoryTransfer, containerTransfer }));
            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var taskSource = PlayerState.TaskSource;
            if (taskSource is null)
            {
                throw new InvalidOperationException();
            }

            taskSource.Intent(intention, actor);
        }
    }
}