using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
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
                var sector = await generator.GenerateDungeonAsync(sectorScheme);
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

        public void Create_DifferentMapsRealDice_NoExceptions(int diceSeed)
        {
            // ARRANGE
            var dice = new Dice(diceSeed);
            var randomSource = new RoomGeneratorRandomSource(dice);
            var roomGenerator = new RoomGenerator(randomSource);
            var mapFactory = new RoomMapFactory(roomGenerator);

            var schemeService = CreateSchemeService();
            var botPlayer = CreateBotPlayer();
            var generator = CreateGenerator(botPlayer, mapFactory);
            var sectorScheme = CreateSectorScheme();



            // ACT
            generator.GenerateDungeonAsync(sectorScheme).Wait();


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


        private static ISector CreateSector()
        {
            var patrolRoutes = new Dictionary<IActor, IPatrolRoute>();
            var sectorMock = new Mock<ISector>();
            sectorMock.SetupGet(x => x.PatrolRoutes).Returns(patrolRoutes);
            var sector = sectorMock.Object;
            return sector;
        }

        private static IMap CreateFakeMap()
        {
            var nodes = new List<IMapNode>();
            var mapMock = new Mock<IMap>();
            mapMock.SetupGet(x => x.Nodes).Returns(nodes);
            var map = mapMock.Object;
            return map;
        }

        private static ISchemeService CreateSchemeService()
        {
            var schemeServiceMock = new Mock<ISchemeService>();

            var propScheme = new TestPropScheme
            {
                Sid = "test-prop"
            };

            schemeServiceMock.Setup(x => x.GetScheme<IPropScheme>(It.IsAny<string>()))
                .Returns(propScheme);

            var trophyTableScheme = new TestDropTableScheme(0, new DropTableRecordSubScheme[0])
            {
                Sid = "default"
            };
            schemeServiceMock.Setup(x => x.GetScheme<IDropTableScheme>(It.IsAny<string>()))
                .Returns(trophyTableScheme);

            var monsterScheme = new TestMonsterScheme
            {
                PrimaryAct = new TestTacticalActStatsSubScheme()
            };

            schemeServiceMock.Setup(x => x.GetScheme<IMonsterScheme>(It.IsAny<string>()))
                .Returns(monsterScheme);

            var schemeService = schemeServiceMock.Object;
            return schemeService;
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