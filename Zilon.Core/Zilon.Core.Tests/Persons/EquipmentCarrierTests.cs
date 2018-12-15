using System;
using FluentAssertions;

using NUnit.Framework;
using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
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
            var scheme = new TestPropScheme
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

        /// <summary>
        /// Тест проверяет, что при установке экипировки выстреливает событие на изменение экипировки.
        /// </summary>
        [Test]
        public void SetEquipment_ChangePistolBySword_EquipmentChanged()
        {
            // ARRANGE
            var pistolScheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Ranged, PropTags.Equipment.Weapon }
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

            var equipment = new Equipment(pistolScheme, new[] { tacticalActScheme });

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

        /// <summary>
        /// Тест проверяет, что при экипировке двух мечей не выбрасывается исключение.
        /// </summary>
        [Test]
        public void SetEquipment_DualShortSwords_NoException()
        {
            // ARRANGE
            var scheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Weapon },
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
                },
                new PersonSlotSubScheme{
                    Types = EquipmentSlotTypes.Hand
                }
            };

            var tacticalActScheme = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Range = new Range<int>(1, 1)
                }
            };

            var swordEquipment1 = new Equipment(scheme, new[] { tacticalActScheme });
            var swordEquipment2 = new Equipment(scheme, new[] { tacticalActScheme });

            const int swordSlot1 = 0;
            const int swordSlot2 = 1;

            var carrier = new EquipmentCarrier(slotSchemes);


            // ACT
            Action act = () =>
            {
                carrier.SetEquipment(swordEquipment1, swordSlot1);
                carrier.SetEquipment(swordEquipment2, swordSlot2);
            };


            // ASSERT
            act.Should().NotThrow<Exception>();
        }

        /// <summary>
        /// Тест проверяет, что при экипировке двух пистолетов (стрелковых оружий) выбрасывается исключение.
        /// </summary>
        /// <remarks>
        /// Потому что для стрелкового оружия может быть разная дистанция действия. Пока не продуман выбор
        /// действий с учётом дальности. Поэтому доступно использование только парного рукопашного оружия.
        /// Если замечена попытка экипировки парных пистолетов, значит его пропустила команда. Это является
        /// сбойной ситуацией, поэтому выбрасываем исключение.
        /// </remarks>
        [Test]
        public void SetEquipment_DualPistols_ExceptionRaised()
        {
            // ARRANGE
            var scheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Ranged, PropTags.Equipment.Weapon },
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
                },
                new PersonSlotSubScheme{
                    Types = EquipmentSlotTypes.Hand
                }
            };

            var tacticalActScheme = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Range = new Range<int>(1, 6)
                }
            };

            var pistolEquipment1 = new Equipment(scheme, new[] { tacticalActScheme });
            var pistolEquipment2 = new Equipment(scheme, new[] { tacticalActScheme });

            const int pistolSlot1 = 0;
            const int pistolSlot2 = 1;

            var carrier = new EquipmentCarrier(slotSchemes);


            // ACT
            Action act = () =>
            {
                carrier.SetEquipment(pistolEquipment1, pistolSlot1);
                carrier.SetEquipment(pistolEquipment2, pistolSlot2);
            };


            // ASSERT
            act.Should().Throw<Exception>();
        }
    }
}