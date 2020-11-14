using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public static class MapFiller
    {
        public static void FillSquareMap(IMap map, int mapSize)
        {
            if (map is null)
            {
                throw new System.ArgumentNullException(nameof(map));
            }

            CreateNodes(map, 0, 0, mapSize);

            if (!(map is HexMap))
            {
                CreateEdges(map);
            }
        }

        public static void FillSquareMap(
            IMap map,
            int startX,
            int startY,
            int mapSize)
        {
            if (map is null)
            {
                throw new System.ArgumentNullException(nameof(map));
            }

            CreateNodes(map, startX, startY, mapSize);

            if (!(map is HexMap))
            {
                CreateEdges(map);
            }
        }

        private static void CreateEdges(IMap map)
        {
            foreach (var node in map.Nodes)
            {
                var currentNode = (HexNode)node;
                var nodes = map.Nodes.Cast<HexNode>().ToArray();
                var neighbors = HexNodeHelper.GetSpatialNeighbors(currentNode, nodes);

                foreach (var neighbor in neighbors)
                {
                    var existsNeibors = map.GetNext(node);
                    if (!existsNeibors.Contains(neighbor))
                    {
                        map.AddEdge(node, neighbor);
                    }
                }
            }
        }

        private static void CreateNodes(
            IMap map,
            int startX,
            int startY,
            int mapSize)
        {
            var nodeIdCounter = 1;
            for (var row = startY; row < startY + mapSize; row++)
            {
                for (var col = startX; col < startX + mapSize; col++)
                {
                    var node = new HexNode(col, row) { Id = nodeIdCounter++ };

                    map.AddNode(node);
                }
            }
        }
    }
}