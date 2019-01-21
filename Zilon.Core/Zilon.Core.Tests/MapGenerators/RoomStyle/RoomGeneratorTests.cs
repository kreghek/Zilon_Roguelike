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
            var settings = new RoomGeneratorSettings()
            {
                MaxNeighbors = 100
            };
            var generator = new RoomGenerator(random, settings);
            var graphMap = new GraphMap();


            // ACT
            Action act = () =>
            {
                var rooms = generator.GenerateRoomsInGrid();
                var edgeHash = new HashSet<string>();
                generator.CreateRoomNodes(graphMap, rooms, edgeHash);
                generator.BuildRoomCorridors(graphMap, rooms, edgeHash);
            };



            // ASSERT
            act.Should().NotThrow();
        }
    }
}