namespace Zilon.Core.Services.CombatMap
{
    using System;
    using System.Collections.Generic;

    using Zilon.Core.Math;
    using Zilon.Core.Tactics.Map;

    public class CellMapGenerator
    {
        private const float LOCATION_DISTANCE = 20;
        private const float LOCATION_OFFSET = LOCATION_DISTANCE / 4;

        private readonly Random random = new Random();

        public void CreateMap(ICombatMap map)
        {
            var nodes = new List<MapNode>();
            var teamNodes = new List<MapNode>();

            var nodeIdCounter = 1;
            const int mapSize = 10;
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var offsetX = (float) random.NextDouble() * LOCATION_OFFSET;
                    var offsetY = (float) random.NextDouble() * LOCATION_OFFSET;


                    var node = new MapNode
                    {
                        Id = nodeIdCounter++,
                        Position = new Vector2
                        {
                            X = LOCATION_DISTANCE * i + offsetX,
                            Y = LOCATION_DISTANCE * j + offsetY
                        }
                    };



                    nodes.Add(node);

                    if (i == 0 || i == mapSize - 1)
                    {
                        if (j == 0 || j == mapSize - 1)
                        {
                            teamNodes.Add(node);
                        }
                    }
                }
            }

            map.Nodes = nodes;
            map.TeamNodes = teamNodes;
        }
    }
}
