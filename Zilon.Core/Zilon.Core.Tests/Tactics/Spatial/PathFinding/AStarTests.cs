using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tactics.Spatial.PathFinding;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Tactics.Spatial.PathFinding
{
    [TestFixture][Parallelizable(ParallelScope.All)]
    public class AStarTests
    {
        /// <summary>
        /// Тест проверяет корректность алгоритма в сетке шестиугольников.
        /// </summary>
        [Test]
        public void Run_ShortGraph_PathFound()
        {
            // ARRAGE
            var expectedPath = new List<IMapNode>();

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

            var context = CreatePathFindingContext();

            var astar = new AStar(map, context, expectedPath.First(), expectedPath.Last());



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
            var map = await CreateGridOpenMapAsync();

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
            var map = await CreateGridOpenMapAsync();

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
            hexMap.AddNode(new HexNode(1, 0, isObstacle: true));
            hexMap.AddNode(new HexNode(2, 0));

            hexMap.AddNode(new HexNode(0, 1));
            hexMap.AddNode(new HexNode(1, 1));
            hexMap.AddNode(new HexNode(2, 1));

            var context = CreatePathFindingContext();

            var startNode = hexMap.Nodes.Cast<HexNode>().SelectBy(0, 0);
            var finishNode = hexMap.Nodes.Cast<HexNode>().SelectBy(2, 0);

            var expectedPath = new[] {
                hexMap.Nodes.Cast<HexNode>().SelectBy(0, 0),
                hexMap.Nodes.Cast<HexNode>().SelectBy(0, 1),
                hexMap.Nodes.Cast<HexNode>().SelectBy(1, 1),
                hexMap.Nodes.Cast<HexNode>().SelectBy(2, 0)
            };


            var astar = new AStar(hexMap, context, startNode, finishNode);



            // ACT
            var factState = astar.Run();



            // ASSERT
            factState.Should().Be(State.GoalFound);

            var factPath = astar.GetPath();
            factPath.Should().BeEquivalentTo(expectedPath);
        }

        private static IPathFindingContext CreatePathFindingContext()
        {
            var contextMock = new Mock<IPathFindingContext>();
            var context = contextMock.Object;

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;
            contextMock.SetupGet(x => x.Actor).Returns(actor);

            return context;
        }

        /// <summary>
        /// Создаёт открытую карту без препятствий.
        /// </summary>
        /// <returns></returns>
        private static async Task<IMap> CreateGridOpenMapAsync()
        {
            return await SquareMapFactory.CreateAsync(10);
        }
    }
}