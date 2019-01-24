using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public class RoomGeneratorRandomSource : IRoomGeneratorRandomSource
    {
        private readonly IDice _dice;

        public RoomGeneratorRandomSource(IDice dice)
        {
            _dice = dice;
        }

        [NotNull, ItemNotNull]
        public Room[] RollConnectedRooms(Room currentRoom, int maxNeighbors, IList<Room> availableRooms)
        {
            var openRooms = new List<Room>(availableRooms);
            var selectedRooms = new HashSet<Room>();
            var neighborCount = _dice.Roll(1, maxNeighbors);
            for (var i = 0; i < neighborCount; i++)
            {
                var rolledRoomIndex = _dice.Roll(0, openRooms.Count - 1);
                var selectedRoom = openRooms[rolledRoomIndex];
                selectedRooms.Add(selectedRoom);
                openRooms.Remove(selectedRoom);

                if (!openRooms.Any())
                {
                    break;
                }
            }

            return selectedRooms.ToArray();
        }

        public IEnumerable<OffsetCoords> RollRoomMatrixPositions(int roomGridSize, int roomCount)
        {
            var roomMatrixPosList = new List<OffsetCoords>(roomGridSize * roomGridSize);
            for (var x = 0; x < roomGridSize; x++)
            {
                for (var y = 0; y < roomGridSize; y++)
                {
                    roomMatrixPosList.Add(new OffsetCoords(x, y));
                }
            }

            var rolledCoords = new List<OffsetCoords>(roomCount);
            for (var i = 0; i < roomCount; i++)
            {
                var rolledIndex = _dice.Roll(0, roomMatrixPosList.Count - 1);
                var currentRolledCoords = roomMatrixPosList[rolledIndex];
                rolledCoords.Add(currentRolledCoords);
                roomMatrixPosList.RemoveAt(rolledIndex);
            }

            return rolledCoords;
        }

        public IDictionary<Room, Room[]> RollRoomNet(IEnumerable<Room> rooms, int maxNeighbors)
        {
            var result = new Dictionary<Room, Room[]>();

            var roomsInGraph = new Dictionary<Room, int>();
            var roomsNotInGraph = new List<Room>(rooms);
            while (roomsNotInGraph.Any())
            {
                var room = roomsNotInGraph.First();
                roomsInGraph.Add(room, 0);
                roomsNotInGraph.Remove(room);
                // для каждой комнаты выбираем произвольную другую комнату
                // и проводим к ней коридор

                var availableRooms = roomsInGraph
                    .Where(x => x.Key != room && x.Value < maxNeighbors)
                    .Select(x => x.Key)
                    .ToArray();

                if (!availableRooms.Any())
                {
                    continue;
                }

                var selectedRooms = RollConnectedRooms(room,
                    maxNeighbors,
                    availableRooms);

                if (!selectedRooms.Any())
                {
                    //Значит текущая комната тупиковая
                    continue;
                }

                result.Add(room, selectedRooms);
                foreach (var selectedRoom in selectedRooms)
                {
                    roomsInGraph[selectedRoom]++;
                }
            }

            return result;
        }

        public Size RollRoomSize(int maxSize)
        {
            var rollWidth = _dice.Roll(2, maxSize);
            var rollHeight = _dice.Roll(2, maxSize);

            return new Size(rollWidth, rollHeight);
        }
    }
}
