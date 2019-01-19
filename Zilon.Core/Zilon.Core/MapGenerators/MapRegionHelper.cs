using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public static class MapRegionHelper
    {
        /// <summary>
        /// Находит узел, который не блокирует выход из сектора.
        /// Найденный узел можно использовать для размещения непроходимых стационарных объектов.
        /// Таких, как сундуки или декорации.
        /// </summary>
        /// <param name="node"> Начальный узел поиска. </param>
        /// <param name="map"> Карта, узлы которой сканируются. Карта знает о доступности. </param>
        /// <param name="availableNodes"> Доступные узлы. Использовать узлы региона.
        /// Возможно, отфильтрованные от уже занятых узлов. </param>
        /// <returns> Возвращает узел, который не закрывает проход в регион карты. </returns>
        public static IMapNode FindNonBlockedNode(
            [NotNull] IMapNode node,
            [NotNull] IMap map,
            [NotNull] [ItemNotNull] IEnumerable<IMapNode> availableNodes)
        {
            var availableNodesArray = availableNodes as IMapNode[] ?? availableNodes.ToArray();
            CheckArguments(node, map, availableNodesArray);

            var openList = new List<IMapNode>(6 + 1) { node };
            var closedNodes = new List<IMapNode>();
            while (openList.Any())
            {
                node = openList[0];
                openList.RemoveAt(0);
                closedNodes.Add(node);

                var neighbors = map.GetNext(node);

                var neighborsArray = neighbors as IMapNode[] ?? neighbors.ToArray();
                var corridorNodes = from neighbor in neighborsArray
                                    where !availableNodesArray.Contains(neighbor)
                                    select neighbor;

                if (!corridorNodes.Any())
                {
                    // Найден узел, не перекрывающий выход из региона
                    return node;
                }

                var openNeighbors = neighborsArray.Where(x => !closedNodes.Contains(x) && availableNodesArray.Contains(x));
                openList.AddRange(openNeighbors);
            }

            // "В комнате не удалось найти узел для размещения сундука."
            return null;
        }

        private static void CheckArguments(IMapNode node,
            IMap map,
            IEnumerable<IMapNode> availableNodes)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            if (availableNodes == null)
            {
                throw new ArgumentNullException(nameof(availableNodes));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
        }
    }
}
