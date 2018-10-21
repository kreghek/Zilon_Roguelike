using FluentAssertions;

using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture()]
    public class UseSelfCommandTests: CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно использовать предмет, если есть информация об использовании.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = Container.GetInstance<UseSelfCommand>();



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
            var command = Container.GetInstance<UseSelfCommand>();
            var humanTaskSourceMock = Container.GetInstance<Mock<IHumanActorTaskSource>>();
            var inventoryState = Container.GetInstance<IInventoryState>();
            var playerState = Container.GetInstance<IPlayerState>();



            // ACT
            command.Execute();


            // ASSERT
            var selectedProp = inventoryState.SelectedProp.Prop;

            humanTaskSourceMock.Verify(x => x.Intent(It.Is<IIntention>(intention =>
                CheckUsePropIntention(intention, playerState, selectedProp)
            )));
        }

        private static bool CheckUsePropIntention(IIntention intention, IPlayerState playerState, IProp usedProp)
        {
            var usePropIntention = (Intention<UsePropTask>)intention;
            var usePropTask = usePropIntention.TaskFactory(playerState.ActiveActor.Actor);
            return usePropTask.UsedProp == usedProp;
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<IPlayerState> playerStateMock)
        {
            var propScheme = new PropScheme
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

            Container.Register(factory => inventoryState, new PerContainerLifetime());

            Container.Register<UseSelfCommand>(new PerContainerLifetime());
        }
    }
}