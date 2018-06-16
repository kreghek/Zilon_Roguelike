using System.Collections.Generic;
using Zilon.Core.Math;
using Zilon.Core.Services.CombatMap;
using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Services.MapGenerators
{
    public class GridMapGenerator : IMapGenerator
    {
        public void CreateMap(ICombatMap map)
        {
            var nodes = new List<MapNode>();

            var nodeIdCounter = 1;
            const int mapSize = 10;
            for (var row = 0; row < mapSize; row++)
            {
                var rowOffset = row % 2 == 0 ? 0 : 0.5f;
                for (var col = 0; col < mapSize; col++)
                {
                    var node = new MapNode
                    {
                        Id = nodeIdCounter++,
                        Position = new Vector2(col + rowOffset, row * 3f/4),
                        Coordinates =  new Vector2(col, row)
                    };

                    nodes.Add(node);
                }
            }

            map.Nodes = nodes;
        }
    }
}
