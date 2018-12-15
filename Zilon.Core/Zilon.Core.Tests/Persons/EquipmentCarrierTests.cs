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
        /// Тест проверяет, что корретно заменяется один пистолет другим.
        /// </summary>
        [Test]
        public void SetEquipment_ChangePistolByOtherPistol_EquipmentChanged()
        {
            // ARRANGE
            var pistolScheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Ranged, PropTags.Equipment.Weapon },
                Equip = new TestPropEquipSubScheme
                {
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Hand
                    }
                }
            };

            var pistol2Scheme = new TestPropScheme
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
                }
            };

            var tacticalActScheme = new TestTacticalActScheme();

            var pistol1 = new Equipment(pistolScheme, new[] { tacticalActScheme });
            var pistol2 = new Equipment(pistol2Scheme, new[] { tacticalActScheme });

            const int changedSlot = 0;

            var carrier = new EquipmentCarrier(slotSchemes);


            // ACT
            carrier.SetEquipment(pistol1, changedSlot);
            Action act = () =>
            {
                carrier.SetEquipment(pistol2, changedSlot);
            };



            // ASSERT
            act.Should().NotThrow();
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

        /// <summary>
        /// Тест проверяет, что при экипировке щитов выбрасывается исключение.
        /// </summary>
        /// <remarks>
        /// Это лишено смысла и не логично. Будет нелепо выглядеть атака, если у игрока два щита в отоих руках.
        /// Варианты с 4 руками пока не рассмыстриваем.
        /// </remarks>
        [Test]
        public void SetEquipment_DualShields_ExceptionRaised()
        {
            // ARRANGE
            var scheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Shield },
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

            var shieldEquipment1 = new Equipment(scheme, new[] { tacticalActScheme });
            var shieldEquipment2 = new Equipment(scheme, new[] { tacticalActScheme });

            const int shieldSlot1 = 0;
            const int shieldSlot2 = 1;

            var carrier = new EquipmentCarrier(slotSchemes);


            // ACT
            Action act = () =>
            {
                carrier.SetEquipment(shieldEquipment1, shieldSlot1);
                carrier.SetEquipment(shieldEquipment2, shieldSlot2);
            };


            // ASSERT
            act.Should().Throw<Exception>();
        }

        /// <summary>
        /// Тест проверяет, что при экипировке меча и щита не происходит исключений.
        /// </summary>
        [Test]
        public void SetEquipment_SwordAndShield_NoException()
        {
            // ARRANGE
            var swordScheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Weapon },
                Equip = new TestPropEquipSubScheme
                {
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Hand
                    }
                }
            };

            var shieldScheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Shield },
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

            var swordEquipment1 = new Equipment(swordScheme, new[] { tacticalActScheme });
            var sheildEquipment2 = new Equipment(shieldScheme, new[] { tacticalActScheme });

            const int swordSlot1 = 0;
            const int swordSlot2 = 1;

            var carrier = new EquipmentCarrier(slotSchemes);


            // ACT
            Action act = () =>
            {
                carrier.SetEquipment(swordEquipment1, swordSlot1);
                carrier.SetEquipment(sheildEquipment2, swordSlot2);
            };


            // ASSERT
            act.Should().NotThrow<Exception>();
        }

        /// <summary>
        /// Тест проверяет, что при экипировке пистолета и щита не происходит исключений.
        /// </summary>
        [Test]
        public void SetEquipment_PistolAndShield_NoException()
        {
            // ARRANGE
            var pistolScheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Weapon, PropTags.Equipment.Ranged },
                Equip = new TestPropEquipSubScheme
                {
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Hand
                    }
                }
            };

            var shieldScheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Shield },
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

            var pistolEquipment1 = new Equipment(pistolScheme, new[] { tacticalActScheme });
            var sheildEquipment2 = new Equipment(shieldScheme, new[] { tacticalActScheme });

            const int swordSlot1 = 0;
            const int swordSlot2 = 1;

            var carrier = new EquipmentCarrier(slotSchemes);


            // ACT
            Action act = () =>
            {
                carrier.SetEquipment(pistolEquipment1, swordSlot1);
                carrier.SetEquipment(sheildEquipment2, swordSlot2);
            };


            // ASSERT
            act.Should().NotThrow<Exception>();
        }

        /// <summary>
        /// Тест проверяет, что при экипировке пистолета и щита не происходит исключений.
        /// </summary>
        [Test]
        public void SetEquipment_PistolAndShield2_NoException()
        {
            // ARRANGE
            var pistolScheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Weapon, PropTags.Equipment.Ranged },
                Equip = new TestPropEquipSubScheme
                {
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Hand
                    }
                }
            };

            var shieldScheme = new TestPropScheme
            {
                Tags = new[] { PropTags.Equipment.Shield },
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

            var pistolEquipment1 = new Equipment(pistolScheme, new[] { tacticalActScheme });
            var sheildEquipment2 = new Equipment(shieldScheme, new[] { tacticalActScheme });

            // Смена слотов относительно предыдузего теста
            const int swordSlot1 = 1;
            const int swordSlot2 = 2;

            var carrier = new EquipmentCarrier(slotSchemes);


            // ACT
            Action act = () =>
            {
                carrier.SetEquipment(pistolEquipment1, swordSlot1);
                carrier.SetEquipment(sheildEquipment2, swordSlot2);
            };


            // ASSERT
            act.Should().NotThrow<Exception>();
        }
    }
}