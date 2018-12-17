using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public class RoomGeneratorRandomSource : IRoomGeneratorRandomSource
    {
        private const int MaxProbably = 100;
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

        public int RollRoomPositionIndex(int maxPosition)
        {
            return _dice.Roll(0, maxPosition - 1);
        }

        public Size RollRoomSize(int maxSize)
        {
            var rollWidth = _dice.Roll(maxSize);
            var rollHeight = _dice.Roll(maxSize);

            return new Size(rollWidth, rollHeight);
        }
    }
}
