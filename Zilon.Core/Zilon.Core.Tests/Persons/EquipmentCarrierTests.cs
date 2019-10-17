using System;
using FluentAssertions;

using NUnit.Framework;
using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.Tests.Tactics.Spatial.TestCases;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture][Parallelizable(ParallelScope.All)]
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
                carrier[changedSlot] = equipment;



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
            carrier[changedSlot] = pistol1;
            Action act = () =>
            {
                carrier[changedSlot] = pistol2;
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
                carrier[swordSlot1] = swordEquipment1;
                carrier[swordSlot2] = swordEquipment2;
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
                carrier[pistolSlot1] = pistolEquipment1;
                carrier[pistolSlot2] = pistolEquipment2;
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
                carrier[shieldSlot1] = shieldEquipment1;
                carrier[shieldSlot2] = shieldEquipment2;
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
                carrier[swordSlot1] = swordEquipment1;
                carrier[swordSlot2] = sheildEquipment2;
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
                carrier[swordSlot1] = pistolEquipment1;
                carrier[swordSlot2] = sheildEquipment2;
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
            const int swordSlot2 = 0;

            var carrier = new EquipmentCarrier(slotSchemes);


            // ACT
            Action act = () =>
            {
                carrier[swordSlot1] = pistolEquipment1;
                carrier[swordSlot2] = sheildEquipment2;
            };


            // ASSERT
            act.Should().NotThrow<Exception>();
        }

        /// <summary>
        /// Тест проверяет, что экипировка различных комбинаций предметов обрабатывается корректно.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(EquipmentCarrierTestsCaseSource), nameof(EquipmentCarrierTestsCaseSource.TestCases))]
        public void SetEquipment_PropCombinationInTwoSlots_ExpectedExceptionGeneration(string propSid1,
            string propSid2,
            bool expectException)
        {
            // ARRANGE

            for (var i = 0; i < 2; i++)
            {

                Equipment equipment1 = null;
                Equipment equipment2 = null;

                var scheme1 = GetSchemeBySid(propSid1);
                if (scheme1 != null)
                {
                    equipment1 = new Equipment(scheme1, new ITacticalActScheme[0]);
                }

                if (propSid1 == propSid2)
                {
                    if (scheme1 != null)
                    {
                        equipment2 = new Equipment(scheme1, new ITacticalActScheme[0]);
                    }
                }
                else
                {
                    var scheme2 = GetSchemeBySid(propSid2);

                    if (scheme2 != null)
                    {
                        equipment2 = new Equipment(scheme2, new ITacticalActScheme[0]);
                    }
                }

                var slotSchemes = new[] {
                    new PersonSlotSubScheme{
                        Types = EquipmentSlotTypes.Hand
                    },
                    new PersonSlotSubScheme{
                        Types = EquipmentSlotTypes.Hand
                    }
                };

                var slotIndex1 = i == 0 ? 0 : 1;
                var slotIndex2 = i == 0 ? 1 : 0;

                var carrier = new EquipmentCarrier(slotSchemes);


                // ACT
                Action act = () =>
                {
                    carrier[slotIndex1] = equipment1;
                    carrier[slotIndex2] = equipment2;
                };


                // ASSERT
                if (expectException)
                {
                    act.Should().Throw<Exception>();
                }
                else
                {
                    act.Should().NotThrow<Exception>();
                }
            }
        }

        private IPropScheme GetSchemeBySid(string sid) {
            if (sid == null)
            {
                return null;
            }

            switch (sid)
            {
                case EquipmentCarrierTestsCaseSource.Sword:
                case EquipmentCarrierTestsCaseSource.Axe:
                    return new TestPropScheme
                        {
                            Tags = new[] { PropTags.Equipment.Weapon },
                            Equip = new TestPropEquipSubScheme
                            {
                                SlotTypes = new[] {
                                    EquipmentSlotTypes.Hand
                            }
                        }
                    };

                case EquipmentCarrierTestsCaseSource.WoodenShield:
                case EquipmentCarrierTestsCaseSource.SteelShield:
                    return new TestPropScheme
                    {
                        Tags = new[] { PropTags.Equipment.Shield },
                        Equip = new TestPropEquipSubScheme
                        {
                            SlotTypes = new[] {
                                    EquipmentSlotTypes.Hand
                            }
                        }
                    };

                case EquipmentCarrierTestsCaseSource.Colt:
                case EquipmentCarrierTestsCaseSource.Magnum:
                    return new TestPropScheme
                    {
                        Tags = new[] { PropTags.Equipment.Weapon, PropTags.Equipment.Ranged },
                        Equip = new TestPropEquipSubScheme
                        {
                            SlotTypes = new[] {
                                    EquipmentSlotTypes.Hand
                            }
                        }
                    };

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}