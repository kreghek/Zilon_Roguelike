using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators
{
    public class SectorGeneratorRandomSource : ISectorGeneratorRandomSource
    {
        private const int MaxProbably = 100;
        private readonly IDice _dice;

        public SectorGeneratorRandomSource(IDice dice)
        {
            _dice = dice;
        }

        public Room[] RollConnectedRooms(Room room, int maxNeighbors, int p, IList<Room> rooms)
        {
            var selectedRooms = new HashSet<Room>();
            for (var i = 0; i < maxNeighbors; i++)
            {
                var hasNeighborRoll = _dice.Roll(MaxProbably);
                var hasNeighborSuccess = MaxProbably - p;
                if (hasNeighborRoll < hasNeighborSuccess)
                {
                    // Провал броска на выбор очередного соседа.
                    continue;
                }

                var rolledRoomIndex = _dice.Roll(rooms.Count) - 1;
                selectedRooms.Add(rooms[rolledRoomIndex]);
            }

            return selectedRooms.ToArray();
        }

        public OffsetCoords RollRoomPosition(int maxPosition)
        {
            var rollX = _dice.Roll(maxPosition);
            var rollY = _dice.Roll(maxPosition);

            return new OffsetCoords(rollX, rollY);
        }

        public Size RollRoomSize(int maxSize)
        {
            var rollWidth = _dice.Roll(maxSize);
            var rollHeight = _dice.Roll(maxSize);

            return new Size(rollWidth, rollHeight);
        }
    }
}
