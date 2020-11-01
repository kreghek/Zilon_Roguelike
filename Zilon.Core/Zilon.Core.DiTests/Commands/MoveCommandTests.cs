using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture]
    public class MoveCommandTests : CommandTestBase
    {
        private List<IActor> _actorList;

        /// <summary>
        ///     Тест проверяет, что можно перемещаться в пустые узлы карты.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<MoveCommand>();

            // ACT
            var canExecute = command.CanExecute();

            // ASSERT
            canExecute.Should().Be(true);
        }

        /// <summary>
        ///     Тест проверяет, что при выполнении команды корректно фисируется намерение игрока на атаку.
        /// </summary>
        [Test]
        public void ExecuteTest()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<MoveCommand>();
            var humanTaskSourceMock =
                ServiceProvider.GetRequiredService<Mock<IHumanActorTaskSource<ISectorTaskSourceContext>>>();
            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();

            // ACT
            command.Execute();

            // ASSERT
            HexNode target = ((IMapNodeViewModel)playerState.HoverViewModel).Node;
            humanTaskSourceMock.Verify(x => x.Intent(It.Is<MoveIntention>(intention => intention.TargetNode == target),
                It.IsAny<IActor>()));
        }

        /// <summary>
        ///     Тест проверяет, что автоперемещение работает, если в зоне видимости нет монстров.
        /// </summary>
        [Test]
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
            Justification = "Naming convention for tests")]
        public void CanRepeate_NoMonsters_ReturnsTrue()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<MoveCommand>();

            // ACT
            var canRepeat = command.CanRepeat();

            // ASSERT
            canRepeat.Should().BeTrue();
        }

        /// <summary>
        ///     Тест проверяет, что автоперемещение не работает, если в зоне видимости монстр.
        /// </summary>
        [Test]
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
            Justification = "Naming convention for tests")]
        public void CanRepeate_MonsterInSign_ReturnsFalse()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<MoveCommand>();
            var player = ServiceProvider.GetRequiredService<IPlayer>();

            var monsterMock = new Mock<IActor>();

            var monsterNode = player.SectorNode.Sector.Map.Nodes.SelectByHexCoords(0, 2);
            monsterMock.SetupGet(x => x.Node).Returns(monsterNode);

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.Fraction).Returns(Fractions.MonsterFraction);
            var person = personMock.Object;
            monsterMock.SetupGet(x => x.Person).Returns(person);

            var monster = monsterMock.Object;
            _actorList.Add(monster);

            // ACT
            var canRepeat = command.CanRepeat();

            // ASSERT
            canRepeat.Should().BeFalse();
        }

        /// <summary>
        ///     Тест проверяет, что автоперемещение работает, если монстр далеко.
        /// </summary>
        [Test]
        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
            Justification = "Naming convention for tests")]
        public void CanRepeate_MonsterNotInSign_ReturnsTrue()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<MoveCommand>();
            var player = ServiceProvider.GetRequiredService<IPlayer>();

            var monsterMock = new Mock<IActor>();

            var monsterNode = player.SectorNode.Sector.Map.Nodes.SelectByHexCoords(0, 6);
            monsterMock.SetupGet(x => x.Node).Returns(monsterNode);

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.Fraction).Returns(Fractions.MonsterFraction);
            var person = personMock.Object;
            monsterMock.SetupGet(x => x.Person).Returns(person);

            var monster = monsterMock.Object;
            _actorList.Add(monster);

            // ACT
            var canRepeat = command.CanRepeat();

            // ASSERT
            canRepeat.Should().BeTrue();
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<ISectorUiState> playerStateMock)
        {
            if (testMap is null)
            {
                throw new ArgumentNullException(nameof(testMap));
            }

            if (playerStateMock is null)
            {
                throw new ArgumentNullException(nameof(playerStateMock));
            }

            var targetNode = testMap.Nodes.SelectByHexCoords(1, 0);
            var targetVmMock = new Mock<IMapNodeViewModel>();
            targetVmMock.SetupProperty(x => x.Node, targetNode);
            var targetVm = targetVmMock.Object;

            playerStateMock.SetupProperty(x => x.HoverViewModel, targetVm);
            playerStateMock.SetupProperty(x => x.SelectedViewModel, targetVm);

            Container.AddSingleton<MoveCommand>();

            _actorList = new List<IActor> {playerStateMock.Object.ActiveActor.Actor};
            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.Setup(x => x.Items).Returns(_actorList);
            var actorManager = actorManagerMock.Object;

            Container.AddSingleton(factory => actorManager);
        }
    }
}