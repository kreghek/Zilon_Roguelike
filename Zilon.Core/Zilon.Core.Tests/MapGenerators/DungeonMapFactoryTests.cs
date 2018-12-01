using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

using Moq;

using NUnit.Framework;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.Tests
{
    [TestFixture]
    public class DungeonMapFactoryTests
    {
        /// <summary>
        /// Тест проверяет, что карта из цепочки комнат строится без ошибок.
        /// </summary>
        [Test]
        public void Create_SimpleSnakeMaze_NoExceptions()
        {
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
                        return new Room[0];
                    }
                });

            var randomSource = randomSourceMock.Object;

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
        /// Тест проверяет, что произвольная карта строится без ошибок. И за допустимое время.
        /// </summary>
        [Test]
        [Timeout(3 * 60 * 1000)]
        public void Create_RealRandom_NoExceptions()
        {
            // ARRANGE
            var dice = new Dice(123);
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

        /// <summary>
        /// Тест проверяет, генератор не создаёт одинаковых ребер (равные соединённые узлы).
        /// </summary>
        [Test]
        public void Create_RealRandom_NoOverlapEdges()
        {
            // ARRANGE
            var dice = new Dice(3245);
            var randomSource = new SectorGeneratorRandomSource(dice);
            var factory = new DungeonMapFactory(randomSource);



            // ACT
            var map = factory.Create();



            // ARRANGE
            foreach (var edge in map.Edges)
            {
                var sameEdge = map.Edges.Where(x => x != edge && 
                    (
                        (x.Nodes[0] == edge.Nodes[0] && x.Nodes[1] == edge.Nodes[1]) ||
                        (x.Nodes[0] == edge.Nodes[1] && x.Nodes[1] == edge.Nodes[0])
                    )
                );
                sameEdge.Should().BeEmpty($"Ребро с {edge.Nodes[0]} и {edge.Nodes[1]} уже есть.");
            }
        }

        private static IMapFactory CreateFactory(ISectorGeneratorRandomSource randomSource)
        {
            return new DungeonMapFactory(randomSource);
        }
    }
}