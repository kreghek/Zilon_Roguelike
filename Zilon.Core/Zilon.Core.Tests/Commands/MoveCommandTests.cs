using System.Linq;

using FluentAssertions;

using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture()]
    public class MoveCommandTests: CommandTestBase
    {
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
            var command = Container.GetInstance<MoveCommand>();
            var humanTaskSourceMock = Container.GetInstance<Mock<IHumanActorTaskSource>>();
            var playerState = Container.GetInstance<IPlayerState>();



            // ACT
            command.Execute();


            // ASSERT
            var target = ((IMapNodeViewModel)playerState.HoverViewModel).Node;
            humanTaskSourceMock.Verify(x => x.Intent(It.Is<MoveIntention>(intention => intention.TargetNode == target)));
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<IPlayerState> playerStateMock)
        {
            var targetNode = testMap.Nodes.OfType<HexNode>().SelectBy(1, 0);
            var targetVmMock = new Mock<IMapNodeViewModel>();
            targetVmMock.SetupProperty(x => x.Node, targetNode);
            var targetVm = targetVmMock.Object;

            playerStateMock.SetupProperty(x => x.HoverViewModel, targetVm);

            Container.Register<MoveCommand>(new PerContainerLifetime());
        }
    }
}