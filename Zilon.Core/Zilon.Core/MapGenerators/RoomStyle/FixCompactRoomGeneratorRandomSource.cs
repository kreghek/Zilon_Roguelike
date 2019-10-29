using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public class FixCompactRoomGeneratorRandomSource : FixRoomGeneratorRandomSourceBase, IRoomGeneratorRandomSource
    {
        public FixCompactRoomGeneratorRandomSource()
        {
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    if (x == 0)
                    {
                        Connections.Add(new Tuple<OffsetCoords, OffsetCoords>(
                            new OffsetCoords(x, y),
                            new OffsetCoords(x, y - 1))
                            );
                    }
                    else
                    {
                        Connections.Add(new Tuple<OffsetCoords, OffsetCoords>(
                            new OffsetCoords(x, y),
                            new OffsetCoords(x - 1, y))
                            );
                    }
                }
            }
        }

        /// <summary>
        /// Выбрасывает случайный набор уникальных координат в матрице комнат указаной длины.
        /// </summary>
        /// <param name="roomGridSize">Размер матрицы комнат.</param>
        /// <param name="roomCount">Количество комнат в секторе.</param>
        /// <returns>
        /// Возвращает массив координат из матрицы комнат.
        /// </returns>
        public IEnumerable<OffsetCoords> RollRoomMatrixPositions(int roomGridSize, int roomCount)
        {
            var result = new OffsetCoords[20];

            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    result[x + y * 5] = new OffsetCoords(x, y);
                }
            }

            return result;
        }

        /// <summary>
        /// Возвращает матрицу смежности между комнатами (сеть комнат).
        /// </summary>
        /// <param name="rooms">Всё комнаты, которые должны быть соединены в сеть.</param>
        /// <param name="maxNeighbors">Максимальное количество соседей у комнаты.</param>
        /// <returns>
        /// Возвращает словарь, представляющий собой матрицу смежности комнат.
        /// Минимальное число соседей - 1. Максимальное - не превышает указанное в аргументе значение.
        /// </returns>
        public IDictionary<Room, Room[]> RollRoomNet(IEnumerable<Room> rooms, int maxNeighbors)
        {
            var result = new Dictionary<Room, Room[]>();

            foreach (var currentRoom in rooms)
            {
                var currentConnection = Connections.Single(x =>
                        x.Item1.X == currentRoom.PositionX &&
                        x.Item1.Y == currentRoom.PositionY);

                var connectedRoom = rooms.SingleOrDefault(x =>
                    x.PositionX == currentConnection.Item2.X &&
                    x.PositionY == currentConnection.Item2.Y);

                if (connectedRoom != null)
                {
                    result.Add(currentRoom, new[] { connectedRoom });
                }
            }

            return result;
        }

        /// <summary>
        /// Выбрасывает случайный размер комнаты.
        /// </summary>
        /// <param name="minSize">Минимальный размер комнаты.</param>
        /// <param name="maxSize">Максимальный размер комнаты.</param>
        /// <returns>
        /// Возвращает размер с произвольными шириной и высотой в диапазоне (minSize, maxSize).
        /// </returns>
        /// <remarks>
        /// Источник рандома возвращает случайный размер комнаты в указанном диапазоне.
        /// </remarks>
        protected override Size RollRoomSize(int minSize, int maxSize)
        {
            return new Size(minSize, minSize);
        }
    }
}
