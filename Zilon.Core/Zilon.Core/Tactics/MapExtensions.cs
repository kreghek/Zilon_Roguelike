using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public static class MapExtensions
    {
        public static void RemoveEdge(this IMap map, int offsetX1, int offsetY1, int offsetX2, int offsetY2)
        {
            var foundFromStart = from node in map.Nodes
                                 let hexNode = (HexNode)node
                                 where hexNode.OffsetX == offsetX1 && hexNode.OffsetY == offsetY1
                                 select node;

            var foundToEnd = from node in foundFromStart
                             from neighborNode in map.GetNext(node)
                             let hexNode = (HexNode)neighborNode
                             where hexNode.OffsetX == offsetX2 && hexNode.OffsetY == offsetY2
                             select new
                             {
                                 start = node,
                                 end = neighborNode
                             };

            var foundNode = foundToEnd.Single();

            map.RemoveEdge(foundNode.start, foundNode.end);
        }
    }
}
