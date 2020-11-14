using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.PathFinding;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Tactics.Spatial.PathFinding
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AStarTests
    {
        /// <summary>
        /// Тест проверяет корректность алгоритма в сетке шестиугольников.
        /// </summary>
        [Test]
        public void Run_ShortGraph_PathFound()
        {
            // ARRAGE
            var expectedPath = new List<IGraphNode>();

            var map = new GraphMap();

            map.AddNode(new HexNode(0, 0));
            map.AddNode(new HexNode(1, 0));
            map.AddNode(new HexNode(0, 1));

            var nodesArray = map.Nodes.ToArray();

            map.AddEdge(nodesArray[0], nodesArray[2]);
            map.AddEdge(nodesArray[2], nodesArray[1]);

            expectedPath.Add(nodesArray[0]);
            expectedPath.Add(nodesArray[2]);
            expectedPath.Add(nodesArray[1]);

            var context = CreatePathFindingContext(map);

            var astar = new AStar(context, expectedPath.First(), expectedPath.Last());

            // ACT
            var factState = astar.Run();

            // ASSERT

            factState.Should().Be(State.GoalFound);

            var factPath = astar.GetPath();
            factPath.Should().BeEquivalentTo(expectedPath);
        }

        /// <summary>
        /// Тест проверяет корректность алгоритма в сетке шестиугольников.
        /// Точки расположены так, что есть прямой путь.
        /// </summary>
        [Test]
        public async Task Run_GridGraphAndLinePath_PathFound()
        {
            // ARRAGE
            var map = await CreateGridOpenMapAsync().ConfigureAwait(false);

            var expectedPath = new IGraphNode[]
            {
                map.Nodes.SelectByHexCoords(1, 1), map.Nodes.SelectByHexCoords(2, 2),
                map.Nodes.SelectByHexCoords(2, 3), map.Nodes.SelectByHexCoords(3, 4),
                map.Nodes.SelectByHexCoords(3, 5), map.Nodes.SelectByHexCoords(4, 6)
            };

            var context = CreatePathFindingContext(map);

            var astar = new AStar(context, expectedPath.First(), expectedPath.Last());

            // ACT
            var factState = astar.Run();

            // ASSERT

            factState.Should().Be(State.GoalFound);

            var factPath = astar.GetPath();
            factPath.Should().BeEquivalentTo(expectedPath);
        }

        /// <summary>
        /// Тест проверяет корректность обхода соседей при выборе пути.
        /// Обход соседей должен начинаться с левого и идти по часовой стрелке.
        /// </summary>
        [Test]
        public async Task Run_CheckNeighborBypass_ExpectedPath()
        {
            // ARRAGE
            var map = await CreateGridOpenMapAsync().ConfigureAwait(false);

            var expectedPath = new IGraphNode[]
            {
                map.Nodes.SelectByHexCoords(1, 1), map.Nodes.SelectByHexCoords(2, 2),
                map.Nodes.SelectByHexCoords(2, 3), map.Nodes.SelectByHexCoords(3, 3),
                map.Nodes.SelectByHexCoords(4, 3), map.Nodes.SelectByHexCoords(5, 3)
            };


            var context = CreatePathFindingContext(map);

            var astar = new AStar(context, expectedPath.First(), expectedPath.Last());

            // ACT
            var factState = astar.Run();

            // ASSERT

            factState.Should().Be(State.GoalFound);

            var factPath = astar.GetPath();
            factPath.Should().BeEquivalentTo(expectedPath);
        }

        /// <summary>
        /// Тест проверяет, что в карте шестиугольников выполняется корректный поиск пути
        /// с учётом припятсвий.
        /// </summary>
        [Test]
        [Category("integrational")]
        public void Run_HexMapWithObstacle_FoundExpectedPath()
        {
            var hexMap = new HexMap(10);
            hexMap.AddNode(new HexNode(0, 0));
            // Узел 1, 0 отсутствует, т.к. занят препятсвием.
            hexMap.AddNode(new HexNode(2, 0));

            hexMap.AddNode(new HexNode(0, 1));
            hexMap.AddNode(new HexNode(1, 1));
            hexMap.AddNode(new HexNode(2, 1));

            var context = CreatePathFindingContext(hexMap);

            var startNode = hexMap.Nodes.SelectByHexCoords(0, 0);
            var finishNode = hexMap.Nodes.SelectByHexCoords(2, 0);

            var expectedPath = new[]
            {
                hexMap.Nodes.SelectByHexCoords(0, 0), hexMap.Nodes.SelectByHexCoords(0, 1),
                hexMap.Nodes.SelectByHexCoords(1, 1), hexMap.Nodes.SelectByHexCoords(2, 0)
            };

            var astar = new AStar(context, startNode, finishNode);

            // ACT
            var factState = astar.Run();

            // ASSERT
            factState.Should().Be(State.GoalFound);

            var factPath = astar.GetPath();
            factPath.Should().BeEquivalentTo(expectedPath);
        }

        private static IAstarContext CreatePathFindingContext(ISpatialGraph graph)
        {
            var contextMock = new Mock<IAstarContext>();
            var context = contextMock.Object;
            contextMock.Setup(x => x.GetNext(It.IsAny<IGraphNode>()))
                .Returns<IGraphNode>(node => graph.GetNext(node));
            contextMock.Setup(x => x.GetDistanceBetween(It.IsAny<IGraphNode>(), It.IsAny<IGraphNode>()))
                .Returns<IGraphNode, IGraphNode>((current, target) => graph.DistanceBetween(current, target));

            return context;
        }

        private static IAstarContext CreatePathFindingContext(HexMap hexMap)
        {
            var contextMock = new Mock<IAstarContext>();
            var context = contextMock.Object;
            contextMock.Setup(x => x.GetNext(It.IsAny<IGraphNode>()))
                .Returns<IGraphNode>(node => hexMap.GetNext(node).Cast<HexNode>());
            contextMock.Setup(x => x.GetDistanceBetween(It.IsAny<IGraphNode>(), It.IsAny<IGraphNode>()))
                .Returns<IGraphNode, IGraphNode>((current, target) => hexMap.DistanceBetween(current, target));

            return context;
        }

        /// <summary>
        /// Создаёт открытую карту без препятствий.
        /// </summary>
        /// <returns></returns>
        private static async Task<IMap> CreateGridOpenMapAsync()
        {
            return await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);
        }
    }
}