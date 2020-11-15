using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Components;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture]
    public class EquipCommandTests : CommandTestBase
    {
        private Mock<IInventoryState> _inventoryStateMock;

        /// <summary>
        /// Тест проверяет, что нельзя экипировать ресурс.
        /// </summary>
        [Test]
        public void CanExecute_SelectConsumableResource_ReturnsFalse()
        {
            // ARRANGE
            var propScheme = new TestPropScheme
            {
                Use = new TestPropUseSubScheme
                {
                    Consumable = true,
                    CommonRules = new[]
                    {
                        new ConsumeCommonRule(ConsumeCommonRuleType.Health, PersonRuleLevel.Lesser,
                            PersonRuleDirection.Positive)
                    }
                }
            };
            var resource = new Resource(propScheme, 10);

            var equipmentViewModelMock = new Mock<IPropItemViewModel>();
            equipmentViewModelMock.SetupGet(x => x.Prop).Returns(resource);
            var equipmentViewModel = equipmentViewModelMock.Object;

            _inventoryStateMock.SetupGet(x => x.SelectedProp).Returns(equipmentViewModel);

            var command = ServiceProvider.GetRequiredService<EquipCommand>();
            command.SlotIndex = 0;

            // ACT
            var canExecute = command.CanExecute();

            // ASSERT
            canExecute.Should().BeFalse();
        }

        /// <summary>
        /// Тест проверяет, что можно использовать экипировку.
        /// </summary>
        [Test]
        public void CanExecute_SelectEquipment_ReturnsTrue()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<EquipCommand>();
            command.SlotIndex = 0;

            // ACT
            var canExecute = command.CanExecute();

            // ASSERT
            canExecute.Should().BeTrue();
        }

        /// <summary>
        /// Тест проверяет, что при выполнении команды корректно фисируется намерение игрока на атаку.
        /// </summary>
        [Test]
        public void Execute_Intention()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<EquipCommand>();
            command.SlotIndex = 0;

            var humanTaskSourceMock =
                ServiceProvider.GetRequiredService<Mock<IHumanActorTaskSource<ISectorTaskSourceContext>>>();

            // ACT
            command.Execute();

            // ASSERT
            humanTaskSourceMock.Verify(x => x.Intent(It.IsAny<IIntention>(), It.IsAny<IActor>()), Times.Once);
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<ISectorUiState> playerStateMock)
        {
            var propScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    SlotTypes = new[]
                    {
                        EquipmentSlotTypes.Hand
                    }
                }
            };
            var equipment = new Equipment(propScheme, new TacticalActScheme[0]);

            var equipmentViewModelMock = new Mock<IPropItemViewModel>();
            equipmentViewModelMock.SetupGet(x => x.Prop).Returns(equipment);
            var equipmentViewModel = equipmentViewModelMock.Object;

            _inventoryStateMock = new Mock<IInventoryState>();
            _inventoryStateMock.SetupProperty(x => x.SelectedProp, equipmentViewModel);
            var inventoryState = _inventoryStateMock.Object;

            Container.AddSingleton(factory => inventoryState);
            Container.AddSingleton<EquipCommand>();
        }
    }
}