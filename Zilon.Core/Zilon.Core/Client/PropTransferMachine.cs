using Zilon.Core.Persons;

namespace Zilon.Core.Client
{
    public class PropTransferMachine
    {
        public PropTransferMachine(IPropStore inventory, IPropStore container)
        {
            Inventory = new PropTransferStore(inventory);
            Container = new PropTransferStore(container);
            Floor = new FloorPropContainer();
        }

        public PropTransferStore Inventory { get; }
        public PropTransferStore Container { get; }
        public FloorPropContainer Floor { get; }

        public void TransferProp(IProp prop, IPropStore sourceStore, IPropStore distStore)
        {
            sourceStore.Remove(prop);
            distStore.Add(prop);
        }
    }
}
