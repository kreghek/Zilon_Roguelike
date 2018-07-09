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
        /// <summary>
        /// Тест проверяет, что источник команд возвращает задачи на перемещение,
        /// если для монстров заданы маршруты.
        /// </summary>
        [Test]
        public void GetActorTasks_PatrolsInClearField_ReturnsMoveTask()
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

        /// <summary>
        /// Тест проверяет, что источник команд возвращает задачи на перемещение,
        /// если патрульный обнаружил противника.
        /// </summary>
        [Test]
        public void GetActorTasks_PatrolsTryToAttackEnemy_ReturnsMoveTask()
        {
            // ARRANGE

            // создание актёра бота
            var botPlayerMock = new Mock<IPlayer>();
            var botPlayer = botPlayerMock.Object;

            var botActorMock = new Mock<IActor>();
            botActorMock.SetupGet(x => x.Owner).Returns(botPlayer);
            var botActor = botActorMock.Object;

            // Создание актёра игрока
            var humanPlayerMock = new Mock<IPlayer>();
            var humanPlayer = humanPlayerMock.Object;

            var humanActorMock = new Mock<IActor>();
            humanActorMock.SetupGet(x => x.Owner).Returns(humanPlayer);
            var humanActor = humanActorMock.Object;

            var actorListInner = new List<IActor> { humanActor, botActor };
            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.SetupGet(x => x.Actors).Returns(actorListInner);
            var actorManager = actorManagerMock.Object;

            var map = new TestGridGenMap();

            // Располагаем рядом игрока и бота
            var botActorNode = map.Nodes.Cast<HexNode>().SelectBy(1, 1);
            humanActorMock.SetupGet(x => x.Node).Returns(map.Nodes.Cast<HexNode>().SelectBy(3, 1));
            botActorMock.SetupGet(x => x.Node).Returns(()=> botActorNode);

            var routePoints = new IMapNode[] {
                map.Nodes.Cast<HexNode>().SelectBy(1,1),
                map.Nodes.Cast<HexNode>().SelectBy(9,9)
            };
            var routeMock = new Mock<IPatrolRoute>();
            routeMock.SetupGet(x => x.Points).Returns(routePoints);
            var route = routeMock.Object;

            var patrolRoutes = new Dictionary<IActor, IPatrolRoute>
            {
                { botActor, route }
            };

            var decisionSourceMock = new Mock<IDecisionSource>();
            var decisionSource = decisionSourceMock.Object;

            var taskSource = new MonsterActorTaskSource(botPlayer, patrolRoutes, decisionSource);



            // ACT
            var tasks = taskSource.GetActorTasks(map, actorManager);



            // ASSERT
            // бот должен начать двигаться к игроку.
            tasks[0].Should().BeOfType<MoveTask>();

            tasks[0].Execute();
            botActorNode.OffsetX.Should().Be(2);
            botActorNode.OffsetY.Should().Be(1);
        }
    }
}