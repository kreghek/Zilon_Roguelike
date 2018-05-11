namespace Zilon.Core.Tactics.Map
{
    using System;
    using System.Collections.Generic;

    using Zilon.Core.Math;

    public class CombatMap
    {
        private const float locationOffset = 5;
        private const float locationDistance = 20;
        private Random random = new Random();

        public List<MapNode> Nodes { get; set; }
        public List<MapNode> TeamNodes { get; set; }

        public CombatMap()
        {
            var nodes = new List<MapNode>();
            var teamNodes = new List<MapNode>();

            var nodeIdCounter = 1;
            const int mapSize = 10;
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var offsetX = (float)random.NextDouble() * locationOffset;
                    var offsetY = (float)random.NextDouble() * locationOffset;

                    var node = new MapNode
                    {
                        Id = nodeIdCounter++,
                        Position = new Vector2
                        {
                            X = locationDistance * i + offsetX,
                            Y = locationDistance * j + offsetY
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

            Nodes = nodes;
            TeamNodes = teamNodes;
        }

        //TODO Вынести этот метод в отдельный хелпер для работы с картой и командой
        public static MapNode[] GetSquadNodes(MapNode teamNode, IEnumerable<MapNode> nodes)
        {
            var result = new List<MapNode>();

            foreach (var node in nodes)
            {
                var xComponent = node.Position.X - teamNode.Position.X;
                var yComponent = node.Position.Y - teamNode.Position.Y;
                var distance = System.Math.Sqrt(System.Math.Pow(xComponent, 2) + System.Math.Pow(yComponent, 2));

                if (distance <= locationDistance * 1.6f)
                {
                    result.Add(node);
                }
            }

            return result.ToArray();
        }

        internal bool IsPositionAvailableFor(MapNode targetNode, ActorSquad actorSquad)
        {
            return true;
        }

        internal void ReleaseNode(MapNode node, ActorSquad actorSquad)
        {
            
        }

        internal void HoldNode(MapNode node, ActorSquad actorSquad)
        {
            
        }
    }
}
