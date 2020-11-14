using Zilon.Core.Client;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.TextClient
{
    class NodeViewModel : IMapNodeViewModel
    {
        public HexNode Node { get; set; }
        public object Item { get => Node; }
    }
}