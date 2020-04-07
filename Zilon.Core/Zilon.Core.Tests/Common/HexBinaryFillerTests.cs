using FluentAssertions;

using NUnit.Framework;

namespace Zilon.Core.Common.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class HexBinaryFillerTests
    {
        [Test]
        public void FloodFill7_OneSize7Area_ReturnsOneCentralPoint()
        {
            // ARRANGE
            var matrix = new Matrix<bool>(10, 10);
            PlaceArea(4, 4, matrix);

            // ACT
            var regions = HexBinaryFiller.FloodFill7(matrix, new OffsetCoords(4, 4));

            // ASSERT
            regions.Should().BeEquivalentTo(new[] { new OffsetCoords(4, 4) });
        }

        [Test]
        public void FloodFill7_2Size7Area_Returns2CentralPoint()
        {
            // ARRANGE
            var matrix = new Matrix<bool>(10, 10);
            PlaceArea(4, 4, matrix);
            PlaceArea(5, 4, matrix);

            // ACT
            var regions = HexBinaryFiller.FloodFill7(matrix, new OffsetCoords(4, 4));

            // ASSERT
            regions.Should().BeEquivalentTo(new[] { new OffsetCoords(4, 4), new OffsetCoords(5, 4) });
        }

        [Test]
        public void FloodFill7_2Size7AreaInDistance_Returns1CentralPoint()
        {
            // ARRANGE
            var matrix = new Matrix<bool>(10, 10);
            PlaceArea(4, 4, matrix);
            PlaceArea(7, 7, matrix);

            // ACT
            var regions = HexBinaryFiller.FloodFill7(matrix, new OffsetCoords(4, 4));

            // ASSERT
            regions.Should().BeEquivalentTo(new[] { new OffsetCoords(4, 4) });
        }

        private void PlaceArea(int x, int y, Matrix<bool> matrix)
        {
            matrix.Items[x, y] = true;
            var neighbors = HexHelper.GetNeighbors(x, y);
            foreach (var neightbor in neighbors)
            {
                matrix.Items[neightbor.X, neightbor.Y] = true;
            }
        }
    }
}