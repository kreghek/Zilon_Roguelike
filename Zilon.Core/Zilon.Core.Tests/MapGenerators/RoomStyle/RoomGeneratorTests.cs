using Zilon.Core.MapGenerators.RoomStyle;
using System;
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class RoomGeneratorTests
    {
        [Test]
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


        [Test]
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
            var generator = new RoomGenerator();



        }
    }
}