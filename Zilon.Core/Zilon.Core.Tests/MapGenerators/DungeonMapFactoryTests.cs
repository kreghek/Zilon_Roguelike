using System;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture]
    public class DungeonMapFactoryTests
    {
        /// <summary>
        /// Тест проверяет, что карта из цепочки комнат строится без ошибок.
        /// </summary>
        [Test, Ignore("На самом деле этот тест не строит змейку")]
        public void Create_SimpleSnakeMaze_NoExceptions()
        {
            // В конечном счёте этот генератор выбирает случайные комнаты, но в порядке их генерации.
            // Это не змейка. Нужно будет модифицировать.
            var randomSource = new TestSnakeRandomSource();


            var roomGeneratorSettings = new RoomGeneratorSettings
            {
                RoomCount = 10,
                RoomCellSize = 10,
                MaxNeighbors = 1
            };

            var factory = new DungeonMapFactory(randomSource, roomGeneratorSettings);



            // ACT
            Action act = () =>
            {
                var map = factory.Create();
            };



            // ARRANGE
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, что произвольная карта строится без ошибок. И за допустимое время.
        /// </summary>
        [Test]
        [Timeout(3 * 60 * 1000)]
        [TestCase(123)]
        [TestCase(1)]
        [TestCase(8674)]
        [TestCase(1000)]
        public void Create_RealRandom_NoExceptions(int diceSeed)
        {
            // ARRANGE
            var dice = new Dice(diceSeed);
            var randomSource = new SectorGeneratorRandomSource(dice);
            var factory = new DungeonMapFactory(randomSource);



            // ACT
            Action act = () =>
            {
                var map = factory.Create();
            };



            // ARRANGE
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, что карта из цепочки комнат строится без ошибок.
        /// </summary>
        [Test]
        public void Create_RealRandom_NoOverlapNodes()
        {
            // ARRANGE
            var dice = new Dice(3245);
            var randomSource = new SectorGeneratorRandomSource(dice);
            var factory = new DungeonMapFactory(randomSource);



            // ACT
            var map = factory.Create();



            // ARRANGE
            var hexNodes = map.Nodes.Cast<HexNode>().ToArray();
            foreach (var node in hexNodes)
            {
                var sameNode = hexNodes.Where(x => x != node && x.OffsetX == node.OffsetX && x.OffsetY == node.OffsetY);
                sameNode.Should().BeEmpty();
            }
        }

        private static IMapFactory CreateFactory(ISectorGeneratorRandomSource randomSource)
        {
            return new DungeonMapFactory(randomSource);
        }
    }
}