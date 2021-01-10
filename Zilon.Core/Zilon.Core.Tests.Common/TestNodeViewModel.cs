using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Common
{
    [ExcludeFromCodeCoverage]
    public sealed class TestNodeViewModel : IMapNodeViewModel
    {
        public HexNode Node { get; set; }
        public object Item => Node;
    }
}