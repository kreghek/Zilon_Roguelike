using FluentAssertions;

using LightInject;

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
    public class PropTrasferCommandTests: CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно использовать экипировку.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = Container.GetInstance<PropTransferCommand>();



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
            var command = Container.GetInstance<PropTransferCommand>();
            var humanTaskSourceMock = Container.GetInstance<Mock<IHumanActorTaskSource>>();



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

            Container.Register(factory => transferMachine, new PerContainerLifetime());
            Container.Register<PropTransferCommand>(new PerContainerLifetime());
        }

        private IPropStore CreateStore()
        {
            var storeMock = new Mock<IPropStore>();
            return storeMock.Object;
        }
    }
}