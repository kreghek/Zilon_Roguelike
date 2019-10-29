using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Tactics.Behaviour
{
    [TestFixture][Parallelizable(ParallelScope.All)]
    public class MoveTaskTests
    {
        /// <summary>
        /// Тест проверяет, то задача на перемещение за несколько итераций
        /// перемещает актёра в целевой узел. В конце, на последней итерации,
        /// когда актёр достиг цели, должна отмечаться, как заверщённая.
        /// </summary>
        [Test]
        public async System.Threading.Tasks.Task ExecuteTest_OpenGridMap_ActorReachPointAndTaskCompleteAsync()
        {
            // ARRANGE
            var map = await SquareMapFactory.CreateAsync(10);

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);

            var expectedPath = new[] {
                map.Nodes.Cast<HexNode>().SelectBy(2, 3),
                map.Nodes.Cast<HexNode>().SelectBy(2, 4),
                finishNode
            };

            var actor = CreateActor(map, startNode);

            var task = new MoveTask(actor, finishNode, map);


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
            var person = personMock.Object;

            IMapNode currentNode = startNode;
            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(() => currentNode);
            actorMock.Setup(x => x.MoveToNode(It.IsAny<IMapNode>()))
                .Callback<IMapNode>(node => currentNode = node);
            // ReSharper disable once UnusedVariable
            var actor = actorMock.Object;

            // Исправить и убрать отключение инспекции для actor.

            var actor2 = new Actor(person, player, startNode);
            map.HoldNode(startNode, actor2);
            return actor2;
        }

        /// <summary>
        /// Тест проверяет, что задача на перемещение учитывает стены.
        /// Актёр должен идти по пути, огибажщем стены.
        /// </summary>
        [Test]
        public async System.Threading.Tasks.Task ExecuteTest_MapWithWalls_ActorAvoidWallsAsync()
        {
            // ARRANGE

            var map = await SquareMapFactory.CreateAsync(10);
            map.RemoveEdge(3, 3, 3, 4);
            map.RemoveEdge(3, 3, 2, 3);

            var expectedPath = new IMapNode[] {
                map.Nodes.Cast<HexNode>().SelectBy(4,4),
                map.Nodes.Cast<HexNode>().SelectBy(3,4),
                map.Nodes.Cast<HexNode>().SelectBy(2,4),
                map.Nodes.Cast<HexNode>().SelectBy(1,5),
            };

            var startNode = expectedPath.First();
            var finishNode = expectedPath.Last();


            var actor = CreateActor(map, (HexNode)startNode);

            var task = new MoveTask(actor, finishNode, map);


            // ACT
            for (var step = 1; step < expectedPath.Length; step++)
            {
                task.Execute();


                // ASSERT
                actor.Node.Should().Be(expectedPath[step]);
            }
        }

        /// <summary>
        /// Тест проверяет, что задача заканчивается, когда актёр доходит до крайнего узла найденного маршрута.
        /// </summary>
        [Test]
        public async System.Threading.Tasks.Task ExecuteTest_FindingPathAndMove_IsCompleteTrueAsync()
        {
            // ARRANGE

            var map = await SquareMapFactory.CreateAsync(10);

            var expectedPath = new IMapNode[] {
                map.Nodes.Cast<HexNode>().SelectBy(4,4),
                map.Nodes.Cast<HexNode>().SelectBy(3,4),
                map.Nodes.Cast<HexNode>().SelectBy(2,4),
                map.Nodes.Cast<HexNode>().SelectBy(1,5),
            };

            var startNode = expectedPath.First();
            var finishNode = expectedPath.Last();


            var actor = CreateActor(map, (HexNode)startNode);

            var task = new MoveTask(actor, finishNode, map);



            // ACT
            for (var step = 1; step < expectedPath.Length; step++)
            {
                task.Execute();
            }



            // ASSERT
            task.IsComplete.Should().BeTrue();
        }
    }
}