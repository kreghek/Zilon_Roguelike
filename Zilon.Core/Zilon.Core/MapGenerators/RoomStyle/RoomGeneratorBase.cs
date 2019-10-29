using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Базовый генератор карты с комнатами.
    /// </summary>
    public abstract class RoomGeneratorBase : IRoomGenerator
    {
        /// <summary>
        /// Соединяет комнаты коридорами.
        /// </summary>
        /// <param name="map">Карта, в рамках которой происходит генерация.</param>
        /// <param name="rooms">Существующие комнаты.</param>
        /// <param name="edgeHash">Хэш рёбер. Нужен для оптимизации при создании узлов графа карты.</param>
        public abstract void BuildRoomCorridors(IMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash);

        /// <summary>
        /// Создаёт узлы комнат на карте.
        /// </summary>
        /// <param name="map">Карта, в рамках которой происходит генерация.</param>
        /// <param name="rooms">Комнаты, для которых создаются узлы графа карты.</param>
        /// <param name="edgeHash">Хэш рёбер. Нужен для оптимизации при создании узлов графа карты.</param>
        public abstract void CreateRoomNodes(ISectorMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash);

        /// <summary>
        /// Генерация комнат.
        /// </summary>
        /// <param name="roomCount">Количество комнат, которые будут сгенерированы.</param>
        /// <param name="roomMinSize">Минимальный размер комнаты.</param>
        /// <param name="roomMaxSize">Максимальный размер комнаты.</param>
        /// <param name="availableTransitions"> Информация о переходах из данного сектора. </param>
        /// <returns>
        /// Возвращает набор созданных комнат.
        /// </returns>
        public abstract IEnumerable<Room> GenerateRoomsInGrid(int roomCount, int roomMinSize, int roomMaxSize, IEnumerable<RoomTransition> availableTransitions);

        /// <summary>
        /// Соединяет две комнаты коридором.
        /// </summary>
        /// <param name="map"> Карта, над которой идёт работа. </param>
        /// <param name="room"> Комната, которую соединяем. </param>
        /// <param name="selectedRoom"> Целевая комната для соединения. </param>
        /// <param name="edgeHash"> Хэш рёбер (для оптимизации). </param>
        protected static void ConnectRoomsWithCorridor(IMap map, Room room, Room selectedRoom, HashSet<string> edgeHash)
        {
            if (room is null)
            {
                throw new System.ArgumentNullException(nameof(room));
            }

            if (selectedRoom is null)
            {
                throw new System.ArgumentNullException(nameof(selectedRoom));
            }

            var currentNode = GetRoomCenterNode(room);
            var targetNode = GetRoomCenterNode(selectedRoom);

            var points = CubeCoordsHelper.CubeDrawLine(currentNode.CubeCoords, targetNode.CubeCoords);

            foreach (var point in points)
            {
                var offsetCoords = HexHelper.ConvertToOffset(point);

                // Это происходит, потому что если при нулевом Х для обеих комнат
                // попытаться отрисовать линию коридора, то она будет змейкой заходить за 0.
                // Нужно искать решение получше.
                offsetCoords = new OffsetCoords(offsetCoords.X < 0 ? 0 : offsetCoords.X,
                    offsetCoords.Y < 0 ? 0 : offsetCoords.Y);

                var node = RoomHelper.CreateCorridorNode(map, edgeHash, currentNode, offsetCoords.X, offsetCoords.Y);
                currentNode = node;
            }
        }

        private static HexNode GetRoomCenterNode(Room room)
        {
            return room.Nodes.First();
        }
    }
}
