using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Tests.Common.TestCases;

namespace Zilon.Core.Tests.Common
{
    [TestFixture]
    public class CubeCoordsHelperTests
    {
        [Test]
        [TestCaseSource(typeof(CubeCoordsHelperTestCases), nameof(CubeCoordsHelperTestCases.TestCases))]
        [Ignore("Не достоверный. См коммент !!!")]
        public void CubeDrawLineTest(int sOffsetX, int sOffsetY, int offsetX, int offsetY)
        {
            // ARRANGE

            var startCubeCoords = HexHelper.ConvertToCube(sOffsetX, sOffsetY);
            var finishCubCoords = HexHelper.ConvertToCube(offsetX, offsetY);



            // ACT
            var line = CubeCoordsHelper.CubeDrawLine(startCubeCoords, finishCubCoords);



            // ASSERT
            foreach (var coord in line)
            {
                var sameCoords = line.Where(x => x == coord);
                sameCoords.Count().Should().Be(1);

                // Проверяем, что у каждой точки линии есть соседи,
                // т.е. нет изолированных разорванных точк.
                // !!! Похоже, то часть проверки, которую не дописал. var neibourOffsets = HexHelper.GetOffsetClockwise();
                var neighborCoords = line.Where(x => x == coord);

                var hasNeighbor = neighborCoords.Any();

                hasNeighbor.Should().Be(true);
            }
        }
    }
}