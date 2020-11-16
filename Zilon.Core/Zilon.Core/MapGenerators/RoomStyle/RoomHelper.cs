using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Вспомогательный класс для генератора комнат.
    /// </summary>
    public static class RoomHelper
    {
        public static void AddAllNeighborToMap(
            IMap map,
            HashSet<string> edgeHash,
            Room room,
            HexNode node)
        {
            if (room is null)
            {
                throw new System.ArgumentNullException(nameof(room));
            }

            var neighbors = HexNodeHelper.GetSpatialNeighbors(node, room.Nodes);

            foreach (var neighbor in neighbors)
            {
                var isExists = IsExistsEdge(edgeHash, node, neighbor);

                if (!isExists)
                {
                    AddEdgeToMap(map, edgeHash, node, neighbor);
                }
            }
        }

        /// <summary>
        /// Создаёт на карте ребро, соединяющее два узла этой карты.
        /// </summary>
        /// <param name="targetMap"> Целевая карта, для которой нужно создать ребро. </param>
        /// <param name="edgeHash"> Хеш ребер карты. </param>
        /// <param name="node"> Исходное ребро карты. </param>
        /// <param name="neighbor"> Соседнее ребро карты, с которым будет соединено исходное. </param>
        public static void AddEdgeToMap(
            IMap targetMap,
            HashSet<string> edgeHash,
            HexNode node,
            HexNode neighbor)
        {
            if (targetMap is null)
            {
                throw new System.ArgumentNullException(nameof(targetMap));
            }

            if (edgeHash is null)
            {
                throw new System.ArgumentNullException(nameof(edgeHash));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (neighbor is null)
            {
                throw new System.ArgumentNullException(nameof(neighbor));
            }

            var hashKey1 = $"{node}-{neighbor}";
            edgeHash.Add(hashKey1);

            targetMap.AddEdge(node, neighbor);
        }

        /// <summary>
        /// Рассчитывает размер ячейки, в которую можно разместить любую комнату.
        /// </summary>
        /// <param name="rooms"> Сгенерированные комнаты. </param>
        /// <returns> Размер ячейки комнаты. </returns>
        public static Size CalcCellSize(IEnumerable<Room> rooms)
        {
            var maxWidth = rooms.Max(x => x.Width);
            var maxHeight = rooms.Max(x => x.Height);

            return new Size(maxWidth, maxHeight);
        }

        public static HexNode CreateCorridorNode(
            IMap map,
            HashSet<string> edgeHash,
            HexNode currentNode,
            int currentX,
            int currentY)
        {
            if (map is null)
            {
                throw new System.ArgumentNullException(nameof(map));
            }

            var node = map.Nodes.OfType<HexNode>()
                .SingleOrDefault(x => (x.OffsetCoords.X == currentX) && (x.OffsetCoords.Y == currentY));

            if (node == null)
            {
                node = new HexNode(currentX, currentY);
                map.AddNode(node);
            }

            var isExists = IsExistsEdge(edgeHash, node, currentNode);

            if (!isExists)
            {
                AddEdgeToMap(map, edgeHash, currentNode, node);
            }

            return node;
        }

        /// <summary>
        /// Возвращает ребро, соединяющее указанные узлы.
        /// </summary>
        /// <param name="edgeHash"> Хеш ребер карты. </param>
        /// <param name="node"> Искомый узел. </param>
        /// <param name="neighbor"> Узел, с которым соединён искомый. </param>
        /// <returns> Ребро или null, если такого ребра нет на карте. </returns>
        public static bool IsExistsEdge(HashSet<string> edgeHash, HexNode node, HexNode neighbor)
        {
            if (edgeHash is null)
            {
                throw new System.ArgumentNullException(nameof(edgeHash));
            }

            if (node is null)
            {
                throw new System.ArgumentNullException(nameof(node));
            }

            if (neighbor is null)
            {
                throw new System.ArgumentNullException(nameof(neighbor));
            }

            var hashKey1 = $"{node}-{neighbor}";
            if (edgeHash.Contains(hashKey1))
            {
                return true;
            }

            var hashKey2 = $"{neighbor}-{node}";
            return edgeHash.Contains(hashKey2);
        }
    }
}