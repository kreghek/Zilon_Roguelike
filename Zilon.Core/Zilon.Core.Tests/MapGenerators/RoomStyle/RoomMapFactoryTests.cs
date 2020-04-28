using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
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

            var sectorFactoryOptions = new SectorMapFactoryOptions(sectorScheme.MapGeneratorOptions);

            // ACT
            Func<Task> act = async () =>
            {
                await factory.CreateAsync(sectorFactoryOptions).ConfigureAwait(false);
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
            var leanerDice = new LinearDice(diceSeed);
            var gaussDice = new GaussDice(diceSeed);
            var randomSource = new RoomGeneratorRandomSource(leanerDice, gaussDice);
            var roomGenerator = new RoomGenerator(randomSource);
            var factory = new RoomMapFactory(roomGenerator);
            var sectorScheme = CreateSectorScheme();

            var sectorFactoryOptions = new SectorMapFactoryOptions(sectorScheme.MapGeneratorOptions);

            // ACT
            Func<Task> act = async () =>
            {
                await factory.CreateAsync(sectorFactoryOptions).ConfigureAwait(false);
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
            var linearDice = new LinearDice(3245);
            var gaussDice = new GaussDice(3245);
            var randomSource = new RoomGeneratorRandomSource(linearDice, gaussDice);
            var roomGenerator = new RoomGenerator(randomSource);
            var factory = new RoomMapFactory(roomGenerator);
            var sectorScheme = CreateSectorScheme();

            var sectorFactoryOptions = new SectorMapFactoryOptions(sectorScheme.MapGeneratorOptions);

            // ACT
            var map = await factory.CreateAsync(sectorFactoryOptions).ConfigureAwait(false);

            // ARRANGE
            var hexNodes = map.Nodes.Cast<HexNode>().ToArray();
            foreach (var node in hexNodes)
            {
                var sameNode = hexNodes.Where(x => x != node && x.OffsetCoords == node.OffsetCoords);
                sameNode.Should().BeEmpty();
            }
        }

        private static ISectorSubScheme CreateSectorScheme()
        {
            return new TestSectorSubScheme
            {
                MapGeneratorOptions = new TestSectorRoomMapFactoryOptionsSubScheme
                {
                    RegionCount = 20,
                    RegionSize = 20,
                }
            };
        }
    }
}