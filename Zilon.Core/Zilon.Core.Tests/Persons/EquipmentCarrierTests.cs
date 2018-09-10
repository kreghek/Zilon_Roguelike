using FluentAssertions;

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
        public void SetEquipment_ChangeEquipment_EventRaised()
        {
            // ARRANGE
            var scheme = new PropScheme
            {
                Equip = new PropEquipSubScheme {
                    ActSids = new[] { "weapon" }
                }
            };

            var slotSchemes = new [] {
                new PersonSlotSubScheme{
                    AvailableTags = new[]{ "weapon" }
                }
            };

            var tacticalActScheme = new TacticalActScheme();

            var equipment = new Equipment(scheme, new []{ tacticalActScheme });

            const int changedSlot = 0;

            var carrier = new EquipmentCarrier(slotSchemes);


            // ACT
            using (var monitor = carrier.Monitor())
            {
                carrier.SetEquipment(equipment, changedSlot);



                // ASSERT
                monitor.Should().Raise(nameof(carrier.EquipmentChanged));
            }
        }
    }
}