using Zilon.Core.Client;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Common
{
    public sealed class TestNodeViewModel : IMapNodeViewModel
    {
        public HexNode Node { get; set; }
        public object Item { get => Node; }
    }
}