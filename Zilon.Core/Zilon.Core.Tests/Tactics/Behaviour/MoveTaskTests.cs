using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Tactics.Behaviour
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MoveTaskTests
    {
        /// <summary>
        /// Тест проверяет, что задача заканчивается, когда актёр доходит до крайнего узла найденного маршрута.
        /// </summary>
        [Test]
        public async Task ExecuteTest_FindingPathAndMove_IsCompleteTrueAsync()
        {
            // ARRANGE

            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

            var expectedPath = CreateExpectedPath(map);

            var startNode = expectedPath.First();
            var finishNode = expectedPath.Last();

            var actor = CreateActor(map, (HexNode)startNode);

            var contextMock = new Mock<IActorTaskContext>();
            var context = contextMock.Object;

            var task = new MoveTask(actor, context, finishNode, map);

            // ACT
            for (var step = 1; step < expectedPath.Length; step++)
            {
                task.Execute();
            }

            // ASSERT
            task.IsComplete.Should().BeTrue();
        }

        /// <summary>
        /// Тест проверяет, что задача на перемещение учитывает стены.
        /// Актёр должен идти по пути, огибажщем стены.
        /// </summary>
        [Test]
        public async Task ExecuteTest_MapWithWalls_ActorAvoidWallsAsync()
        {
            // ARRANGE

            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);
            map.RemoveEdge(3, 3, 3, 4);
            map.RemoveEdge(3, 3, 2, 3);

            var expectedPath = CreateExpectedPath(map);

            var startNode = expectedPath.First();
            var finishNode = expectedPath.Last();

            var actor = CreateActor(map, (HexNode)startNode);

            var contextMock = new Mock<IActorTaskContext>();
            var context = contextMock.Object;

            var task = new MoveTask(actor, context, finishNode, map);

            // ACT
            for (var step = 1; step < expectedPath.Length; step++)
            {
                task.Execute();

                // ASSERT
                actor.Node.Should().Be(expectedPath[step]);
            }
        }

        /// <summary>
        /// Тест проверяет, то задача на перемещение за несколько итераций
        /// перемещает актёра в целевой узел. В конце, на последней итерации,
        /// когда актёр достиг цели, должна отмечаться, как заверщённая.
        /// </summary>
        [Test]
        public async Task ExecuteTest_OpenGridMap_ActorReachPointAndTaskCompleteAsync()
        {
            // ARRANGE
            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

            var startNode = map.Nodes.SelectByHexCoords(3, 3);
            var finishNode = map.Nodes.SelectByHexCoords(1, 5);

            var expectedPath = new[]
            {
                map.Nodes.SelectByHexCoords(2, 3),
                map.Nodes.SelectByHexCoords(2, 4),
                finishNode
            };

            var actor = CreateActor(map, startNode);

            var contextMock = new Mock<IActorTaskContext>();
            var context = contextMock.Object;

            var task = new MoveTask(actor, context, finishNode, map);

            // ACT
            for (var step = 1; step <= expectedPath.Length; step++)
            {
                task.Execute();

                // ASSERT
                var expectedIsComplete = step >= 3;
                task.IsComplete.Should().Be(expectedIsComplete);

                actor.Node.Should().Be(expectedPath[step - 1]);
            }

            // ASSERT

            task.IsComplete.Should().Be(true);
            actor.Node.Should().Be(finishNode);
        }

        private static IActor CreateActor(IMap map, HexNode startNode)
        {
            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.PhysicalSize).Returns(PhysicalSizePattern.Size1);
            var person = personMock.Object;

            var taskSourceMock = new Mock<IActorTaskSource<ISectorTaskSourceContext>>();
            var taskSource = taskSourceMock.Object;

            var actor = new Actor(person, taskSource, startNode);
            map.HoldNode(startNode, actor);
            return actor;
        }

        private static IGraphNode[] CreateExpectedPath(ISectorMap map)
        {
            return new IGraphNode[]
            {
                map.Nodes.SelectByHexCoords(4, 4),
                map.Nodes.SelectByHexCoords(3, 4),
                map.Nodes.SelectByHexCoords(2, 4),
                map.Nodes.SelectByHexCoords(1, 5)
            };
        }
    }
}