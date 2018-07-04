using FluentAssertions;

using NUnit.Framework;

namespace Zilon.Core.Tactics.Generation.Tests
{
    [TestFixture()]
    public class GenerationHelperTests
    {
        [Test()]
        public void GetFreeCellTest()
        {
            // ARRANGE
            var rooms = new Room[10, 10];
            rooms[3, 3] = new Room();
            var testedCoords = new OffsetCoords(3, 3);



            // ACT
            var factCoords = GenerationHelper.GetFreeCell(rooms, testedCoords);


            // ASSERT
            factCoords.Should().NotBeNull();
            var isEquals = factCoords.X == testedCoords.X && factCoords.Y == testedCoords.Y;
            isEquals.Should().BeFalse();
        }
    }
}