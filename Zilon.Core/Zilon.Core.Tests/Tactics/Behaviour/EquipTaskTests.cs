using FluentAssertions;

using Moq;

using NUnit.Framework;
using Zilon.Core.PersonModules;
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
    [TestFixture][Parallelizable(ParallelScope.All)]
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
            var testedEquipmentProp = new Equipment(propScheme, System.Array.Empty<ITacticalActScheme>());

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var equipmentModuleMock = new Mock<EquipmentModuleBase>(1)
                .As<IEquipmentModule>();
            equipmentModuleMock.CallBase = true;
            var equipmentModule = equipmentModuleMock.Object;
            personMock.SetupGet(x => x.GetModule<IEquipmentModule>(It.IsAny<string>())).Returns(equipmentModule);

            var inventoryMock = new Mock<IPropStore>();
            var inventory = inventoryMock.Object;
            personMock.SetupGet(x => x.Inventory).Returns(inventory);


            var task = new EquipTask(actor, testedEquipmentProp, testedSlotIndex);



            // ACT
            task.Execute();



            // ASSERT
            equipmentModule[0].Should().BeSameAs(testedEquipmentProp);
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
            var testedEquipmentProp = new Equipment(propScheme, System.Array.Empty<ITacticalActScheme>());
            var equipedEquipmentProp = new Equipment(propScheme, System.Array.Empty<ITacticalActScheme>());

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var initEquipments = new Equipment[] { equipedEquipmentProp };
            var equipmentCarrierMock = new Mock<EquipmentModuleBase>(new object[] { initEquipments })
                .As<IEquipmentModule>();
            equipmentCarrierMock.CallBase = true;
            var equipmentModule = equipmentCarrierMock.Object;
            personMock.SetupGet(x => x.GetModule<IEquipmentModule>(It.IsAny<string>())).Returns(equipmentModule);

            var inventoryMock = new Mock<IPropStore>();
            var inventory = inventoryMock.Object;
            personMock.SetupGet(x => x.Inventory).Returns(inventory);



            var task = new EquipTask(actor, testedEquipmentProp, testedSlotIndex);



            // ACT
            task.Execute();



            // ASSERT
            equipmentModule[0].Should().BeSameAs(testedEquipmentProp);
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
            var testedEquipmentProp = new Equipment(propScheme, System.Array.Empty<ITacticalActScheme>());

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var equipmentsInit = new Equipment[] { null, testedEquipmentProp };
            var equipmentModuleMock = new Mock<EquipmentModuleBase>(new object[] { equipmentsInit })
                .As<IEquipmentModule>();
            equipmentModuleMock.CallBase = true;
            var equipmentModule = equipmentModuleMock.Object;
            personMock.SetupGet(x => x.GetModule<IEquipmentModule>(It.IsAny<string>())).Returns(equipmentModule);

            var inventoryMock = new Mock<IPropStore>();
            var inventory = inventoryMock.Object;
            personMock.SetupGet(x => x.Inventory).Returns(inventory);



            var task = new EquipTask(actor, testedEquipmentProp, testedSlotIndex);



            // ACT
            task.Execute();



            // ASSERT
            equipmentModule[0].Should().BeSameAs(testedEquipmentProp);
            equipmentModule[1].Should().BeNull();
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
            var testedEquipmentProp = new Equipment(propScheme, System.Array.Empty<ITacticalActScheme>(), name: "tested");
            var equipedEquipmentProp = new Equipment(propScheme, System.Array.Empty<ITacticalActScheme>(), name: "equiped");

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            var equipmentsInit = new Equipment[] { equipedEquipmentProp, testedEquipmentProp };
            var equipmentModuleMock = new Mock<EquipmentModuleBase>(new object[] { equipmentsInit })
               .As<IEquipmentModule>();
            equipmentModuleMock.CallBase = true;
            var equipmentModule = equipmentModuleMock.Object;
            personMock.SetupGet(x => x.GetModule<IEquipmentModule>(It.IsAny<string>())).Returns(equipmentModule);

            var inventoryMock = new Mock<IPropStore>();
            var inventory = inventoryMock.Object;
            personMock.SetupGet(x => x.Inventory).Returns(inventory);



            var task = new EquipTask(actor, testedEquipmentProp, testedSlotIndex);



            // ACT
            task.Execute();



            // ASSERT
            equipmentModule[0].Should().BeSameAs(testedEquipmentProp);
            equipmentModule[1].Should().BeSameAs(equipedEquipmentProp);
            inventoryMock.Verify(x => x.Add(It.IsAny<IProp>()), Times.Never);
            inventoryMock.Verify(x => x.Remove(It.IsAny<IProp>()), Times.Never);
        }
    }
}