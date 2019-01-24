using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public class FixLargeRoomGeneratorRandomSource : IRoomGeneratorRandomSource
    {
        private readonly List<Tuple<OffsetCoords, OffsetCoords>> _connections;

        public FixLargeRoomGeneratorRandomSource()
        {
            // 20 комнат - это 6х6 матрица
            _connections = new List<Tuple<OffsetCoords, OffsetCoords>>(20);

            // Все комнаты пересекаются через всё доступное пространство.
            // Каждая комната из ряда пересекается с зеркальной комнатой.
            // Т.е. левая верхняя с правой нижней.

            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    var current = new OffsetCoords(x, y);
                    var mirror = new OffsetCoords(5 - x, 5 - y);

                    _connections.Add(new Tuple<OffsetCoords, OffsetCoords>(
                        current,
                        mirror)
                        );
                }
            }
        }

        [NotNull, ItemNotNull]
        public Room[] RollConnectedRooms(Room currentRoom, int maxNeighbors, IList<Room> availableRooms)
        {
            if (!availableRooms.Any())
            {
                return new Room[0];
            }

            var currentConnection = _connections.Single(x =>
                        x.Item1.X == currentRoom.PositionX &&
                        x.Item1.Y == currentRoom.PositionY);

            var connectedRoom = availableRooms.Single(x =>
                x.PositionX == currentConnection.Item2.X &&
                x.PositionY == currentConnection.Item2.Y);

            return new[] { connectedRoom };
        }

        public IEnumerable<OffsetCoords> RollRoomMatrixPositions(int roomGridSize, int roomCount)
        {
            var result = new List<OffsetCoords>(20);

            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    var current = new OffsetCoords(x, y);
                    var mirror = new OffsetCoords(5 - x, 5 - y);

                    result.Add(current);
                    result.Add(mirror);
                }
            }

            return result;
        }

        public IDictionary<Room, Room[]> RollRoomNet(IEnumerable<Room> rooms, int maxNeighbors)
        {
            var result = new Dictionary<Room, Room[]>();

            foreach (var currentRoom in rooms)
            {
                var currentConnection = _connections.SingleOrDefault(x =>
                        x.Item1.X == currentRoom.PositionX &&
                        x.Item1.Y == currentRoom.PositionY);

                if (currentConnection == null)
                {
                    continue;
                }

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

        public Size RollRoomSize(int maxSize)
        {
            return new Size(maxSize, maxSize);
        }
    }
}
