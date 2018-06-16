using FluentAssertions;

using NUnit.Framework;

namespace Zilon.Core.Common.Tests
{
    [TestFixture()]
    public class HexHelperTests
    {
        /// <summary>
        /// Тест проверяет корректность преобразования координат смещения
        /// в кубические координаты для сетки шестигранников.
        /// </summary>
        [Test()]
        public void ConvertToCubeTest()
        {
            // ARRANGE
            var offsetX = 1;
            var offsetY = 2;
            var expectedCubeCoords = new CubeCoords(0, -2, 2);

            // ACT
            var factCubeCoords = HexHelper.ConvertToCube(offsetX, offsetY);


            // ASSERT
            factCubeCoords.Should().BeEquivalentTo(expectedCubeCoords);
        }
    }
}