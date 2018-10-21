using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture]
    public class SectorProceduralGeneratorTests
    {
        /// <summary>
        /// Тест проверяет, что сектор из цепочки комнат строится без ошибок.
        /// </summary>
        [Test]
        public void Generate_SimpleSnakeMaze_NoExceptions()
        {
            // ARRANGE
            var expectedRolls = 10;

            var rollIndex = 0;
            var rolledOffsetCoords = new[] {
                new OffsetCoords(0, 0),new OffsetCoords(1, 0), new OffsetCoords(2, 0), new OffsetCoords(3, 0),
                new OffsetCoords(3, 1), new OffsetCoords(2, 1), new OffsetCoords(1, 1), new OffsetCoords(0, 1),
                new OffsetCoords(0, 2),new OffsetCoords(1, 2)
            };

            var rolledSize = new Size(3, 3);

            var randomSourceMock = new Mock<ISectorGeneratorRandomSource>();
            randomSourceMock.Setup(x => x.RollRoomPosition(It.IsAny<int>()))
                .Returns(() =>
                {
                    var rolled = rolledOffsetCoords[rollIndex];
                    rollIndex++;
                    return rolled;
                });

            randomSourceMock.Setup(x => x.RollRoomSize(It.IsAny<int>()))
                .Returns(rolledSize);

            var rolledConnectedRoomIndexes = new[] {
                new[]{ 1 }, new[]{ 2 },new[]{ 3 },new[]{ 4 },
                new[]{ 5 },new[]{ 6 },new[]{ 7 },new[]{ 8 },
                new[]{ 9 }
            };
            randomSourceMock.Setup(x => x.RollConnectedRooms(It.IsAny<Room>(), 1, 100, It.IsAny<IList<Room>>()))
                .Returns<Room, int, int, IList<Room>>((room, max, p, rooms) =>
                {
                    if (rollIndex < expectedRolls - 1)
                    {
                        var connectedRoomIndex = rolledConnectedRoomIndexes[rollIndex][0];
                        return new[] { rooms[connectedRoomIndex] };
                    }
                    else
                    {
                        return null;
                    }
                });

            var randomSource = randomSourceMock.Object;

            var schemeService = CreateSchemeService();

            var sector = CreateSector();

            var map = CreateFakeMap();

            var botPlayer = CreateBotPlayer();

            var generator = CreateGenerator(randomSource, schemeService, botPlayer);


            // ACT
            Action act = () =>
            {
                generator.Generate(sector, map);
            };


            // ASSERT
            act.Should().NotThrow();
        }

        private static SectorProceduralGenerator CreateGenerator(ISectorGeneratorRandomSource randomSource,
            ISchemeService schemeService,
            IBotPlayer botPlayer)
        {
            var dropResolverMock = new Mock<IDropResolver>();
            var dropResolver = dropResolverMock.Object;

            var actorManagerMock = new Mock<IActorManager>();
            var actorManager = actorManagerMock.Object;

            var propContainerManagerMock = new Mock<IPropContainerManager>();
            var propContainerManager = propContainerManagerMock.Object;

            return new SectorProceduralGenerator(
                actorManager,
                propContainerManager,
                randomSource,
                botPlayer, 
                schemeService,
                dropResolver);
        }

        /// <summary>
        /// Тест проверяет, что сектор с базовыми сервисам генерации случайностей
        /// строится без ошибок за допустимое время.
        /// </summary>
        [Test()]
        [Timeout(3 * 60 * 1000)]
        public void Generate_BaseRandomServices_NoExceptions()
        {
            // ARRANGE
            var dice = new Dice(123);
            var randomSource = new SectorGeneratorRandomSource(dice);

            var schemeService = CreateSchemeService();

            var sector = CreateSector();

            var map = CreateFakeMap();

            var botPlayer = CreateBotPlayer();


            var generator = CreateGenerator(randomSource, schemeService, botPlayer);


            // ACT
            Action act = () =>
            {
                generator.Generate(sector, map);
            };


            // ASSERT
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, генератор не создаёт одинаковых узлов (равные координаты).
        /// </summary>
        [Test()]
        public void Generate_BaseRandomServices_NoOverlapNodes()
        {
            // ARRANGE
            var dice = new Dice(3245);
            var randomSource = new SectorGeneratorRandomSource(dice);

            var schemeService = CreateSchemeService();

            var sector = CreateSector();

            var map = CreateFakeMap();

            var botPlayer = CreateBotPlayer();


            var generator = CreateGenerator(randomSource, schemeService, botPlayer);


            // ACT
            generator.Generate(sector, map);



            // ASSERT
            var hexNodes = map.Nodes.Cast<HexNode>().ToArray();
            foreach (var node in hexNodes)
            {
                var sameNode = hexNodes.Where(x => x != node && x.OffsetX == node.OffsetX && x.OffsetY == node.OffsetY);
                sameNode.Should().BeEmpty();
            }
        }

        /// <summary>
        /// Тест проверяет, генератор не создаёт одинаковых ребер (равные соединённые узлы).
        /// </summary>
        [Test()]
        public void Generate_BaseRandomServices_NoEqualEdges()
        {
            // ARRANGE
            var dice = new Dice(3245);
            var randomSource = new SectorGeneratorRandomSource(dice);

            var schemeService = CreateSchemeService();
            var sector = CreateSector();

            var map = CreateFakeMap();

            var botPlayer = CreateBotPlayer();


            var generator = CreateGenerator(randomSource, schemeService, botPlayer);


            // ACT
            generator.Generate(sector, map);



            // ASSERT
            foreach (var edge in map.Edges)
            {
                var sameEdge = map.Edges.Where(x => x != edge && ((x.Nodes[0] == edge.Nodes[0] && x.Nodes[1] == edge.Nodes[1]) || (x.Nodes[0] == edge.Nodes[1] && x.Nodes[1] == edge.Nodes[0])));
                sameEdge.Should().BeEmpty($"Ребро с {edge.Nodes[0]} и {edge.Nodes[1]} уже есть.");
            }
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
            var edges = new List<IEdge>();
            var mapMock = new Mock<IMap>();
            mapMock.SetupGet(x => x.Nodes).Returns(nodes);
            mapMock.SetupGet(x => x.Edges).Returns(edges);
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
    }
}