using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Common;

namespace Zilon.Core.Tests.Common
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MatrixHelperTests
    {
        /// <summary>
        /// Тест проверяет, что метод поворота матрицы по часовой стрелке корректно поворачивает матрицу.
        /// </summary>
        [Test]
        public void RotateClockwise_3x3_ReturnRotatedMatrix()
        {
            // ARRANGE
            var source = new int[,] {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            var expected = new int[,] {
                { 7, 4, 1 },
                { 8, 5, 2 },
                { 9, 6, 3 }
            };



            // ACT
            var fact = MatrixHelper.RotateClockwise(source);



            // ASSERT
            fact.Should().BeEquivalentTo(expected);
        }
    }
}