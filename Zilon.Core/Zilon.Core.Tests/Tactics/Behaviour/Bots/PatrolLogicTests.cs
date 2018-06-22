using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tactics.Behaviour.Bots.Tests
{
    [TestFixture()]
    public class PatrolLogicTests
    {
        [Test()]
        public void GetCurrentTask_Bypassing_ActorWalkThroughRount()
        {
            // ARRANGE
            var map = new TestGridGenMap();

            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.Player).Returns(player);
            var person = personMock.Object;

            IMapNode factActorNode = map.Nodes.OfType<HexNode>().SelectBy(1, 1);
            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(factActorNode);
            actorMock.Setup(x => x.MoveToNode(It.IsAny<IMapNode>()))
                .Callback<IMapNode>(node => factActorNode = node);
            actorMock.SetupGet(x => x.Person).Returns(person);
            var actor = actorMock.Object;

            var patrolRouteMock = new Mock<IPatrolRoute>();
            var routePoints = new IMapNode[] {
                map.Nodes.OfType<HexNode>().SelectBy(1, 1),
                map.Nodes.OfType<HexNode>().SelectBy(5, 3),
                map.Nodes.OfType<HexNode>().SelectBy(3, 5)
            };
            patrolRouteMock.SetupGet(x => x.Points).Returns(routePoints);
            var patrolRoute = patrolRouteMock.Object;

            var actors = new List<IActor> { actor };
            var actorListMock = new Mock<IActorList>();
            actorListMock.SetupGet(x => x.Actors).Returns(actors);
            var actorList = actorListMock.Object;


            const int expectedIdleDuration = 1;
            var decisionSourceMock = new Mock<IDecisionSource>();
            decisionSourceMock.Setup(x => x.SelectIdleDuration(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(expectedIdleDuration);
            var decisionSource = decisionSourceMock.Object;

            var expectedActorPositions = new IMapNode[] {
                map.Nodes.OfType<HexNode>().SelectBy(2, 2),
                map.Nodes.OfType<HexNode>().SelectBy(2, 3),
                map.Nodes.OfType<HexNode>().SelectBy(3, 3),
                map.Nodes.OfType<HexNode>().SelectBy(4, 3),
                map.Nodes.OfType<HexNode>().SelectBy(5, 3),

                map.Nodes.OfType<HexNode>().SelectBy(5, 3),

                map.Nodes.OfType<HexNode>().SelectBy(4, 3),
                map.Nodes.OfType<HexNode>().SelectBy(4, 4),
                map.Nodes.OfType<HexNode>().SelectBy(3, 5),

                map.Nodes.OfType<HexNode>().SelectBy(3, 5),

                map.Nodes.OfType<HexNode>().SelectBy(3, 4),
                map.Nodes.OfType<HexNode>().SelectBy(2, 3),
                map.Nodes.OfType<HexNode>().SelectBy(2, 2),
                map.Nodes.OfType<HexNode>().SelectBy(1, 1),

                map.Nodes.OfType<HexNode>().SelectBy(1, 1),
            };


            var logic = new PatrolLogic(actor, patrolRoute, map, actorList, decisionSource);



            // ACT
            for (var round = 0; round < expectedActorPositions.Count() + 1; round++)
            {
                var task = logic.GetCurrentTask();


                // ASSERT
                task.Should().NotBeNull();
                switch (round)
                {
                    case 6:
                    case 10:
                    case 15:
                        task.Should().BeOfType<IdleTask>();
                        break;

                    default:
                        task.Should().BeOfType<MoveTask>();
                        break;
                }

                task.Execute();

                if (round < expectedActorPositions.Count())
                {
                    factActorNode.Should().Be(expectedActorPositions[round]);
                }
                else
                {
                    factActorNode.Should().Be(expectedActorPositions[0]);
                }
            }
            
        }
    }
}