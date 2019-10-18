using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.MapGenerators.RoomStyle
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class RoomMapFactoryTests
    {
        /// <summary>
        /// Тест проверяет, что карта из цепочки комнат строится без ошибок.
        /// </summary>
        [Test]
        public void Create_SimpleSnakeMaze_NoExceptions()
        {
            var roomGenerator = new TestSnakeRoomGenerator();
            var factory = new RoomMapFactory(roomGenerator);
            var sectorScheme = CreateSectorScheme();

            // ACT
            Func<Task> act = async () =>
            {
                var map = await factory.CreateAsync(sectorScheme);
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
        [Parallelizable]
        public void Create_RealRoomRandom_NoExceptions(int diceSeed)
        {
            // ARRANGE
            var dice = new Dice(diceSeed);
            var randomGenerator = new GaussRandomNumberGenerator(dice);
            var randomSource = new RoomGeneratorRandomSource(dice, randomGenerator);
            var roomGenerator = new RoomGenerator(randomSource);
            var factory = new RoomMapFactory(roomGenerator);
            var sectorScheme = CreateSectorScheme();

            // ACT
            Func<Task> act = async () =>
            {
                var map = await factory.CreateAsync(sectorScheme);
            };

            // ARRANGE
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, что карта из цепочки комнат строится без ошибок.
        /// </summary>
        [Test]
        [Parallelizable]
        public async Task Create_RealRoomRandom_NoOverlapNodesAsync()
        {
            // ARRANGE
            var dice = new Dice(3245);
            var randomGenerator = new GaussRandomNumberGenerator(dice);
            var randomSource = new RoomGeneratorRandomSource(dice, randomGenerator);
            var roomGenerator = new RoomGenerator(randomSource);
            var factory = new RoomMapFactory(roomGenerator);
            var sectorScheme = CreateSectorScheme();

            // ACT
            var map = await factory.CreateAsync(sectorScheme);

            // ARRANGE
            var hexNodes = map.Nodes.Cast<HexNode>().ToArray();
            foreach (var node in hexNodes)
            {
                var sameNode = hexNodes.Where(x => x != node && x.OffsetX == node.OffsetX && x.OffsetY == node.OffsetY);
                sameNode.Should().BeEmpty();
            }
        }

        private static ISectorSubScheme CreateSectorScheme()
        {
            return new TestSectorSubScheme
            {
                RegionSize = 20,
                RegionCount = 20
            };
        }
    }
}