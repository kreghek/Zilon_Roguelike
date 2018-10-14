using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public static class MapExtensions
    {
        public static void RemoveEdge(this IMap map, int offsetX1, int offsetY1, int offsetX2, int offsetY2)
        {
            var edge = GetEdge(map, offsetX1, offsetY1, offsetX2, offsetY2);
            map.Edges.Remove(edge);
        }

        private static IEdge GetEdge(IMap map, int offsetX1, int offsetY1, int offsetX2, int offsetY2)
        {
            var foundFromStart = from edge in map.Edges
                                 from node in edge.Nodes
                                 let hexNode = (HexNode)node
                                 where hexNode.OffsetX == offsetX1 && hexNode.OffsetY == offsetY1
                                 select edge;

            var foundToEnd = from edge in foundFromStart
                             from node in edge.Nodes
                             let hexNode = (HexNode)node
                             where hexNode.OffsetX == offsetX2 && hexNode.OffsetY == offsetY2
                             select edge;

            return foundToEnd.SingleOrDefault();
        }

    }
}
