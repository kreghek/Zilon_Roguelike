using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public class FixCompactRoomGeneratorRandomSource : IRoomGeneratorRandomSource
    {
        private readonly List<Tuple<OffsetCoords, OffsetCoords>> _connections;

        public FixCompactRoomGeneratorRandomSource()
        {
            _connections = new List<Tuple<OffsetCoords, OffsetCoords>>(20);

            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 5; x++)
                {

                    if (x == 0)
                    {
                        _connections.Add(new Tuple<OffsetCoords, OffsetCoords>(
                            new OffsetCoords(x, y),
                            new OffsetCoords(x, y - 1))
                            );
                    }
                    else
                    {
                        _connections.Add(new Tuple<OffsetCoords, OffsetCoords>(
                            new OffsetCoords(x, y),
                            new OffsetCoords(x - 1, y))
                            );
                    }
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

        public IDictionary<Room, Room[]> RollRoomNet(IEnumerable<Room> rooms, int maxNeighbors)
        {
            var result = new Dictionary<Room, Room[]>();

            foreach (var currentRoom in rooms)
            {
                var currentConnection = _connections.Single(x =>
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

        public Size RollRoomSize(int maxSize)
        {
            return new Size(2, 2);
        }
    }
}
