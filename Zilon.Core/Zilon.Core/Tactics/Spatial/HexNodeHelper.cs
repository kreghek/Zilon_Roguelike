using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Вспомогательный класс для работы с узлами в поле шестигранников.
    /// </summary>
    public static class HexNodeHelper
    {
        /// <summary>
        /// Возвращает географически соседние узлы. Т.е. не учитывается, соединены ли узлы рёбрами.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static HexNode[] GetSpatialNeighbors(HexNode currentNode, IEnumerable<HexNode> nodes)
        {
            if (currentNode is null)
            {
                throw new ArgumentNullException(nameof(currentNode));
            }

            if (nodes is null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            var currentCubeCoords = currentNode.CubeCoords;

            var directions = HexHelper.GetOffsetClockwise();

            var neighborCoords = new List<CubeCoords>();

            for (var i = 0; i < 6; i++)
            {
                var dir = directions[i];
                var pos = dir + currentCubeCoords;

                neighborCoords.Add(pos);
            }

            var list = new List<HexNode>();

            var nodeDictionary = nodes.ToDictionary(x => x.CubeCoords, x => x);
            for (var i = 0; i < 6; i++)
            {
                var neighborCoord = neighborCoords[i];

                if (nodeDictionary.TryGetValue(neighborCoord, out var neighborNode))
                {
                    list.Add(neighborNode);
                }
            }

            return list.ToArray();
        }
    }
}