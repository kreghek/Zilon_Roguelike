namespace Zilon.Core.Tactics.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Zilon.Core.Math;

    public class CombatMap : ICombatMap
    {
        public List<MapNode> Nodes { get; set; }
        public List<MapNode> TeamNodes { get; set; }

        public bool IsPositionAvailableFor(MapNode targetNode, Actor actor)
        {
            return true;
        }

        public void ReleaseNode(MapNode node, Actor actor)
        {
            
        }

        public void HoldNode(MapNode node, Actor actor)
        {
            
        }

        public MapNode[] GetNeiberhoods(MapNode currentNode)
        {
            var currentCubeCoords = currentNode.GetCubeCoords();

            var directions = new[] {
               new Vector3(+1, -1, 0), new Vector3(+1, 0, -1), new Vector3(0, +1, -1),
               new Vector3(-1, +1, 0), new Vector3(-1, 0, +1), new Vector3(0, -1, +1)
            };

            var neiberhoodPositions = new List<Vector3>();
            foreach (var dir in directions)
            {
                var pos = new Vector3(dir.X + currentCubeCoords.X,
                    dir.Y + currentCubeCoords.Y,
                    dir.Z + currentCubeCoords.Z);

                neiberhoodPositions.Add(pos);
            }

            var list = new List<MapNode>();
            foreach (var node in Nodes)
            {
                var cubeCoords = node.GetCubeCoords();

                var isNeib = neiberhoodPositions.Any(x => x == cubeCoords);
                if (isNeib)
                {
                    list.Add(node);
                }
            }

            return list.ToArray();
        }
    }
}
