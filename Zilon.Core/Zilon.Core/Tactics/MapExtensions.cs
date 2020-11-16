using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public static class MapExtensions
    {
        public static void RemoveEdge(
            this IMap map,
            int offsetX1,
            int offsetY1,
            int offsetX2,
            int offsetY2)
        {
            if (map is null)
            {
                throw new System.ArgumentNullException(nameof(map));
            }

            var foundFromStart = from node in map.Nodes
                let hexNode = (HexNode)node
                where (hexNode.OffsetCoords.X == offsetX1) && (hexNode.OffsetCoords.Y == offsetY1)
                select node;

            var foundToEnd = from node in foundFromStart
                from neighborNode in map.GetNext(node)
                let hexNode = (HexNode)neighborNode
                where (hexNode.OffsetCoords.X == offsetX2) && (hexNode.OffsetCoords.Y == offsetY2)
                select new
                {
                    start = node, end = neighborNode
                };

            var foundNode = foundToEnd.SingleOrDefault();

            if (foundNode != null)
            {
                map.RemoveEdge(foundNode.start, foundNode.end);
            }
        }
    }
}