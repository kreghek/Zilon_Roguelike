using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public class FixedPropContainer : IPropContainer
    {
        public IMapNode Node { get; private set; }

        public IProp[] Props { get; }

        public FixedPropContainer(IMapNode node, IProp[] props)
        {
            Node = node;
            Props = props;
        }
    }
}
