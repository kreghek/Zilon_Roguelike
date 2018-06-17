using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Tactics.Spatial.PathFinding.TestCases;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    [TestFixture]
    public class MoveTaskTests
    {
        /// <summary>
        /// Тест проверяет, то задача на перемещение за несколько итераций
        /// перемещает актёра в целевой узел. В конце, на последней итерации,
        /// когда актёр достиг цели, должна отмечаться, как заверщённая.
        /// </summary>
        [Test]
        public void ExecuteTest_OpenGridMap_ActorReachPointAndTaskComplete()
        {
            // ARRANGE
            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);

            var expectedPath = new[] {
                map.Nodes.Cast<HexNode>().SelectBy(2, 3),
                map.Nodes.Cast<HexNode>().SelectBy(2, 4),
                finishNode
            };

            var actor = new Actor(new Person(), startNode);

            var task = new MoveTask(actor, finishNode, map);


            // ACT
            for (var step = 1; step <= 3; step++)
            {
                task.Execute();



                // ASSERT
                if (step < 3)
                {
                    task.IsComplete.Should().Be(false);
                }
                else
                {
                    task.IsComplete.Should().Be(true);
                }

                actor.Node.Should().Be(expectedPath[step - 1]);
            }



            // ASSERT

            task.IsComplete.Should().Be(true);
            actor.Node.Should().Be(finishNode);
        }

        /// <summary>
        /// Тест проверяет, что задача на перемещение учитывает стены.
        /// Актёр должен идти по пути, огибажщем стены.
        /// </summary>
        [Test, TestCaseSource(typeof(HexPathTestCaseSource), nameof(HexPathTestCaseSource.HexTestCases))]
        public void ExecuteTest_MapWithWalls_ActorAvoidWalls(List<IMapNode> nodes, List<IEdge> edges, IMapNode[] expectedPath)
        {
            // ARRANGE

            var mapMock = new Mock<IMap>();

            mapMock.SetupProperty(x => x.Nodes, nodes);
            mapMock.SetupProperty(x => x.Edges, edges);

            var map = mapMock.Object;

            var startNode = expectedPath.First();
            var finishNode = expectedPath.Last();


            var actor = new Actor(new Person(), (HexNode)startNode);

            var task = new MoveTask(actor, finishNode, map);


            // ACT
            for (var step = 1; step < expectedPath.Length; step++)
            {
                task.Execute();


                // ASSERT
                actor.Node.Should().Be(expectedPath[step]);
            }
        }
    }
}