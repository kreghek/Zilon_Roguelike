using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers.Tests
{
    [TestFixture]
    public class HasBetterEquipmentTriggerTests
    {
        [Test]
        public void TestTest()
        {
            // ARRAGE

            var personMock = new Mock<IPerson>();
            personMock.Setup(x => x.HasModule(nameof(IInventoryModule))).Returns(true);
            personMock.Setup(x => x.HasModule(nameof(IEquipmentModule))).Returns(true);

            var inventoryModuleMock = new Mock<IInventoryModule>();
            var equipmentSchemeMock = new Mock<IPropScheme>();
            var equipmentSubScheme = Mock.Of<IPropEquipSubScheme>(x => x.SlotTypes == new[] { Core.Components.EquipmentSlotTypes.Hand });
            equipmentSchemeMock.SetupGet(x => x.Equip).Returns(equipmentSubScheme);
            var equipments = new[] { new Equipment(equipmentSchemeMock.Object) };
            inventoryModuleMock.Setup(x => x.CalcActualItems()).Returns(equipments);
            var inventoryModule = inventoryModuleMock.Object;
            personMock.Setup(x => x.GetModule<IInventoryModule>(nameof(IInventoryModule))).Returns(inventoryModule);

            var equipmentModuleMock = new Mock<IEquipmentModule>();
            equipmentModuleMock.Setup(x => x.Slots).Returns(new[] { new PersonSlotSubScheme { Types = Core.Components.EquipmentSlotTypes.Hand } });
            personMock.Setup(x => x.GetModule<IEquipmentModule>(nameof(IEquipmentModule))).Returns(equipmentModuleMock.Object);

            var actor = Mock.Of<IActor>(x => x.Person == personMock.Object);

            var context = Mock.Of<ISectorTaskSourceContext>();

            var currentState = Mock.Of<ILogicState>();

            var strategyDataMock = new Mock<ILogicStrategyData>();

            var trigger = new HasBetterEquipmentTrigger();

            // ACT

            var fact = trigger.Test(actor, context, currentState, strategyDataMock.Object);

            // ASSERT
            fact.Should().BeTrue();
            strategyDataMock.VerifySet(x => x.TargetEquipment = It.Is<Equipment>(equipment => equipment != null));
            strategyDataMock.VerifySet(x => x.TargetEquipmentSlot = It.Is<int?>(slot => slot != null));
        }
    }
}