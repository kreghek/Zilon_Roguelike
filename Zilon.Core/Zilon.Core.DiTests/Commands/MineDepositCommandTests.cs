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
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Commands;
using Zilon.Core.Tests.Common;

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
            humanTaskSourceMock.Verify(x => x.Intent(It.IsAny<IIntention>(), It.IsAny<IActor>()));
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<ISectorUiState> playerStateMock)
        {
            Container.AddSingleton<MineDepositCommand>();
            Container.AddSingleton(Mock.Of<IMineDepositMethodRandomSource>());

            var depositObjectMock = new Mock<IStaticObject>();
            var depoNode = testMap.Nodes.SelectByHexCoords(1, 0);
            depositObjectMock.SetupGet(x => x.Node).Returns(depoNode);

            var depositModuleMock = new Mock<IPropDepositModule>();
            depositModuleMock.Setup(x => x.GetToolTags()).Returns(Array.Empty<string>());
            depositObjectMock.Setup(x => x.GetModule<IPropDepositModule>(nameof(IPropDepositModule)))
                .Returns(depositModuleMock.Object);
            depositObjectMock.Setup(x => x.HasModule(nameof(IPropDepositModule))).Returns(true);

            var targetVm = Mock.Of<IContainerViewModel>(x => x.StaticObject == depositObjectMock.Object);
            playerStateMock.SetupProperty(x => x.SelectedViewModel, targetVm);
        }
    }
}