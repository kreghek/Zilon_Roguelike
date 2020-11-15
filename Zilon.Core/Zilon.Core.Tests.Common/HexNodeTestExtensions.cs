using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Common
{
    public static class HexNodeTestExtensions
    {
        public static HexNode SelectBy(this IEnumerable<HexNode> nodes, int offsetX, int offsetY)
        {
            return nodes.SingleOrDefault(n => n.OffsetCoords.CompsEqual(offsetX, offsetY));
        }

        public static HexNode SelectByHexCoords(this IEnumerable<IGraphNode> nodes, int offsetX, int offsetY)
        {
            return nodes.Cast<HexNode>().SelectBy(offsetX, offsetY);
        }
    }
}