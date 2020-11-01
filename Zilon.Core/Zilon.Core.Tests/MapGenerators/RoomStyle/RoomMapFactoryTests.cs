using System;
using System.Threading.Tasks;
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
        ///     Тест проверяет, что карта из цепочки комнат строится без ошибок.
        /// </summary>
        [Test]
        public void Create_SimpleSnakeMaze_NoExceptions()
        {
            TestSnakeRoomGenerator roomGenerator = new TestSnakeRoomGenerator();
            RoomMapFactory factory = new RoomMapFactory(roomGenerator);
            ISectorSubScheme sectorScheme = CreateSectorScheme();

            SectorMapFactoryOptions sectorFactoryOptions =
                new SectorMapFactoryOptions(sectorScheme.MapGeneratorOptions);

            // ACT
            Func<Task> act = async () =>
            {
                await factory.CreateAsync(sectorFactoryOptions).ConfigureAwait(false);
            };

            // ARRANGE
            act.Should().NotThrow();
        }

        /// <summary>
        ///     Тест проверяет, что произвольная карта строится без ошибок. И за допустимое время.
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
            LinearDice leanerDice = new LinearDice(diceSeed);
            GaussDice gaussDice = new GaussDice(diceSeed);
            RoomGeneratorRandomSource randomSource = new RoomGeneratorRandomSource(leanerDice, gaussDice);
            RoomGenerator roomGenerator = new RoomGenerator(randomSource);
            RoomMapFactory factory = new RoomMapFactory(roomGenerator);
            ISectorSubScheme sectorScheme = CreateSectorScheme();

            SectorMapFactoryOptions sectorFactoryOptions =
                new SectorMapFactoryOptions(sectorScheme.MapGeneratorOptions);

            // ACT
            Func<Task> act = async () =>
            {
                await factory.CreateAsync(sectorFactoryOptions).ConfigureAwait(false);
            };

            // ARRANGE
            act.Should().NotThrow();
        }

        /// <summary>
        ///     Тест проверяет, что карта из цепочки комнат строится без ошибок.
        /// </summary>
        [Test]
        [Parallelizable]
        public async Task Create_RealRoomRandom_NoOverlapNodesAsync()
        {
            // ARRANGE
            LinearDice linearDice = new LinearDice(3245);
            GaussDice gaussDice = new GaussDice(3245);
            RoomGeneratorRandomSource randomSource = new RoomGeneratorRandomSource(linearDice, gaussDice);
            RoomGenerator roomGenerator = new RoomGenerator(randomSource);
            RoomMapFactory factory = new RoomMapFactory(roomGenerator);
            ISectorSubScheme sectorScheme = CreateSectorScheme();

            SectorMapFactoryOptions sectorFactoryOptions =
                new SectorMapFactoryOptions(sectorScheme.MapGeneratorOptions);

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
                    RegionCount = 20, RegionSize = 20
                }
            };
        }
    }
}