using System;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.Tests.MapGenerators.RoomStyle;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture][Parallelizable(ParallelScope.All)]
    public class SectorProceduralGeneratorTests
    {
        /// <summary>
        /// Тест проверяет, что для различных карт в виде змейки генерация работает без ошибок.
        /// </summary>
        [Test]
        public void Create_DifferentMaps_NoExceptions()
        {
            // ARRANGE
            var roomGenerator = new TestSnakeRoomGenerator();
            var mapFactory = new RoomMapFactory(roomGenerator);

            var botPlayer = CreateBotPlayer();
            var generator = CreateGenerator(botPlayer, mapFactory);
            var sectorScheme = CreateSectorScheme();

            // ACT
            Func<Task> act = async () =>
            {
                var sector = await generator.GenerateDungeonAsync(sectorScheme).ConfigureAwait(false);
            };

            // ASSERT
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, что для различных карт генератор сектора работает без ошибок.
        /// </summary>
        [Test]
        [TestCase(1)]
        [TestCase(123)]
        [TestCase(3257)]
        [TestCase(636)]
        [TestCase(100000)]

        public async Task Create_DifferentMapsRealDice_NoExceptions(int diceSeed)
        {
            // ARRANGE
            var linearDice = new LinearDice(diceSeed);
            var gaussDice = new GaussDice(diceSeed);
            var randomSource = new RoomGeneratorRandomSource(linearDice, gaussDice);
            var roomGenerator = new RoomGenerator(randomSource);
            var mapFactory = new RoomMapFactory(roomGenerator);

            var botPlayer = CreateBotPlayer();
            var generator = CreateGenerator(botPlayer, mapFactory);
            var sectorScheme = CreateSectorScheme();

            // ACT
            await generator.GenerateDungeonAsync(sectorScheme).ConfigureAwait(false);
        }

        private static ISectorGenerator CreateGenerator(IBotPlayer botPlayer,
            IMapFactory mapFactory)
        {
            var chestGeneratorMock = new Mock<IChestGenerator>();
            var chestGenerator = chestGeneratorMock.Object;

            var monsterGeneratorMock = new Mock<IMonsterGenerator>();
            var monsterGenerator = monsterGeneratorMock.Object;

            var sectorFactoryMock = new Mock<ISectorFactory>();
            var sectorFactory = sectorFactoryMock.Object;

            var citizenGeneratorMock = new Mock<ICitizenGenerator>();
            var citizenGenerator = citizenGeneratorMock.Object;

            var mapFactorySelectorMock = new Mock<IMapFactorySelector>();
            mapFactorySelectorMock.Setup(x => x.GetMapFactory(It.IsAny<ISectorSubScheme>()))
                .Returns(mapFactory);
            var mapFactorySelector = mapFactorySelectorMock.Object;

            return new SectorGenerator(mapFactorySelector,
                sectorFactory,
                monsterGenerator,
                chestGenerator,
                citizenGenerator,
                botPlayer);
        }

        private static IBotPlayer CreateBotPlayer()
        {
            var botPlayerMock = new Mock<IBotPlayer>();
            var botPlayer = botPlayerMock.Object;
            return botPlayer;
        }

        private static ISectorSubScheme CreateSectorScheme()
        {
            return new TestSectorSubScheme
            {
                RegularMonsterSids = new[] { "rat" },
                RegionCount = 20,
                RegionSize = 20
            };
        }
    }
}