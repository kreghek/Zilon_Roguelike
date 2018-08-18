using System.Linq;

using FluentAssertions;

using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture]
    public class OpenContainerCommandTests
    {
        private ServiceContainer _container;


        /// <summary>
        /// Тест проверяет, что можно атаковать, если не мешают стены.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = _container.GetInstance<OpenContainerCommand>();



            // ACT
            var canExecute = command.CanExecute();


            // ASSERT
            canExecute.Should().Be(true);
        }

        /// <summary>
        /// Тест проверяет, что при выполнении команды корректно фисируется намерение игрока на атаку.
        /// </summary>
        [Test]
        [Ignore("Не рабочий. Некорректно проверят вызов Intent")]
        public void ExecuteTest()
        {
            var command = _container.GetInstance<OpenContainerCommand>();
            var humanTaskSourceMock = _container.GetInstance<Mock<IHumanActorTaskSource>>();
            var playerState = _container.GetInstance<IPlayerState>();



            // ACT
            command.Execute();



            // ASSERT
            var target = ((IContainerViewModel)playerState.HoverViewModel).Container;
            //humanTaskSourceMock.Verify(x => x.IntentOpenContainer(target, It.IsAny<IOpenContainerMethod>()));
            humanTaskSourceMock.Verify(x => x.Intent(null));
        }

        [SetUp]
        public void SetUp()
        {
            _container = new ServiceContainer();

            var testMap = new TestGridGenMap(3);

            var sectorMock = new Mock<ISector>();
            sectorMock.SetupGet(x => x.Map).Returns(testMap);
            var sector = sectorMock.Object;

            var sectorManagerMock = new Mock<ISectorManager>();
            sectorManagerMock.SetupProperty(x => x.CurrentSector, sector);
            var sectorManager = sectorManagerMock.Object;


            var actorMock = new Mock<IActor>();
            var actorNode = testMap.Nodes.OfType<HexNode>().SelectBy(0, 0);
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            var actor = actorMock.Object;

            var actorVmMock = new Mock<IActorViewModel>();
            actorVmMock.SetupProperty(x => x.Actor, actor);
            var actorVm = actorVmMock.Object;

            var targetMock = new Mock<IPropContainer>();
            var targetNode = testMap.Nodes.OfType<HexNode>().SelectBy(2, 0);
            targetMock.SetupGet(x => x.Node).Returns(targetNode);
            var target = targetMock.Object;

            var targetVmMock = new Mock<IContainerViewModel>();
            targetVmMock.SetupProperty(x => x.Container, target);
            var targetVm = targetVmMock.Object;

            var decisionSourceMock = new Mock<IDecisionSource>();
            var decisionSource = decisionSourceMock.Object;

            var humanTaskSourceMock = new Mock<IHumanActorTaskSource>();
            var humanTaskSource = humanTaskSourceMock.Object;

            var playerStateMock = new Mock<IPlayerState>();
            playerStateMock.SetupProperty(x => x.ActiveActor, actorVm);
            playerStateMock.SetupProperty(x => x.HoverViewModel, targetVm);
            playerStateMock.SetupProperty(x => x.TaskSource, humanTaskSource);
            var playerState = playerStateMock.Object;


            _container.Register<OpenContainerCommand>(new PerContainerLifetime());
            _container.Register(factory => sectorManager, new PerContainerLifetime());
            _container.Register(factory => playerState, new PerContainerLifetime());
            _container.Register(factory => humanTaskSourceMock, new PerContainerLifetime());
        }
    }
}