namespace Zilon.Core.Tactics.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Zilon.Core.Common;

    public class HexNodeHelper
    {
        private const float LOCATION_DISTANCE = 20;

        public static HexNode[] GetNeighbors(HexNode currentNode, IEnumerable<HexNode> nodes)
        {
            var currentCubeCoords = currentNode.CubeCoords;

            var directions = HexHelper.GetOffsetClockwise();

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


        /// <summary>
        /// Ищет ближайший узел карты в сетке шестиугольников без учёта ребёр.
        /// </summary>
        /// <param name="node"> Опорный узел. </param>
        /// <param name="targets"> Целевые узлы, среди которых будет поиск. </param>
        /// <returns> Возвращает ближайший узел карты. </returns>
        public static HexNode GetNearbyCoordinates(HexNode node, IEnumerable<HexNode> targets)
        {
            if (!targets.Any())
            {
                throw new ArgumentException("Набор целевых узлов не может быть пустым.", nameof(targets));
            }

            var minDistance = -1;
            HexNode nearbyNode = null;
            foreach (var target in targets)
            {
                if (target == node)
                {
                    return node;
                }

                var distance = target.CubeCoords.DistanceTo(node.CubeCoords);
                if (distance == 1)
                {
                    return target;
                }

                if (minDistance == -1 || distance < minDistance)
                {
                    minDistance = distance;
                    nearbyNode = target;
                }
            }

            return nearbyNode;
        }

        public static bool EqualCoordinates(HexNode hexNode1, HexNode hexNode2)
        {
            return hexNode1.OffsetX == hexNode2.OffsetX &&
                hexNode1.OffsetY == hexNode2.OffsetY;
        }
    }
}