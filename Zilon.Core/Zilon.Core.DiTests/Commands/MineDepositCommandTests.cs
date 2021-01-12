using System;
using System.Collections.Generic;
using System.Text;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Commands.Sector;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Commands;

namespace Zilon.Core.DiTests.Commands
{
    [TestFixture]
    public sealed class MineDepositCommandTests : CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно атаковать, если не мешают стены.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<MineDepositCommand>();

            // ACT
            var canExecute = command.CanExecute();

            // ASSERT
            canExecute.Should().BeTrue();
        }

        /// <summary>
        /// Тест проверяет, что при выполнении команды корректно фисируется намерение игрока на атаку.
        /// </summary>
        [Test]
        public void ExecuteTest()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<MineDepositCommand>();
            var humanTaskSourceMock =
                ServiceProvider.GetRequiredService<Mock<IHumanActorTaskSource<ISectorTaskSourceContext>>>();

            // ACT
            command.Execute();

            // ASSERT
            humanTaskSourceMock.Verify(x => x.IntentAsync(It.IsAny<IIntention>(), It.IsAny<IActor>()));
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<ISectorUiState> playerStateMock)
        {
            Container.AddSingleton<MineDepositCommand>();
            Container.AddSingleton<IMineDepositMethodRandomSource>(Mock.Of<IMineDepositMethodRandomSource>());
        }
    }
}