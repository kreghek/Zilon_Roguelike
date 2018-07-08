using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class EquipmentCarrierTests
    {
        /// <summary>
        /// Тест проверяет, что при установке экипировки выстреливает событие на изменение экипировки.
        /// </summary>
        [Test]
        public void SetEquipmentTest()
        {
            // ARRANGE
            var scheme = new PropScheme
            {
                Equip = new PropEquipSubScheme()
            };
            var tacticalActMock = new Mock<ITacticalAct>();
            var tacticalAct = tacticalActMock.Object;

            var equipment = new Equipment(scheme, new []{ tacticalAct });

            const int expectedSlot = 1;
            const int expectedSlotCount = 3;

            var carrier = new EquipmentCarrier(expectedSlotCount);


            // ACT
            using (var monitor = carrier.Monitor())
            {
                carrier.SetEquipment(equipment, expectedSlot);



                // ASSERT
                monitor.Should().Raise(nameof(carrier.EquipmentChanged));
            }
        }
    }
}