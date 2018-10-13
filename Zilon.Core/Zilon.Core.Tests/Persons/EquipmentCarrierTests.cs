using FluentAssertions;

using NUnit.Framework;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

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
                Equip = new TestPropEquipSubScheme
                {
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Hand                        
                    }
                }
            };

            var slotSchemes = new[] {
                new PersonSlotSubScheme{
                    Types = EquipmentSlotTypes.Hand
                }
            };

            var tacticalActScheme = new TestTacticalActScheme();

            var equipment = new Equipment(scheme, new[] { tacticalActScheme });

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