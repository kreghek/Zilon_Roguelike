using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tests.Tactics.Behaviour.Bots
{
    [TestFixture]
    public class MonsterActorTaskSourceTests
    {
        [Test]
        public void GetActorTasksTest()
        {
            // ARRANGE

            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Owner).Returns(player);
            var actor = actorMock.Object;

            var actorListInner = new List<IActor> { actor };
            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.SetupGet(x => x.Actors).Returns(actorListInner);
            var actorManager = actorManagerMock.Object;

            var map = new TestGridGenMap();

            actorMock.SetupGet(x => x.Node).Returns(map.Nodes.Cast<HexNode>().SelectBy(1, 1));

            var routePoints = new IMapNode[] {
                map.Nodes.Cast<HexNode>().SelectBy(1,1),
                map.Nodes.Cast<HexNode>().SelectBy(9,9)
            };
            var routeMock = new Mock<IPatrolRoute>();
            routeMock.SetupGet(x => x.Points).Returns(routePoints);
            var route = routeMock.Object;

            var patrolRoutes = new Dictionary<IActor, IPatrolRoute>
            {
                { actor, route }
            };

            var decisionSourceMock = new Mock<IDecisionSource>();
            var decisionSource = decisionSourceMock.Object;

            var taskSource = new MonsterActorTaskSource(player, patrolRoutes, decisionSource);



            // ACT
            var tasks = taskSource.GetActorTasks(map, actorManager);



            // ASSERT
            tasks[0].Should().BeOfType<MoveTask>();
        }
    }
}