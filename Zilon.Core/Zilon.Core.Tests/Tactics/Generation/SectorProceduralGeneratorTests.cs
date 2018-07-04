using Moq;
using NUnit.Framework;

namespace Zilon.Core.Tactics.Generation.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Zilon.Core;
    using Zilon.Core.CommonServices.Dices;
    using Zilon.Core.Tactics.Spatial;

    [TestFixture()]
    public class SectorProceduralGeneratorTests
    {
        /// <summary>
        /// Тест проверяет, что сектор из цепочки комнат строится без ошибок.
        /// </summary>
        [Test()]
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

            var sectorMock = new Mock<ISector>();
            var sector = sectorMock.Object;

            var nodes = new List<IMapNode>();
            var edges = new List<IEdge>();
            var mapMock = new Mock<IMap>();
            mapMock.SetupProperty(x => x.Nodes, nodes);
            mapMock.SetupProperty(x => x.Edges, edges);
            var map = mapMock.Object;


            var generator = new SectorProceduralGenerator(randomSource);


            // ACT
            Action act = () =>
            {
                generator.Generate(sector, map);
            };


            // ASSERT
            act.Should().NotThrow();
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
            var dice = new Dice();
            var randomSource = new SectorGeneratorRandomSource(dice);

            var sectorMock = new Mock<ISector>();
            var sector = sectorMock.Object;

            var nodes = new List<IMapNode>();
            var edges = new List<IEdge>();
            var mapMock = new Mock<IMap>();
            mapMock.SetupProperty(x => x.Nodes, nodes);
            mapMock.SetupProperty(x => x.Edges, edges);
            var map = mapMock.Object;


            var generator = new SectorProceduralGenerator(randomSource);


            // ACT
            Action act = () =>
            {
                generator.Generate(sector, map);
            };


            // ASSERT
            act.Should().NotThrow();
        }


        /// <summary>
        /// Тест проверяет, что генератор корректно передаёт набор потенциальных
        /// соседей при соединении комнат.
        /// Выбрана комната в ячейке (4, 2) размером (10, 6).
        /// Выбрана комната в ячейке(3, 1) размером(3, 3).
        /// Выбрана комната в ячейке(2, 2) размером(6, 2). -- тут неправильно. дублируется
        /// Выбрана комната в ячейке(4, 2) размером(3, 6).
        /// Выбрана комната в ячейке(2, 2) размером(3, 2). -- тут неправильно. дублируется
        /// Выбрана комната в ячейке(3, 4) размером(1, 10).
        /// Выбрана комната в ячейке(4, 1) размером(2, 8).
        /// Выбрана комната в ячейке(2, 4) размером(7, 4).
        /// Выбрана комната в ячейке(4, 3) размером(5, 7).
        /// Выбрана комната в ячейке(1, 4) размером(9, 2).
        /// </summary>
        [Test()]
        [Ignore("В исходных данных для теста была обнаружена ошибка. Сначала нужно пофиксить её.")]
        public void Generate_RandomRooms_CorectNeighborInputDuringNeighborSelection()
        {
            // ARRANGE
            var expectedRolls = 10;

            var rollIndex = 0;
            var rolledOffsetCoords = new[] {
                new OffsetCoords(4, 2),
                new OffsetCoords(3, 1),
                new OffsetCoords(2, 2),
                new OffsetCoords(4, 2),
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

            var sectorMock = new Mock<ISector>();
            var sector = sectorMock.Object;

            var nodes = new List<IMapNode>();
            var edges = new List<IEdge>();
            var mapMock = new Mock<IMap>();
            mapMock.SetupProperty(x => x.Nodes, nodes);
            mapMock.SetupProperty(x => x.Edges, edges);
            var map = mapMock.Object;


            var generator = new SectorProceduralGenerator(randomSource);


            // ACT
            Action act = () =>
            {
                generator.Generate(sector, map);
            };


            // ASSERT
            act.Should().NotThrow();
        }
    }
}