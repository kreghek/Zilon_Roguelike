using Zilon.Core.Client;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot
{
    public sealed class NodeViewModel : IMapNodeViewModel
    {
        public HexNode Node { get; set; }
    }
}
