using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture]
    public class PropTrasferCommandTests : CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно использовать экипировку.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<PropTransferCommand>();

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
            var command = ServiceProvider.GetRequiredService<PropTransferCommand>();
            var humanTaskSourceMock = ServiceProvider.GetRequiredService<Mock<IHumanActorTaskSource>>();

            var transferMachine = ServiceProvider.GetRequiredService<PropTransferMachine>();
            command.TransferMachine = transferMachine;

            // ACT
            command.Execute();

            // ASSERT
            humanTaskSourceMock.Verify(x => x.Intent(It.IsAny<IIntention>()));
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<ISectorUiState> playerStateMock)
        {
            var inventory = CreateStore();
            var container = CreateStore();
            var transferMachine = new PropTransferMachine(inventory, container);

            Container.AddSingleton(factory => transferMachine);
            Container.AddSingleton<PropTransferCommand>();
        }

        private IPropStore CreateStore()
        {
            var storeMock = new Mock<IPropStore>();
            return storeMock.Object;
        }
    }
}