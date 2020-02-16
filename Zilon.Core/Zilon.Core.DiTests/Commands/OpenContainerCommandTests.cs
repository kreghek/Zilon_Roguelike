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
    public class OpenContainerCommandTests : CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно атаковать, если не мешают стены.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<OpenContainerCommand>();

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
            var command = ServiceProvider.GetRequiredService<OpenContainerCommand>();
            var humanTaskSourceMock = ServiceProvider.GetRequiredService<Mock<IHumanActorTaskSource>>();

            // ACT
            command.Execute();

            // ASSERT
            humanTaskSourceMock.Verify(x => x.Intent(It.IsAny<IIntention>()));
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<ISectorUiState> playerStateMock)
        {
            var targetMock = new Mock<IPropContainer>();
            var targetNode = testMap.Nodes.OfType<HexNode>().SelectBy(2, 0);
            targetMock.SetupGet(x => x.Node).Returns(targetNode);
            var target = targetMock.Object;

            var targetVmMock = new Mock<IContainerViewModel>();
            targetVmMock.SetupProperty(x => x.Container, target);
            var targetVm = targetVmMock.Object;

            playerStateMock.SetupProperty(x => x.HoverViewModel, targetVm);

            Container.AddSingleton<OpenContainerCommand>();
        }
    }
}