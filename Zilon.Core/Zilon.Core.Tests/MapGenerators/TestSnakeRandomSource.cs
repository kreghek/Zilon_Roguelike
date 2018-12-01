using System.Collections.Generic;

using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tests.MapGenerators
{
    public class TestSnakeRandomSource : ISectorGeneratorRandomSource
    {
        private readonly int expectedRolls;
        private int rollIndex;
        private readonly int[][] rolledConnectedRoomIndexes;
        private readonly OffsetCoords[] rolledOffsetCoords;
        private readonly Size rolledSize;

        public TestSnakeRandomSource()
        {
            expectedRolls = 10;
            rollIndex = 0;
            rolledOffsetCoords = new[] {
                new OffsetCoords(0, 0),new OffsetCoords(1, 0), new OffsetCoords(2, 0), new OffsetCoords(3, 0),
                new OffsetCoords(3, 1), new OffsetCoords(2, 1), new OffsetCoords(1, 1), new OffsetCoords(0, 1),
                new OffsetCoords(0, 2),new OffsetCoords(1, 2)
            };

            rolledSize = new Size(3, 3);

            rolledConnectedRoomIndexes = new[] {
                new[]{ 1 }, new[]{ 2 },new[]{ 3 },new[]{ 4 },
                new[]{ 5 },new[]{ 6 },new[]{ 7 },new[]{ 8 },
                new[]{ 9 }
            };
        }

        public Room[] RollConnectedRooms(Room room, int maxNeighbors, int p, IList<Room> rooms)
        {
            if (rollIndex < expectedRolls - 1)
            {
                var connectedRoomIndex = rolledConnectedRoomIndexes[rollIndex][0];
                return new[] { rooms[connectedRoomIndex] };
            }
            else
            {
                return new Room[0];
            }
        }

        public OffsetCoords RollRoomPosition(int maxPosition)
        {
            var rolled = rolledOffsetCoords[rollIndex];
            rollIndex++;
            return rolled;
        }

        public Size RollRoomSize(int maxSize)
        {
            return rolledSize;
        }
    }
}
