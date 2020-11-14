using System;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture]
    public class UseSelfCommandTests : CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно использовать предмет, если есть информация об использовании.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<UseSelfCommand>();

            // ACT
            var canExecute = command.CanExecute();

            // ASSERT
            canExecute.Should().Be(true);
        }

        /// <summary>
        /// Тест проверяет, что при выполнении команды корректно фисируется намерение игрока использование предмета.
        /// </summary>
        [Test]
        public void Execute_CanUse_UsageIntended()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<UseSelfCommand>();
            var humanTaskSourceMock =
                ServiceProvider.GetRequiredService<Mock<IHumanActorTaskSource<ISectorTaskSourceContext>>>();
            var inventoryState = ServiceProvider.GetRequiredService<IInventoryState>();
            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();

            // ACT
            command.Execute();

            // ASSERT
            var selectedProp = inventoryState.SelectedProp.Prop;

            humanTaskSourceMock.Verify(x => x.Intent(It.Is<IIntention>(intention =>
                    CheckUsePropIntention(intention, playerState, selectedProp)
                ),
                It.IsAny<IActor>()));
        }

        private static bool CheckUsePropIntention(IIntention intention, ISectorUiState playerState, IProp usedProp)
        {
            var usePropIntention = (Intention<UsePropTask>)intention;
            var usePropTask = usePropIntention.TaskFactory(playerState.ActiveActor.Actor);
            return usePropTask.UsedProp == usedProp;
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<ISectorUiState> playerStateMock)
        {
            var propScheme = new TestPropScheme
            {
                Use = new TestPropUseSubScheme()
            };
            var usableResource = new Resource(propScheme, 1);

            var equipmentViewModelMock = new Mock<IPropItemViewModel>();
            equipmentViewModelMock.SetupGet(x => x.Prop).Returns(usableResource);
            var equipmentViewModel = equipmentViewModelMock.Object;

            var inventoryStateMock = new Mock<IInventoryState>();
            inventoryStateMock.SetupProperty(x => x.SelectedProp, equipmentViewModel);
            var inventoryState = inventoryStateMock.Object;

            Container.AddSingleton(factory => inventoryState);

            Container.AddSingleton<UseSelfCommand>();

            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.SetupGet(x => x.Items).Returns(Array.Empty<IActor>());
            var actorManager = actorManagerMock.Object;
            Container.AddSingleton(actorManager);
        }
    }
}