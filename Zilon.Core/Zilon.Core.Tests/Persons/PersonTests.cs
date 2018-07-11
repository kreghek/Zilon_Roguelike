using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class PersonTests
    {
        /// <summary>
        /// Тест проверяет, что персонаж корректно обрабатывает назначение экипировки.
        /// </summary>
        [Test]
        public void SetEquipment_SetSingleEquipment_HasActs()
        {
            // ARRANGE
            var personScheme = new PersonScheme
            {
                SlotCount = 3
            };

            var person = new Person(personScheme);

            var scheme = new PropScheme
            {
                Equip = new PropEquipSubScheme()
            };

            var tacticActMock = new Mock<ITacticalAct>();
            var tacticAct = tacticActMock.Object;

            var equipment = new Equipment(scheme, new []{ tacticAct });

            const int expectedSlotIndex = 0;



            // ACT

            person.EquipmentCarrier.SetEquipment(equipment, expectedSlotIndex);



            // ARRANGE
            person.TacticalActCarrier.Acts[0].Should().Be(tacticAct);
        }
    }
}