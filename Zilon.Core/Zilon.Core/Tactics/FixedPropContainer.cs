using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public class FixedPropContainer : IPropContainer
    {
        public IMapNode Node { get; private set; }

        public IPropStore Inventory { get; }

        public FixedPropContainer(IMapNode node, IProp[] props)
        {
            Node = node;
            Inventory = new Inventory();
            foreach (var prop in props)
            {
                Inventory.Add(prop);
            }
        }
    }
}
