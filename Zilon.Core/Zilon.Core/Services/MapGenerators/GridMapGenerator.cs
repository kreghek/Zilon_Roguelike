using System.Collections.Generic;

using Zilon.Core.Services.CombatMap;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Services.MapGenerators
{
    public class GridMapGenerator : IMapGenerator
    {
        public void CreateMap(IHexMap map)
        {
            var nodes = new List<HexNode>();

            var nodeIdCounter = 1;
            const int mapSize = 10;
            for (var row = 0; row < mapSize; row++)
            {
                for (var col = 0; col < mapSize; col++)
                {
                    var node = new HexNode(col, row)
                    {
                        Id = nodeIdCounter++
                    };

                    nodes.Add(node);
                }
            }

            map.Nodes = nodes;
        }
    }
}
