using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Commands.Tests
{
    //TODO Добавить тест, который проверяет, что нельзя атаковать сквозь стены.
    [TestFixture]
    public class AttackCommandTests
    {
        /// <summary>
        /// Тест проверяет, что можно атаковать, если не мешают стены.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            //ASSERT
            var actorManagerMock = new Mock<IActorManager>();
            var actorManager = actorManagerMock.Object;

            var containerManagerMock = new Mock<IPropContainerManager>();
            var containerManager = containerManagerMock.Object;

            var testMap = new TestGridGenMap();
            var sector = new Sector(testMap, actorManager, containerManager);

            var sectorManagerMock = new Mock<ISectorManager>();
            sectorManagerMock.SetupProperty(x => x.CurrentSector, sector);
            var sectorManager = sectorManagerMock.Object;


            var actorMock = new Mock<IActor>();
            var actorNode = testMap.Nodes.OfType<HexNode>().SelectBy(0, 0);
            actorMock.SetupGet(x=>x.Node).Returns(actorNode);
            var actor = actorMock.Object;

            var actorVmMock = new Mock<IActorViewModel>();
            actorVmMock.SetupProperty(x => x.Actor, actor);
            var actorVm = actorVmMock.Object;

            var targetMock = new Mock<IActor>();
            var targetNode = testMap.Nodes.OfType<HexNode>().SelectBy(2, 0);
            targetMock.SetupGet(x => x.Node).Returns(targetNode);
            var target = targetMock.Object;

            var targetVmMock = new Mock<IActorViewModel>();
            targetVmMock.SetupProperty(x => x.Actor, target);
            var targetVm = targetVmMock.Object;

            var playerStateMock = new Mock<IPlayerState>();
            playerStateMock.SetupProperty(x => x.ActiveActor, actorVm);
            playerStateMock.SetupProperty(x => x.SelectedActor, targetVm);
            var playerState = playerStateMock.Object;

            var command = new AttackCommand(sectorManager, playerState);



            // ACT
            var canExecute = command.CanExecute();


            // ASSERT
            canExecute.Should().Be(true);
        }
    }
}