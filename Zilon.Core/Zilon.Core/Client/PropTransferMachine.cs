using Zilon.Core.Props;

namespace Zilon.Core.Client
{
    public sealed class PropTransferMachine
    {
        public PropTransferMachine(IPropStore inventory, IPropStore container)
        {
            Inventory = new PropTransferStore(inventory);
            Container = new PropTransferStore(container);
        }

        public PropTransferStore Inventory { get; }
        public PropTransferStore Container { get; }

        public void TransferProp(IProp prop, IPropStore sourceStore, IPropStore distStore)
        {
            sourceStore.Remove(prop);
            distStore.Add(prop);
        }
    }
}
