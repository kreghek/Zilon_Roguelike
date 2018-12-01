using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Tactics.Behaviour.Bots
{
    [TestFixture]
    public class MonsterActorTaskSourceTests
    {
        private ServiceContainer _container;
        private IMap _map;
        private List<IActor> _actorListInner;
        private IActor _testedActor;

        /// <summary>
        /// Тест проверяет, что источник команд возвращает задачи на перемещение,
        /// если для монстров заданы маршруты.
        /// </summary>
        [Test]
        public void GetActorTasks_PatrolsInClearField_ReturnsMoveTask()
        {
            // ARRANGE

            var taskSource = _container.GetInstance<MonsterActorTaskSource>();



            // ACT
            var tasks = taskSource.GetActorTasks(_testedActor);



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

            var taskSource = _container.GetInstance<MonsterActorTaskSource>();

            // Располагаем рядом игрока и бота
            CreateBotActor(3, 1);


            // ACT
            var tasks = taskSource.GetActorTasks(_testedActor);



            // ASSERT
            // бот должен начать двигаться к игроку.
            tasks[0].Should().BeOfType<MoveTask>();

            tasks[0].Execute();
            ((HexNode)_testedActor.Node).OffsetX.Should().Be(2);
            ((HexNode)_testedActor.Node).OffsetY.Should().Be(1);
        }

        /// <summary>
        /// Тест проверяет, что источник команд возвращает задачу на атаку,
        /// если патрульный стоит рядом и может атаковать.
        /// </summary>
        [Test]
        public void GetActorTasks_PatrolsTryToAttackEnemy_ReturnsAttackTask()
        {
            // ARRANGE

            var taskSource = _container.GetInstance<MonsterActorTaskSource>();

            // Располагаем рядом игрока и бота
            CreateBotActor(2, 1);



            // ACT
            var tasks = taskSource.GetActorTasks(_testedActor);



            // ASSERT
            // бот должен начать двигаться к игроку.
            tasks[0].Should().BeOfType<AttackTask>();

            Action act = () =>
            {
                tasks[0].Execute();
            };

            act.Should().NotThrow();
        }

        /// <summary>
        /// В стартовые настройки входит создание актёра, для которого назначен маршрут.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _container = new ServiceContainer();

            _map = new TestGridGenMap();

            var sectorMock = new Mock<ISector>();
            sectorMock.SetupGet(x => x.Map).Returns(_map);
            var sector = sectorMock.Object;

            var sectorManagerMock = new Mock<ISectorManager>();
            sectorManagerMock.SetupGet(x => x.CurrentSector).Returns(sector);
            var sectorManager = sectorManagerMock.Object;

            _actorListInner = new List<IActor>();

            _testedActor = CreateBotActor(1, 1);

            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.SetupGet(x => x.Items).Returns(_actorListInner);
            var actorManager = actorManagerMock.Object;

            var routePoints = new IMapNode[] {
                _map.Nodes.Cast<HexNode>().SelectBy(1,1),
                _map.Nodes.Cast<HexNode>().SelectBy(9,9)
            };
            var routeMock = new Mock<IPatrolRoute>();
            routeMock.SetupGet(x => x.Points).Returns(routePoints);
            var route = routeMock.Object;

            var patrolRoutes = new Dictionary<IActor, IPatrolRoute>
            {
                { _testedActor, route }
            };
            sectorMock.SetupGet(x => x.PatrolRoutes).Returns(patrolRoutes);

            var decisionSourceMock = new Mock<IDecisionSource>();
            var decisionSource = decisionSourceMock.Object;

            var tacticalActUsageServiceMock = new Mock<ITacticalActUsageService>();
            var tacticalActUsageService = tacticalActUsageServiceMock.Object;

            _container.Register(factory => sectorManager, new PerContainerLifetime());
            _container.Register(factory => (IBotPlayer)_testedActor.Owner, new PerContainerLifetime());
            _container.Register(factory => actorManager, new PerContainerLifetime());
            _container.Register(factory => tacticalActUsageService, new PerContainerLifetime());
            _container.Register(factory => decisionSource, new PerContainerLifetime());
            _container.Register<MonsterActorTaskSource>(new PerContainerLifetime());
        }

        private IActor CreateBotActor(int nodeX, int nodeY)
        {
            var playerMock = new Mock<IBotPlayer>();
            var player = playerMock.Object;
            var person = CreatePerson();

            var actorNode = _map.Nodes.Cast<HexNode>().SelectBy(nodeX, nodeY);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Owner).Returns(player);
            actorMock.SetupGet(x => x.Person).Returns(person);
            actorMock.SetupGet(x => x.Node).Returns(() => actorNode);
            actorMock.Setup(x => x.MoveToNode(It.IsAny<IMapNode>()))
                .Callback<IMapNode>((node) => actorNode = (HexNode)node);
            actorMock.Setup(x => x.CanBeDamaged()).Returns(true);

            var actor = actorMock.Object;

            _actorListInner.Add(actor);
            _map.HoldNode(actorNode, actor);

            return actor;
        }

        private static IPerson CreatePerson()
        {
            var tacticalActStatsScheme = new TestTacticalActStatsSubScheme();
            var tacticalActMock = new Mock<ITacticalAct>();
            tacticalActMock.SetupGet(x => x.Stats).Returns(tacticalActStatsScheme);
            var tacticalAct = tacticalActMock.Object;

            var tacticalCarrierMock = new Mock<ITacticalActCarrier>();
            tacticalCarrierMock.SetupProperty(x => x.Acts, new[] { tacticalAct });
            var tacticalCarrier = tacticalCarrierMock.Object;

            var survivalDataMock = new Mock<ISurvivalData>();
            var survivalData = survivalDataMock.Object;


            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.TacticalActCarrier).Returns(tacticalCarrier);
            personMock.SetupGet(x => x.Survival).Returns(survivalData);
            var person = personMock.Object;
            return person;
        }
    }
}