using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tactics.Spatial.PathFinding;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Tactics.Spatial.PathFinding
{
    [TestFixture]
    public class AStarTests
    {
        /// <summary>
        /// Тест проверяет корректность алгоритма в сетке шестиугольников.
        /// </summary>
        [Test]
        public void Run_ShortGraph_PathFound()
        {
            // ARRAGE
            var nodes = new List<IMapNode>();
            var edges = new List<IEdge>();
            var expectedPath = new List<IMapNode>();

            nodes.Add(new HexNode(0, 0));
            nodes.Add(new HexNode(1, 0));
            nodes.Add(new HexNode(0, 1));

            edges.Add(new Edge(nodes[0], nodes[2]));
            edges.Add(new Edge(nodes[2], nodes[1]));

            expectedPath.Add(nodes[0]);
            expectedPath.Add(nodes[2]);
            expectedPath.Add(nodes[1]);


            var mapMock = new Mock<IMap>();

            mapMock.SetupGet(x => x.Nodes).Returns(nodes);
            mapMock.SetupGet(x => x.Edges).Returns(edges);
            mapMock.Setup(x => x.IsPositionAvailableFor(It.IsAny<IMapNode>(), It.IsAny<IActor>()))
                .Returns(true);

            var map = mapMock.Object;

            var context = CreatePathFindingContext();

            var astar = new AStar(map, context, expectedPath.First(), expectedPath.Last());



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

        private static IPathFindingContext CreatePathFindingContext()
        {
            var contextMock = new Mock<IPathFindingContext>();
            var context = contextMock.Object;
            return context;
        }

        /// <summary>
        /// Тест проверяет корректность алгоритма в сетке шестиугольников.
        /// Точки расположены так, что есть прямой путь.
        /// </summary>
        [Test]
        public void Run_GridGraphAndLinePath_PathFound()
        {
            // ARRAGE
            var map = CreateGridOpenMap();

            var expectedPath = new IMapNode[] {
                map.Nodes.Cast<HexNode>().SelectBy(1,1),
                map.Nodes.Cast<HexNode>().SelectBy(2,2),
                map.Nodes.Cast<HexNode>().SelectBy(2,3),
                map.Nodes.Cast<HexNode>().SelectBy(3,4),
                map.Nodes.Cast<HexNode>().SelectBy(3,5),
                map.Nodes.Cast<HexNode>().SelectBy(4,6)
            };


            var context = CreatePathFindingContext();

            var astar = new AStar(map, context, expectedPath.First(), expectedPath.Last());



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

        /// <summary>
        /// Тест проверяет корректность обхода соседей при выборе пути.
        /// Обход соседей должен начинаться с левого и идти по часовой стрелке.
        /// </summary>
        [Test]
        public void Run_CheckNeighborBypass_ExpectedPath()
        {
            // ARRAGE
            var map = CreateGridOpenMap();

            var expectedPath = new IMapNode[] {
                map.Nodes.OfType<HexNode>().SelectBy(1, 1),
                map.Nodes.OfType<HexNode>().SelectBy(2, 2),
                map.Nodes.OfType<HexNode>().SelectBy(2, 3),
                map.Nodes.OfType<HexNode>().SelectBy(3, 3),
                map.Nodes.OfType<HexNode>().SelectBy(4, 3),
                map.Nodes.OfType<HexNode>().SelectBy(5, 3),
            };


            var context = CreatePathFindingContext();

            var astar = new AStar(map, context, expectedPath.First(), expectedPath.Last());



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


        /// <summary>
        /// Создаёт открытую карту без препятствий.
        /// </summary>
        /// <returns></returns>
        private static IMap CreateGridOpenMap()
        {
            var nodes = new List<IMapNode>();
            var edges = new List<IEdge>();

            var mapMock = new Mock<IMap>();

            mapMock.SetupGet(x => x.Nodes).Returns(nodes);
            mapMock.SetupGet(x => x.Edges).Returns(edges);
            mapMock.Setup(x => x.IsPositionAvailableFor(It.IsAny<IMapNode>(), It.IsAny<IActor>()))
                .Returns(true);

            var map = mapMock.Object;

            var genertor = new GridMapGenerator();
            genertor.CreateMap(map);
            return map;
        }
    }
}