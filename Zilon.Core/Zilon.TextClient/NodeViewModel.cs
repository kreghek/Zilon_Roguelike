using Zilon.Core.Client;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.TextClient
{
    internal class NodeViewModel : IMapNodeViewModel
    {
        public object Item => Node;
        public HexNode Node { get; set; }
    }
}