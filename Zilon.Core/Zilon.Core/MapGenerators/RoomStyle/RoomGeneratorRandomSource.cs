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
        public Room[] RollConnectedRooms(Room room, int maxNeighbors, IList<Room> rooms)
        {
            var availableRooms = new List<Room>(rooms);
            var selectedRooms = new HashSet<Room>();
            var neighborCount = _dice.Roll(1, maxNeighbors);
            for (var i = 0; i < neighborCount; i++)
            {
                var rolledRoomIndex = _dice.Roll(0, availableRooms.Count - 1);
                var selectedRoom = availableRooms[rolledRoomIndex];
                selectedRooms.Add(selectedRoom);
                availableRooms.Remove(selectedRoom);

                if (!availableRooms.Any())
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

        public Size RollRoomSize(int maxSize)
        {
            var rollWidth = _dice.Roll(maxSize);
            var rollHeight = _dice.Roll(maxSize);

            return new Size(rollWidth, rollHeight);
        }
    }
}
