using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public static class MapFiller
    {
        public static void FillSquareMap(IMap map, int mapSize,
            OptionsDelegate optionsDelegate = null)
        {
            CreateNodes(map, 0, 0, mapSize, optionsDelegate);
            CreateEdges(map);
        }

        public static void FillSquareMap(IMap map,
            int startX,
            int startY,
            int mapSize,
            OptionsDelegate optionsDelegate = null)
        {
            CreateNodes(map, startX, startY,  mapSize, optionsDelegate);
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

        private static void CreateNodes(IMap map,
            int startX,
            int startY,
            int mapSize,
            OptionsDelegate optionsDelegate)
        {
            var nodeIdCounter = 1;
            for (var row = startY; row < startY + mapSize; row++)
            {
                for (var col = startX; col < startX + mapSize; col++)
                {
                    HexNodeOptions options;
                    if (optionsDelegate != null)
                    {
                        options = optionsDelegate(col, row);
                    }
                    else
                    {
                        options = new HexNodeOptions
                        {
                            IsObstacle = false
                        };
                    }

                    var node = new HexNode(col, row, options.IsObstacle)
                    {
                        Id = nodeIdCounter++
                    };

                    map.AddNode(node);
                }
            }
        }

        public struct HexNodeOptions
        {
            public bool IsObstacle;
        }

        public delegate HexNodeOptions OptionsDelegate(int x, int y);
    }
}
