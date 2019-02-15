using Zilon.Core.MapGenerators.RoomStyle;
using System;
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Tactics.Spatial;
using Moq;
using System.Linq;

namespace Zilon.Core.MapGenerators.RoomStyle.Tests
{
    [TestFixture]
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
                var rooms = generator.GenerateRoomsInGrid(20, 2, 20, new[] { RoomTransition.CreateGlobalExit() });
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
                var rooms = generator.GenerateRoomsInGrid(20, 2, 20, new[] { RoomTransition.CreateGlobalExit() });
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
            var transition = RoomTransition.CreateGlobalExit();
            var availableTransitions = new[] { transition };

            var randomMock = new Mock<IRoomGeneratorRandomSource>();
            randomMock.Setup(x => x.RollRoomMatrixPositions(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new[] { new OffsetCoords(0, 0) });
            randomMock.Setup(x => x.RollTransitions(It.IsAny<IEnumerable<RoomTransition>>()))
                .Returns(new[] { transition });
            var random = randomMock.Object;

            var generator = new RoomGenerator(random);

            var expectedTransitions = new[] { transition };



            // ACT
            var factRooms = generator.GenerateRoomsInGrid(1, 1, 1, availableTransitions);



            // ASSERT
            factRooms.ElementAt(0).Transitions.Should().BeEquivalentTo(availableTransitions);
        }
    }
}