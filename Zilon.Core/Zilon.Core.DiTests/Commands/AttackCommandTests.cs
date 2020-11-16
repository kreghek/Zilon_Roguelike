using System;
using System.Linq;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture]
    public class AttackCommandTests : CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно атаковать, если не мешают стены.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<AttackCommand>();

            // ACT
            var canExecute = command.CanExecute();

            // ASSERT
            canExecute.Should()
                      .Be(true);
        }

        /// <summary>
        /// Тест проверяет, что при выполнении команды корректно фисируется намерение игрока на атаку.
        /// </summary>
        [Test]
        public void Execute_CanAttack_AttackIntended()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<AttackCommand>();
            var humanTaskSourceMock =
                ServiceProvider.GetRequiredService<Mock<IHumanActorTaskSource<ISectorTaskSourceContext>>>();
            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();

            // ACT
            command.Execute();

            // ASSERT
            var target = ((IActorViewModel)playerState.SelectedViewModel).Actor;

            humanTaskSourceMock.Verify(x => x.Intent(It.Is<IIntention>(intention =>
                CheckAttackIntention(intention, playerState, target)
            ), It.IsAny<IActor>()));
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

            var targetMock = new Mock<IActor>();
            var targetNode = testMap.Nodes.SelectByHexCoords(2, 0);
            targetMock.SetupGet(x => x.Node)
                      .Returns(targetNode);
            var target = targetMock.Object;

            var targetVmMock = new Mock<IActorViewModel>();
            targetVmMock.SetupProperty(x => x.Actor, target);
            var targetVm = targetVmMock.Object;
            playerStateMock.SetupProperty(x => x.SelectedViewModel, targetVm);

            Container.AddSingleton<AttackCommand>();
        }

        private static bool CheckAttackIntention(IIntention intention, ISectorUiState playerState, IActor target)
        {
            var attackIntention = (Intention<AttackTask>)intention;
            var attackTask = attackIntention.TaskFactory(playerState.ActiveActor.Actor);
            return attackTask.Target == target;
        }
    }
}