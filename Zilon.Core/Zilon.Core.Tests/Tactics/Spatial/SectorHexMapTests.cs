using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.MapGenerators;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tactics.Spatial.Tests
{
    [TestFixture]
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
        /// Тест проверяет, что на карте с препятсвием целевой узел НЕ находится на линии видимости.
        /// </summary>
        [Test]
        public void TargetIsOnLine_MapWithObstacle_HasNotLine()
        {
            // ARRANGE
            var map = new SectorHexMap();
            MapFiller.FillSquareMap(map, 3, (x, y) =>
            {
                return new MapFiller.HexNodeOptions
                {
                    IsObstacle = x == 1 && y == 0  // (0, 1) здесь будет препятсвие
                };
            });

            var startMap = map.HexNodes.SelectBy(0, 0);
            var targetMap = map.HexNodes.SelectBy(2, 1);




            // ACT
            var fact = map.TargetIsOnLine(startMap, targetMap);



            // ASSERT
            fact.Should().BeFalse();
        }

        /// <summary>
        /// Тест проверяет, что на карте с препятсвием, не мешающем обзору,
        /// целевой узел находится на линии видимости.
        /// </summary>
        /// <remarks>
        /// Когда между персонажами 
        /// </remarks>
        [Test]
        public void TargetIsOnLine_MapWithObstacleInSide_HasNotLine()
        {
            // ARRANGE
            var map = new SectorHexMap();
            MapFiller.FillSquareMap(map, 3, (x, y) =>
            {
                return new MapFiller.HexNodeOptions
                {
                    IsObstacle = x == 1 && y == 1  // (1, 1) здесь будет препятсвие
                };
            });

            var startMap = map.HexNodes.SelectBy(0, 0);
            var targetMap = map.HexNodes.SelectBy(2, 1);




            // ACT
            var fact = map.TargetIsOnLine(startMap, targetMap);



            // ASSERT
            fact.Should().BeFalse();
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
                mapSize: 2);

            MapFiller.FillSquareMap(map,
                startX: 3,
                startY: 0,
                mapSize: 2);

            var startMap = map.HexNodes.SelectBy(0, 0);
            var targetMap = map.HexNodes.SelectBy(4, 0);




            // ACT
            var fact = map.TargetIsOnLine(startMap, targetMap);



            // ASSERT
            fact.Should().BeFalse();
        }
    }
}