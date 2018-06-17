using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Tests.Tactics.Spatial.TestCases;

namespace Zilon.Core.Common.Tests
{
    [TestFixture]
    public class HexHelperTests
    {
        /// <summary>
        /// Тест проверяет корректность преобразования координат смещения
        /// в кубические координаты для сетки шестигранников.
        /// </summary>
        [Test]
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

        [Test]
        [TestCaseSource(typeof(HexWorldPositionTestCaseSource),
            nameof(HexWorldPositionTestCaseSource.TestCases))]
        public float[] ConvertToWorldTest(int offsetX, int offsetY)
        {
            // ARRANGE
            

            // ACT
            var factCubeCoords = HexHelper.ConvertToWorld(offsetX, offsetY);


            // ASSERT
            return factCubeCoords;
        }
    }
}