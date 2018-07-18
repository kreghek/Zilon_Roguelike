using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public class FixedPropContainer : IPropContainer
    {
        public IMapNode Node { get; private set; }

        public IPropStore Content { get; }

        public FixedPropContainer(IMapNode node, IProp[] props)
        {
            Node = node;
            Content = new Inventory();
            foreach (var prop in props)
            {
                Content.Add(prop);
            }
        }
    }
}
