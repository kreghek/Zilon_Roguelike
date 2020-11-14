using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics.Spatial.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class SectorHexMapTests
    {
        /// <summary>
        /// Тест проверяет, что на пустой карте целевой узел находится на линии видимости.
        /// </summary>
        [Test]
        public void TargetIsOnLine_EmptyMap_HasLine()
        {
            // ARRANGE
            var map = new SectorHexMap();
            MapFiller.FillSquareMap(map, 3);

            var startMap = map.HexNodes.SelectBy(0, 0);
            var targetMap = map.HexNodes.SelectBy(2, 1);

            // ACT
            var fact = map.TargetIsOnLine(startMap, targetMap);

            // ASSERT
            fact.Should().BeTrue();
        }

        /// <summary>
        /// Тест проверяет, что на карте с узлами, не выстроенными в одну цепочку,
        /// видимости не будет.
        /// </summary>
        /// <remarks>
        /// Эмулируем разные комнаты.
        /// </remarks>
        [Test]
        public void TargetIsOnLine_MapWithGap_HasLine()
        {
            // ARRANGE
            var map = new SectorHexMap();
            MapFiller.FillSquareMap(map,
                2);

            MapFiller.FillSquareMap(map,
                3,
                0,
                2);

            var startMap = map.HexNodes.SelectBy(0, 0);
            var targetMap = map.HexNodes.SelectBy(4, 0);

            // ACT
            var fact = map.TargetIsOnLine(startMap, targetMap);

            // ASSERT
            fact.Should().BeFalse();
        }
    }
}