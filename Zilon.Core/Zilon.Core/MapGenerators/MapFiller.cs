using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public static class MapFiller
    {
        public static void FillSquareMap(IMap map, int mapSize)
        {
            CreateNodes(map, mapSize);
            CreateEdges(map);
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

        private static void CreateNodes(IMap map, int mapSize)
        {
            var nodeIdCounter = 1;
            for (var row = 0; row < mapSize; row++)
            {
                for (var col = 0; col < mapSize; col++)
                {
                    var node = new HexNode(col, row)
                    {
                        Id = nodeIdCounter++
                    };

                    map.Nodes.Add(node);
                }
            }
        }
    }
}
