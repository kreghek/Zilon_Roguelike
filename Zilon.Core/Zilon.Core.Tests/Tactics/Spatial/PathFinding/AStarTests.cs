using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Tests.Tactics.Spatial.PathFinding.TestCases;

namespace Zilon.Core.Tactics.Spatial.PathFinding.Tests
{
    [TestFixture]
    public class AStarTests
    {
        /// <summary>
        /// Тест проверяет корректность алгоритма в сетке шестиугольников.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(TestCaseSource), nameof(TestCaseSource.TestCases))]
        public void Run_HexGrid_PathFound(List<IMapNode> nodes, List<IEdge> edges, IMapNode[] expectedPath)
        {
            // ARRAGE
            var mapMock = new Mock<IMap<IMapNode, IEdge>>();

            mapMock.SetupProperty(x => x.Nodes, nodes);
            mapMock.SetupProperty(x => x.Edges, edges);

            var map = mapMock.Object;

            var astar = new AStar(map, expectedPath.First(), expectedPath.Last());



            // ACT
            var factState = astar.Run();




            // ASSERT

            factState.Should().Be(State.GoalFound);

            var factPath = astar.GetPath();

            for (var i = 0; i < expectedPath.Count(); i++)
            {
                factPath[i].Should().Be(expectedPath[i]);
            }
        }
    }
}