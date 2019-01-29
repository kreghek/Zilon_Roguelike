using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture()]
    public class MoveCommandTests: CommandTestBase
    {
        private List<IActor> _actorList;

        /// <summary>
        /// Тест проверяет, что можно перемещаться в пустые узлы карты.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = Container.GetInstance<MoveCommand>();



            // ACT
            var canExecute = command.CanExecute();


            // ASSERT
            canExecute.Should().Be(true);
        }

        /// <summary>
        /// Тест проверяет, что при выполнении команды корректно фисируется намерение игрока на атаку.
        /// </summary>
        [Test]
        public void ExecuteTest()
        {
            // ARRANGE
            var command = Container.GetInstance<MoveCommand>();
            var humanTaskSourceMock = Container.GetInstance<Mock<IHumanActorTaskSource>>();
            var playerState = Container.GetInstance<IPlayerState>();



            // ACT
            command.Execute();


            // ASSERT
            var target = ((IMapNodeViewModel)playerState.HoverViewModel).Node;
            humanTaskSourceMock.Verify(x => x.Intent(It.Is<MoveIntention>(intention => intention.TargetNode == target)));
        }

        /// <summary>
        /// Тест проверяет, что автоперемещение работает, если в зоне видимости нет монстров.
        /// </summary>
        [Test]
        public void CanRepeate_NoMonsters_ReturnsTrue()
        {
            // ARRANGE
            var command = Container.GetInstance<MoveCommand>();


            // ACT
            var canRepeat = command.CanRepeat();



            // ASSERT
            canRepeat.Should().BeTrue();
        }

        /// <summary>
        /// Тест проверяет, что автоперемещение не работает, если в зоне видимости монстр.
        /// </summary>
        [Test]
        public void CanRepeate_MonsterInSign_ReturnsFalse()
        {
            // ARRANGE
            var command = Container.GetInstance<MoveCommand>();
            var playerState = Container.GetInstance<IPlayerState>();
            var sectorManager = Container.GetInstance<ISectorManager>();


            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var monsterMock = new Mock<IActor>();
            monsterMock.Setup(x => x.Owner).Returns(player);

            var monsterNode = sectorManager.CurrentSector.Map.Nodes.OfType<HexNode>().SelectBy(0, 2);
            monsterMock.SetupGet(x => x.Node).Returns(monsterNode);

            var monster = monsterMock.Object;
            _actorList.Add(monster);



            // ACT
            var canRepeat = command.CanRepeat();



            // ASSERT
            canRepeat.Should().BeFalse();
        }

        /// <summary>
        /// Тест проверяет, что автоперемещение работает, если монстр далеко.
        /// </summary>
        [Test]
        public void CanRepeate_MonsterNotInSign_ReturnsTrue()
        {
            // ARRANGE
            var command = Container.GetInstance<MoveCommand>();
            var playerState = Container.GetInstance<IPlayerState>();
            var sectorManager = Container.GetInstance<ISectorManager>();


            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var monsterMock = new Mock<IActor>();
            monsterMock.Setup(x => x.Owner).Returns(player);

            var monsterNode = sectorManager.CurrentSector.Map.Nodes.OfType<HexNode>().SelectBy(0, 6);
            monsterMock.SetupGet(x => x.Node).Returns(monsterNode);

            var monster = monsterMock.Object;
            _actorList.Add(monster);



            // ACT
            var canRepeat = command.CanRepeat();



            // ASSERT
            canRepeat.Should().BeTrue();
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<IPlayerState> playerStateMock)
        {
            var targetNode = testMap.Nodes.OfType<HexNode>().SelectBy(1, 0);
            var targetVmMock = new Mock<IMapNodeViewModel>();
            targetVmMock.SetupProperty(x => x.Node, targetNode);
            var targetVm = targetVmMock.Object;

            playerStateMock.SetupProperty(x => x.HoverViewModel, targetVm);

            Container.Register<MoveCommand>(new PerContainerLifetime());

            _actorList = new List<IActor> { playerStateMock.Object.ActiveActor.Actor };
            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.Setup(x => x.Items).Returns(_actorList);
            var actorManager = actorManagerMock.Object;

            Container.Register(factory => actorManager, new PerContainerLifetime());
        }
    }
}