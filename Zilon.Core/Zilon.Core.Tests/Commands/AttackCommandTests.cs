using System.Linq;

using FluentAssertions;

using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Commands
{
    //TODO Добавить тест, который проверяет, что нельзя атаковать сквозь стены.
    [TestFixture]
    public class AttackCommandTests: CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно атаковать, если не мешают стены.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = _container.GetInstance<AttackCommand>();



            // ACT
            var canExecute = command.CanExecute();


            // ASSERT
            canExecute.Should().Be(true);
        }

        /// <summary>
        /// Тест проверяет, что при выполнении команды корректно фисируется намерение игрока на атаку.
        /// </summary>
        [Test]
        public void Execute_CanAttack_AttackIntended()
        {
            // ARRANGE
            var command = _container.GetInstance<AttackCommand>();
            var humanTaskSourceMock = _container.GetInstance<Mock<IHumanActorTaskSource>>();
            var playerState = _container.GetInstance<IPlayerState>();



            // ACT
            command.Execute();


            // ASSERT
            var target = ((IActorViewModel)playerState.HoverViewModel).Actor;

            humanTaskSourceMock.Verify(x => x.Intent(It.Is<IIntention>(intention =>
                CheckAttackIntention(intention, playerState, target)
            )));
        }

        private static bool CheckAttackIntention(IIntention intention, IPlayerState playerState, IActor target)
        {
            var attackIntention = (Intention<AttackTask>)intention;
            var attackTask = attackIntention.TaskFactory(playerState.ActiveActor.Actor);
            return attackTask.Target == target;
        }

        //[SetUp]
        //public void SetUp()
        //{
        //    _container = new ServiceContainer();

        //    var testMap = new TestGridGenMap(3);

        //    var sectorMock = new Mock<ISector>();
        //    sectorMock.SetupGet(x => x.Map).Returns(testMap);
        //    var sector = sectorMock.Object;

        //    var sectorManagerMock = new Mock<ISectorManager>();
        //    sectorManagerMock.SetupProperty(x => x.CurrentSector, sector);
        //    var sectorManager = sectorManagerMock.Object;

        //    var actMock = new Mock<ITacticalAct>();
        //    actMock.SetupGet(x => x.Stats).Returns(new TacticalActStatsSubScheme
        //    {
        //        Range = new Range<int>(1, 2)
        //    });
        //    var act = actMock.Object;

        //    var actCarrierMock = new Mock<ITacticalActCarrier>();
        //    actCarrierMock.SetupGet(x => x.Acts)
        //        .Returns(new[] { act });
        //    var actCarrier = actCarrierMock.Object;

        //    var personMock = new Mock<IPerson>();
        //    personMock.SetupGet(x => x.TacticalActCarrier).Returns(actCarrier);
        //    var person = personMock.Object;

        //    var actorMock = new Mock<IActor>();
        //    var actorNode = testMap.Nodes.OfType<HexNode>().SelectBy(0, 0);
        //    actorMock.SetupGet(x => x.Node).Returns(actorNode);
        //    actorMock.SetupGet(x => x.Person).Returns(person);
        //    var actor = actorMock.Object;

        //    var actorVmMock = new Mock<IActorViewModel>();
        //    actorVmMock.SetupProperty(x => x.Actor, actor);
        //    var actorVm = actorVmMock.Object;

        //    var targetMock = new Mock<IActor>();
        //    var targetNode = testMap.Nodes.OfType<HexNode>().SelectBy(2, 0);
        //    targetMock.SetupGet(x => x.Node).Returns(targetNode);
        //    var target = targetMock.Object;

        //    var targetVmMock = new Mock<IActorViewModel>();
        //    targetVmMock.SetupProperty(x => x.Actor, target);
        //    var targetVm = targetVmMock.Object;

        //    var humanTaskSourceMock = new Mock<IHumanActorTaskSource>();
        //    var humanTaskSource = humanTaskSourceMock.Object;

        //    var playerStateMock = new Mock<IPlayerState>();
        //    playerStateMock.SetupProperty(x => x.ActiveActor, actorVm);
            
        //    playerStateMock.SetupProperty(x => x.TaskSource, humanTaskSource);
        //    var playerState = playerStateMock.Object;

        //    var usageServiceMock = new Mock<ITacticalActUsageService>();
        //    var usageService = usageServiceMock.Object;

        //    var gameLoopMock = new Mock<IGameLoop>();
        //    var gameLoop = gameLoopMock.Object;


        //    _container.Register<AttackCommand>(new PerContainerLifetime());
        //    _container.Register(factory => sectorManager, new PerContainerLifetime());
        //    _container.Register(factory => playerState, new PerContainerLifetime());
        //    _container.Register(factory => humanTaskSourceMock, new PerContainerLifetime());
        //    _container.Register(factory => usageService, new PerContainerLifetime());
        //    _container.Register(factory => gameLoop, new PerContainerLifetime());
        //}

        protected override void RegisterSpecificServices(IMap testMap, Mock<IPlayerState> playerStateMock)
        {
            var targetMock = new Mock<IActor>();
            var targetNode = testMap.Nodes.OfType<HexNode>().SelectBy(2, 0);
            targetMock.SetupGet(x => x.Node).Returns(targetNode);
            var target = targetMock.Object;

            var targetVmMock = new Mock<IActorViewModel>();
            targetVmMock.SetupProperty(x => x.Actor, target);
            var targetVm = targetVmMock.Object;
            playerStateMock.SetupProperty(x => x.HoverViewModel, targetVm);

            _container.Register<AttackCommand>(new PerContainerLifetime());
        }
    }
}