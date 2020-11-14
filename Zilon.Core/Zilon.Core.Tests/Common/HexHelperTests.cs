using Zilon.Core.Common;
using Zilon.Core.Tests.Tactics.Spatial.TestCases;

namespace Zilon.Core.Tests.Common
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class HexHelperTests
    {
        /// <summary>
        ///     Тест проверяет корректность преобразования координат смещения
        ///     в кубические координаты для сетки шестигранников.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(ConvertOffsetToCubeTestCaseSource),
            nameof(ConvertOffsetToCubeTestCaseSource.TestCases))]
        public void ConvertToCubeTest(int offsetX, int offsetY, int cubeX, int cubeY, int cubeZ)
        {
            // ARRANGE
            CubeCoords expectedCubeCoords = new CubeCoords(cubeX, cubeY, cubeZ);


            // ACT
            CubeCoords factCubeCoords = HexHelper.ConvertToCube(offsetX, offsetY);


            // ASSERT
            factCubeCoords.Should().BeEquivalentTo(expectedCubeCoords);
        }

        [Test]
        [TestCaseSource(typeof(ConvertOffsetToCubeTestCaseSource),
            nameof(ConvertOffsetToCubeTestCaseSource.TestCases))]
        public void ConvertToOffsetTest(int offsetX, int offsetY, int cubeX, int cubeY, int cubeZ)
        {
            // ARRANGE
            CubeCoords cubeCoords = new CubeCoords(cubeX, cubeY, cubeZ);
            OffsetCoords expectedOffset = new OffsetCoords(offsetX, offsetY);


            // ACT
            OffsetCoords factOffsetCoords = HexHelper.ConvertToOffset(cubeCoords);


            // ASSERT
            factOffsetCoords.Should().BeEquivalentTo(expectedOffset);
        }


        [Test]
        [TestCaseSource(typeof(HexWorldPositionTestCaseSource),
            nameof(HexWorldPositionTestCaseSource.TestCases))]
        public float[] ConvertToWorldTest(int offsetX, int offsetY)
        {
            // ARRANGE


            // ACT
            float[] factCubeCoords = HexHelper.ConvertToWorld(offsetX, offsetY);


            // ASSERT
            return factCubeCoords;
        }
    }
}