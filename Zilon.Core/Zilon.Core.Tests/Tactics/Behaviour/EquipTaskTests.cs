using FluentAssertions;
using Moq;

using NUnit.Framework;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Tactics.Behaviour
{
    /// <summary>
    /// Тесты на команду экипировки предмета в указанный слот.
    /// </summary>
    /// <remarks>
    ///Предмет может быть экипирован из инвентаря (и) или из другого слота (с).
    /// Предмет может быть экипирован в пустой слот (0) и слот, в котором уже есть другой предмет (1).
    ///       (и)                                           (с)
    /// (0) изымаем предмет из инвентаря                 меняем предметы в слотах местами
    /// (1) изымаем из инвентаря, а текущий в инвентярь  меняем предметы в слотах местами
    /// </remarks>
    [TestFixture]
    public class EquipTaskTests
    {
        /// <summary>
        /// Тест проверяет, что предмет успешно экипируется в пустой подходящий слой (и0).
        /// </summary>
        [Test]
        public void EquipTask_EmptySlotAndFromInventory_PropEquipedInSlot()
        {
            // ARRANGE
            const int testedSlotIndex = 0;
            var propScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme()
            };
            var testedEquipmentProp = new Equipment(propScheme, new ITacticalActScheme[0]);

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var equipmentCarrierMock = new Mock<EquipmentCarrierBase>(1)
                .As<IEquipmentCarrier>();
            var equipmentCarrier = equipmentCarrierMock.Object;
            personMock.SetupGet(x => x.EquipmentCarrier).Returns(equipmentCarrier);

            var inventoryMock = new Mock<IPropStore>();
            var inventory = inventoryMock.Object;
            personMock.SetupGet(x => x.Inventory).Returns(inventory);


            var task = new EquipTask(actor, testedEquipmentProp, testedSlotIndex);



            // ACT
            task.Execute();



            // ASSERT
            equipmentCarrier[0].Should().BeSameAs(testedEquipmentProp);
            inventoryMock.Verify(x => x.Remove(It.Is<IProp>(equipment => equipment == testedEquipmentProp)), Times.Once);
            inventoryMock.Verify(x => x.Add(It.IsAny<IProp>()), Times.Never);
        }

        /// <summary>
        /// Тест проверяет, что предмет успешно экипируется в пустой подходящий слой (и1).
        /// </summary>
        [Test]
        public void EquipTask_EquipedSlotAndFromInventory_PropEquipedInSlot()
        {
            // ARRANGE
            const int testedSlotIndex = 0;
            var propScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme()
            };
            var testedEquipmentProp = new Equipment(propScheme, new ITacticalActScheme[0]);
            var equipedEquipmentProp = new Equipment(propScheme, new ITacticalActScheme[0]);

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var equipmentCarrierMock = new Mock<EquipmentCarrierBase>(new Equipment[] { equipedEquipmentProp })
                .As<IEquipmentCarrier>();
            var equipmentCarrier = equipmentCarrierMock.Object;
            personMock.SetupGet(x => x.EquipmentCarrier).Returns(equipmentCarrier);

            var inventoryMock = new Mock<IPropStore>();
            var inventory = inventoryMock.Object;
            personMock.SetupGet(x => x.Inventory).Returns(inventory);

            

            var task = new EquipTask(actor, testedEquipmentProp, testedSlotIndex);



            // ACT
            task.Execute();



            // ASSERT
            equipmentCarrier[0].Should().BeSameAs(testedEquipmentProp);
            inventoryMock.Verify(x => x.Remove(It.Is<IProp>(equipment => equipment == testedEquipmentProp)), Times.Once);
            inventoryMock.Verify(x => x.Add(It.Is<IProp>(equipment => equipment == equipedEquipmentProp)), Times.Once);
        }

        /// <summary>
        /// Тест проверяет, что предмет успешно экипируется в пустой подходящий слой (и1).
        /// </summary>
        [Test]
        public void EquipTask_EmptySlotAndFromSlot_PropEquipedInSlot()
        {
            // ARRANGE
            const int testedSlotIndex = 0;
            var propScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme()
            };
            var testedEquipmentProp = new Equipment(propScheme, new ITacticalActScheme[0]);

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var equipmentsInit = new Equipment[] {null, testedEquipmentProp };
            var equipmentCarrierMock = new Mock<EquipmentCarrierBase>(equipmentsInit)
                .As<IEquipmentCarrier>();
            var equipmentCarrier = equipmentCarrierMock.Object;
            personMock.SetupGet(x => x.EquipmentCarrier).Returns(equipmentCarrier);

            var inventoryMock = new Mock<IPropStore>();
            var inventory = inventoryMock.Object;
            personMock.SetupGet(x => x.Inventory).Returns(inventory);



            var task = new EquipTask(actor, testedEquipmentProp, testedSlotIndex);



            // ACT
            task.Execute();



            // ASSERT
            equipmentCarrier[0].Should().BeSameAs(testedEquipmentProp);
            equipmentCarrier[1].Should().BeNull();
            inventoryMock.Verify(x => x.Add(It.IsAny<IProp>()), Times.Never);
            inventoryMock.Verify(x => x.Remove(It.IsAny<IProp>()), Times.Never);
        }

        /// <summary>
        /// Тест проверяет, что предмет успешно экипируется в пустой подходящий слой (и1).
        /// </summary>
        [Test]
        public void EquipTask_EquipedSlotAndFromSlot_PropEquipedInSlot()
        {
            // ARRANGE
            const int testedSlotIndex = 0;
            var propScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme()
            };
            var testedEquipmentProp = new Equipment(propScheme, new ITacticalActScheme[0], name: "tested");
            var equipedEquipmentProp = new Equipment(propScheme, new ITacticalActScheme[0], name: "equiped");

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var equipmentsInit = new Equipment[] { equipedEquipmentProp, testedEquipmentProp };
            var equipmentCarrierMock = new Mock<EquipmentCarrierBase>(equipmentsInit)
               .As<IEquipmentCarrier>();
            var equipmentCarrier = equipmentCarrierMock.Object;
            personMock.SetupGet(x => x.EquipmentCarrier).Returns(equipmentCarrier);

            var inventoryMock = new Mock<IPropStore>();
            var inventory = inventoryMock.Object;
            personMock.SetupGet(x => x.Inventory).Returns(inventory);



            var task = new EquipTask(actor, testedEquipmentProp, testedSlotIndex);



            // ACT
            task.Execute();



            // ASSERT
            equipmentCarrier[0].Should().BeSameAs(testedEquipmentProp);
            equipmentCarrier[1].Should().BeSameAs(equipedEquipmentProp);
            inventoryMock.Verify(x => x.Add(It.IsAny<IProp>()), Times.Never);
            inventoryMock.Verify(x => x.Remove(It.IsAny<IProp>()), Times.Never);
        }
    }
}