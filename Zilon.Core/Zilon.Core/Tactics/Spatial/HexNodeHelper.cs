namespace Zilon.Core.Tactics.Spatial
{
    using System.Collections.Generic;
    using System.Linq;

    public class HexNodeHelper
    {
        private const float LOCATION_DISTANCE = 20;

        public static HexNode[] GetSquadNodes(HexNode teamNode, IEnumerable<HexNode> nodes)
        {
            var result = new List<HexNode>();

            //foreach (var node in nodes)
            //{
            //    var xComponent = node.OffsetX - teamNode.OffsetX;
            //    var yComponent = node.Position.Y - teamNode.Position.Y;
            //    var distance = System.Math.Sqrt(System.Math.Pow(xComponent, 2) + System.Math.Pow(yComponent, 2));

            //    if (distance <= LOCATION_DISTANCE * 1.6f)
            //    {
            //        result.Add(node);
            //    }
            //}

            return result.ToArray();
        }

        public static HexNode[] GetNeighbors(HexNode currentNode, IEnumerable<HexNode> nodes)
        {
            var currentCubeCoords = currentNode.CubeCoords;

            var directions = new[] {
               new CubeCoords(+1, -1, 0), new CubeCoords(+1, 0, -1), new CubeCoords(0, +1, -1),
               new CubeCoords(-1, +1, 0), new CubeCoords(-1, 0, +1), new CubeCoords(0, -1, +1)
            };

            var neiberhoodPositions = new List<CubeCoords>();
            foreach (var dir in directions)
            {
                var pos = new CubeCoords(dir.X + currentCubeCoords.X,
                    dir.Y + currentCubeCoords.Y,
                    dir.Z + currentCubeCoords.Z);

                neiberhoodPositions.Add(pos);
            }

            var list = new List<HexNode>();
            foreach (var node in nodes)
            {
                var cubeCoords = node.CubeCoords;

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