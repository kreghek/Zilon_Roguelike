using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public class PropContainer : IPropContainer
    {
        public IMapNode Node { get; private set; }

        public PropContainer(IMapNode node)
        {
            Node = node;
        }
    }
}
