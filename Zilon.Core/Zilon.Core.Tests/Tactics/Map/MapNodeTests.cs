using FluentAssertions;
using NUnit.Framework;
using Zilon.Core.Math;
using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Tests.Tactics.Map
{
    [TestFixture]
    public class MapNodeTests
    {
        [Test]
        public void GetCubeCoords_DifferentStates_CorrectCubeCoords()
        {
            // ARRANGE
            var node = new MapNode();
            node.Coordinates = new Vector2(1, 2);
            var expectedCubeCoords = new Vector3(0, -2, 2);

            // ACT
            var cubeCoords = node.GetCubeCoords();


            // ASSERT
            cubeCoords.Should().BeEquivalentTo(expectedCubeCoords);
        }
    }
}