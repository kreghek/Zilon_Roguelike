using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Common;
using Zilon.Core.Tests.Common.TestCases;

namespace Zilon.Core.Tests.Common
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class CubeCoordsHelperTests
    {
        /// <summary>
        ///     Тест проверяет, что при произвольных входных данных строится непрерывная линия.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(CubeCoordsHelperTestCases), nameof(CubeCoordsHelperTestCases.TestCases))]
        public void CubeDrawLine_DifferentPoints_LineIsSolid(int sOffsetX, int sOffsetY, int offsetX, int offsetY)
        {
            // ARRANGE

            CubeCoords startCubeCoords = HexHelper.ConvertToCube(sOffsetX, sOffsetY);
            CubeCoords finishCubeCoords = HexHelper.ConvertToCube(offsetX, offsetY);

            // ACT
            CubeCoords[] line = CubeCoordsHelper.CubeDrawLine(startCubeCoords, finishCubeCoords);

            // ASSERT
            CubeCoords[] neibourOffsets = HexHelper.GetOffsetClockwise();

            foreach (CubeCoords coord in line)
            {
                IEnumerable<CubeCoords> sameCoords = line.Where(x => x == coord);
                sameCoords.Count().Should().Be(1);

                if (line.Count() > 1)
                {
                    // Проверяем, что у каждой точки линии есть соседи,
                    // т.е. нет изолированных разорванных точк.
                    bool hasNeighbor = false;

                    foreach (CubeCoords neibourOffset in neibourOffsets)
                    {
                        CubeCoords neighborCoord = coord + neibourOffset;
                        IEnumerable<CubeCoords> foundCoords = line.Where(x => x == neighborCoord);

                        bool hasNeighborInThisDirection = foundCoords.Any();

                        if (hasNeighborInThisDirection)
                        {
                            hasNeighbor = true;
                        }
                    }

                    hasNeighbor.Should().Be(true, "Линия должна быть непрерывной.");
                }
            }
        }

        /// <summary>
        ///     Тест проверяет, что нет различий, с какой стороны строить линию. От начала до конца или наоборот.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(CubeCoordsHelperTestCases), nameof(CubeCoordsHelperTestCases.TestCases))]
        public void CubeDrawLine_DifferentPoints_ReverseEquals(int sOffsetX, int sOffsetY, int offsetX, int offsetY)
        {
            // ARRANGE

            CubeCoords startCubeCoords = HexHelper.ConvertToCube(sOffsetX, sOffsetY);
            CubeCoords finishCubeCoords = HexHelper.ConvertToCube(offsetX, offsetY);

            // ACT
            CubeCoords[] line = CubeCoordsHelper.CubeDrawLine(startCubeCoords, finishCubeCoords);
            CubeCoords[] reverseLine = CubeCoordsHelper.CubeDrawLine(finishCubeCoords, startCubeCoords);

            // ASSERT
            for (int i = 0; i < line.Length; i++)
            {
                CubeCoords linePoint = line[i];
                CubeCoords reversePoint = reverseLine[reverseLine.Length - i - 1];

                reversePoint.Should().Be(linePoint);
            }
        }
    }
}