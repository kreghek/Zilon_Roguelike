using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.Core.Tests.MapGenerators.RoomStyle
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class RoomGeneratorTests
    {
        /// <summary>
        /// Тест проверяет, что генератор корректно отрабатывает с источником рандома, выбрасывающим лучшие случаи (см бенчи).
        /// </summary>
        [Test]
        [Category("integration")]
        public void GenerateRoomsInGrid_WithFixCompact_NotThrowsExceptions()
        {
            // ARRANGE
            var random = new FixCompactRoomGeneratorRandomSource();
            var generator = new RoomGenerator(random);
            var graphMap = new SectorHexMap();

            // ACT
            Action act = () =>
            {
                var rooms = generator.GenerateRoomsInGrid(20, 2, 20, Array.Empty<RoomTransition>());
                var edgeHash = new HashSet<string>();
                generator.CreateRoomNodes(graphMap, rooms, edgeHash);
                generator.BuildRoomCorridors(graphMap, rooms, edgeHash);
            };

            // ASSERT
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, что генератор корректно отрабатывает с источником рандома, выбрасывающим худшие случаи (см бенчи).
        /// </summary>
        [Test]
        [Category("integration")]
        public void GenerateRoomsInGrid_WithFixLarge_NotThrowsExceptions()
        {
            // ARRANGE
            var random = new FixLargeRoomGeneratorRandomSource();
            var generator = new RoomGenerator(random);
            var graphMap = new SectorHexMap();

            // ACT
            Action act = () =>
            {
                var rooms = generator.GenerateRoomsInGrid(20, 2, 20, Array.Empty<RoomTransition>());
                var edgeHash = new HashSet<string>();
                generator.CreateRoomNodes(graphMap, rooms, edgeHash);
                generator.BuildRoomCorridors(graphMap, rooms, edgeHash);
            };

            // ASSERT
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, что если в схеме сектора обозначены переходы,
        /// то они генерируются в комнате.
        /// </summary>
        [Test]
        public void GenerateRoomsInGrid_Transitions()
        {
            // ARRANGE
            var sectorNodeMock = new Mock<ISectorNode>();
            var sectorNode = sectorNodeMock.Object;
            var transition = new RoomTransition(sectorNode);
            var availableTransitions = new[]
            {
                transition
            };

            var randomMock = new Mock<IRoomGeneratorRandomSource>();
            randomMock.Setup(x => x.RollRoomMatrixPositions(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new[]
                {
                    new OffsetCoords(0, 0)
                });
            randomMock.Setup(x => x.RollTransitions(It.IsAny<IEnumerable<RoomTransition>>()))
                .Returns(new[]
                {
                    transition
                });
            randomMock.Setup(x => x.RollRoomSize(It.IsAny<int>(), It.IsAny<int>(), It.IsIn<int>(1)))
                .Returns<int, int, int>((min, max, count) =>
                {
                    return new[]
                    {
                        new Size(0, 0)
                    };
                });
            var random = randomMock.Object;

            var generator = new RoomGenerator(random);

            var expectedTransitions = new[]
            {
                transition
            };

            // ACT
            var factRooms = generator.GenerateRoomsInGrid(1, 1, 1, availableTransitions);

            // ASSERT
            factRooms.ElementAt(0).Transitions.Should().BeEquivalentTo(expectedTransitions);
        }
    }
}