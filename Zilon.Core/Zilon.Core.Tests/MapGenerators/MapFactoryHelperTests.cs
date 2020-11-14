using FluentAssertions;
using NUnit.Framework;
using Zilon.Core.Common;
using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture]
    public class MapFactoryHelperTests
    {
        /// <summary>
        /// Тест проверяет, что матрица успешно расширяется до размера 7.
        /// Это означает, что для каждого узла вокруг будут добавлены соседи с тем же значением.
        /// </summary>
        [Test]
        public void ResizeMatrixTo7Test()
        {
            // ARRANGE

            var matrix = new Matrix<bool>(1, 1);
            matrix[0, 0] = true;

            // ACT

            var resizedMatrix = MapFactoryHelper.ResizeMatrixTo7(matrix);

            // ASSERT

            resizedMatrix[1, 1].Should().BeTrue();
            var n = HexHelper.GetNeighbors(1, 1);
            foreach (var c in n)
            {
                resizedMatrix[c.X, c.Y].Should().BeTrue();
            }
        }
    }
}