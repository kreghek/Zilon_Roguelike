using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="map"> Карта, узлы которой сканируются. </param>
        /// <param name="availableNodes"> Доступные узлы. Использовать узлы региона.
        /// Возможно отфильтрованные от уже занятых узлов. </param>
        /// <returns> Возвращает узел, который не закрывает проход в регион карты. </returns>
        public static IMapNode FindFreeNode(IMap map, IEnumerable<IMapNode> availableNodes)
        {
            var checkedNodes = new List<IMapNode>();

            var absNodeIndex = availableNodes.Count();
            var node = availableNodes.ElementAt(absNodeIndex / 2);
            while (checkedNodes.Count < availableNodes.Count())
            {
                
                checkedNodes.Add(node);

                var neigbours = map.GetNext(node);
                var corridorNodes = from neighbor in neigbours
                                    where !availableNodes.Contains(neighbor)
                                    select neighbor;

                if (!corridorNodes.Any())
                {
                    // Найден узел, не перекрывающий выход из региона
                    return node;
                }
                else
                {
                    node = neigbours.Where(x => !checkedNodes.Contains(x)).FirstOrDefault();

                    if (node == null)
                    {
                        throw new InvalidOperationException("В комнате не удалось найти узел для размещения сундука.");
                    }
                }
            }

            throw new InvalidOperationException("В комнате не удалось найти узел для размещения сундука.");
        }
    }
}
