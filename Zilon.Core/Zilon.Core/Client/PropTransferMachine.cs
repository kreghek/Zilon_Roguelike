using Zilon.Core.Persons;

namespace Zilon.Core.Client
{
    public class PropTransferMachine
    {
        public PropTransferMachine(IPropStore inventory, IPropStore container)
        {
            //Floor = new 
        }

        public IPropStore Inventory { get; }
        public IPropStore Container { get; }
        public IPropStore Floor { get; }
    }
}
