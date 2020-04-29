using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class HumanPersonTests
    {
        /// <summary>
        /// Тест проверяет, что персонаж корректно обрабатывает назначение экипировки.
        /// </summary>
        [Test]
        public void SetEquipment_SetSingleEquipment_HasActs()
        {
            // ARRANGE
            var slotSchemes = new[] {
                new PersonSlotSubScheme{
                    Types = EquipmentSlotTypes.Hand
                }
            };

            var personScheme = new PersonScheme
            {
                Slots = slotSchemes
            };

            var defaultActScheme = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme()
            };

            var evolutionModule = CreateEvolutionFakeModule();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var person = new HumanPerson(personScheme, defaultActScheme, evolutionModule, survivalRandomSource);

            var propScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    SlotTypes = new[] { EquipmentSlotTypes.Hand }
                }
            };

            var tacticalActScheme = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme()
            };

            var equipment = new Equipment(propScheme, new[] { tacticalActScheme });

            const int expectedSlotIndex = 0;

            // ACT

            person.GetModule<IEquipmentModule>()[expectedSlotIndex] = equipment;

            // ARRANGE
            person.GetModule<ICombatActModule>().Acts[0].Stats.Should().Be(tacticalActScheme.Stats);
        }

        /// <summary>
        /// Тест проверяет, что при получении перка характеристики персонажа пересчитываются.
        /// </summary>
        [Test]
        public void HumanPerson_PerkLeveledUp_StatsRecalculated()
        {
            // ARRANGE

            var slotSchemes = new[] {
                new PersonSlotSubScheme{
                    Types = EquipmentSlotTypes.Hand
                }
            };

            var personScheme = new PersonScheme
            {
                Slots = slotSchemes
            };

            var defaultActScheme = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme()
            };

            var perkMock = new Mock<IPerk>();
            perkMock.SetupGet(x => x.CurrentLevel).Returns(new PerkLevel(0, 0));
            perkMock.SetupGet(x => x.Scheme).Returns(new PerkScheme
            {
                Levels = new[] {
                    new PerkLevelSubScheme{
                        Rules = new []{
                            new PerkRuleSubScheme{
                                Type = PersonRuleType.Ballistic,
                                Level = PersonRuleLevel.Lesser
                            }
                        }
                    }
                }
            });
            var perk = perkMock.Object;

            var stats = new[] {
                new SkillStatItem{Stat = SkillStatType.Ballistic, Value = 10 }
            };

            var evolutionModuleMock = new Mock<IEvolutionModule>();
            evolutionModuleMock.SetupGet(x => x.Key).Returns(nameof(IEvolutionModule));
            evolutionModuleMock.SetupGet(x => x.Perks).Returns(new[] { perk });
            evolutionModuleMock.SetupGet(x => x.Stats).Returns(stats);
            var evolutionModule = evolutionModuleMock.Object;

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            // ACT
            var person = new HumanPerson(personScheme, defaultActScheme, evolutionModule, survivalRandomSource);

            // ASSERT
            var testedStat = person.GetModule<IEvolutionModule>().Stats.Single(x => x.Stat == SkillStatType.Ballistic);
            testedStat.Value.Should().Be(11);
        }

        /// <summary>
        /// Тест проверяет, что при получении перка на здоровье характеристики персонажа
        /// пересчитываются - хп увеличивается.
        /// </summary>
        [Test]
        public void HumanPerson_HpPerk_HealthIncreased()
        {
            // ARRANGE

            var slotSchemes = new[] {
                new PersonSlotSubScheme{
                    Types = EquipmentSlotTypes.Hand
                }
            };

            var personScheme = new PersonScheme
            {
                Hp = 10,
                Slots = slotSchemes
            };

            var defaultActScheme = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme()
            };

            var perkMock = new Mock<IPerk>();
            perkMock.SetupGet(x => x.CurrentLevel).Returns(new PerkLevel(0, 0));
            perkMock.SetupGet(x => x.Scheme).Returns(new PerkScheme
            {
                Levels = new[] {
                    new PerkLevelSubScheme{
                        Rules = new []{
                            new PerkRuleSubScheme{
                                Type = PersonRuleType.Health,
                                Level = PersonRuleLevel.Lesser
                            }
                        }
                    }
                }
            });
            var perk = perkMock.Object;

            var stats = System.Array.Empty<SkillStatItem>();

            var evolutionModuleMock = new Mock<IEvolutionModule>();
            evolutionModuleMock.SetupGet(x => x.Key).Returns(nameof(IEvolutionModule));
            evolutionModuleMock.SetupGet(x => x.Perks).Returns(new[] { perk });
            evolutionModuleMock.SetupGet(x => x.Stats).Returns(stats);
            var evolutionModule = evolutionModuleMock.Object;

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            // ACT
            var person = new HumanPerson(personScheme, defaultActScheme, evolutionModule, survivalRandomSource);

            // ASSERT
            var testedStat = person.Survival.Stats.Single(x => x.Type == SurvivalStatType.Health);
            testedStat.Value.Should().Be(11);
        }

        /// <summary>
        /// Тест проверяет, что при получении перка на увеличение урона для экипировки
        /// с определёнными тегами модификатор броса на эффективность действия увеличивается.
        /// </summary>
        [Test]
        public void HumanPerson_SwordSkillsPerk_DamageOfActIsIncreased()
        {
            // ARRANGE

            var slotSchemes = new[] {
                new PersonSlotSubScheme{
                    Types = EquipmentSlotTypes.Hand
                }
            };

            var personScheme = new PersonScheme
            {
                Hp = 10,
                Slots = slotSchemes
            };

            var defaultActScheme = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme()
            };

            var perkMock = new Mock<IPerk>();
            perkMock.SetupGet(x => x.CurrentLevel).Returns(new PerkLevel(0, 0));
            perkMock.SetupGet(x => x.Scheme).Returns(new PerkScheme
            {
                Levels = new[] {
                    new PerkLevelSubScheme{
                        Rules = new []{
                            new PerkRuleSubScheme{
                                Type = PersonRuleType.Damage,
                                Level = PersonRuleLevel.Absolute,
                                Params = "{\"WeaponTags\":[\"sword\"]}"
                            }
                        }
                    }
                }
            });
            var perk = perkMock.Object;

            var stats = System.Array.Empty<SkillStatItem>();

            var evolutionModuleMock = new Mock<IEvolutionModule>();
            evolutionModuleMock.SetupGet(x => x.Key).Returns(nameof(IEvolutionModule));
            evolutionModuleMock.SetupGet(x => x.Perks).Returns(new[] { perk });
            evolutionModuleMock.SetupGet(x => x.Stats).Returns(stats);
            var evolutionData = evolutionModuleMock.Object;

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var swordScheme = new TestPropScheme
            {
                Tags = new[] { "sword" },
                Equip = new TestPropEquipSubScheme
                {
                    SlotTypes = new[] { EquipmentSlotTypes.Hand }
                }
            };

            var swordAct = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Effect = TacticalActEffectType.Damage,
                    Efficient = new Roll(1, 1)
                }
            };

            var equipment = new Equipment(swordScheme, new ITacticalActScheme[] { swordAct });

            // ACT
            var person = new HumanPerson(personScheme, defaultActScheme, evolutionData, survivalRandomSource);
            person.GetModule<IEquipmentModule>()[0] = equipment;

            // ASSERT
            var testedAct = person.GetModule<ICombatActModule>().Acts[0];
            testedAct.Efficient.Modifiers.ResultBuff.Should().Be(10);
        }

        /// <summary>
        /// Тест проверяет, что если экипировать предмет с бронёй,
        /// то броня будет записана в боевые характеристики персонажа.
        /// </summary>
        [Test]
        public void HumanPerson_EquipArmor_ArmorInCombatStats()
        {
            // ARRANGE

            var personScheme = new TestPersonScheme
            {
                Slots = new[]{
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Body
                    }
                }
            };

            var defaultAct = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Efficient = new Roll(6, 1)
                }
            };

            var evolutionData = CreateEvolutionFakeModule();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var person = new HumanPerson(personScheme, defaultAct, evolutionData, survivalRandomSource);

            var armorPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 10,
                            AbsorbtionLevel = PersonRuleLevel.Lesser
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Body
                    }
                }
            };

            var armorProp = new Equipment(armorPropScheme, System.Array.Empty<ITacticalActScheme>());

            // ACT
            person.GetModule<IEquipmentModule>()[0] = armorProp;

            // ASSERT
            person.CombatStats.DefenceStats.Armors[0].Impact.Should().Be(ImpactType.Kinetic);
            person.CombatStats.DefenceStats.Armors[0].ArmorRank.Should().Be(10);
            person.CombatStats.DefenceStats.Armors[0].AbsorbtionLevel.Should().Be(PersonRuleLevel.Lesser);
        }

        private static IEvolutionModule CreateEvolutionFakeModule()
        {
            var evolutionDataMock = new Mock<IEvolutionModule>();
            evolutionDataMock.SetupGet(x => x.Key).Returns(nameof(IEvolutionModule));
            var evolutionData = evolutionDataMock.Object;
            return evolutionData;
        }

        /// <summary>
        /// Тест проверяет, что если экипировать несколько предметов с бронёй,
        /// то вся броня будет учитываться.
        /// </summary>
        [Test]
        public void HumanPerson_MultipleArmor_ArmorRankIncreased()
        {
            // ARRANGE

            var personScheme = new TestPersonScheme
            {
                Slots = new[]{
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Head
                    },
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Body
                    }
                }
            };

            var defaultAct = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Efficient = new Roll(6, 1)
                }
            };

            var evolutionData = CreateEvolutionFakeModule();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var person = new HumanPerson(personScheme, defaultAct, evolutionData, survivalRandomSource);

            var armorHeadPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 10,
                            AbsorbtionLevel = PersonRuleLevel.Lesser
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Head
                    }
                }
            };

            var armorBodyPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 10,
                            AbsorbtionLevel = PersonRuleLevel.Lesser
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Body
                    }
                }
            };

            var armorHeadProp = new Equipment(armorHeadPropScheme, System.Array.Empty<ITacticalActScheme>());
            var armorBodyProp = new Equipment(armorBodyPropScheme, System.Array.Empty<ITacticalActScheme>());

            // ACT
            person.GetModule<IEquipmentModule>()[0] = armorHeadProp;
            person.GetModule<IEquipmentModule>()[1] = armorBodyProp;

            // ASSERT
            person.CombatStats.DefenceStats.Armors[0].Impact.Should().Be(ImpactType.Kinetic);
            person.CombatStats.DefenceStats.Armors[0].ArmorRank.Should().Be(15);
            person.CombatStats.DefenceStats.Armors[0].AbsorbtionLevel.Should().Be(PersonRuleLevel.Lesser);
        }

        /// <summary>
        /// Тест проверяет, что если экипировать несколько предметов с бронёй,
        /// то вся броня будет учитываться.
        /// </summary>
        [Test]
        public void HumanPerson_MultipleArmor_ArmorRankIncreased2()
        {
            // ARRANGE

            var personScheme = new TestPersonScheme
            {
                Slots = new[]{
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Head
                    },
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Body
                    }
                }
            };

            var defaultAct = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Efficient = new Roll(6, 1)
                }
            };

            var evolutionData = CreateEvolutionFakeModule();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var person = new HumanPerson(personScheme, defaultAct, evolutionData, survivalRandomSource);

            var armorHeadPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 18,
                            AbsorbtionLevel = PersonRuleLevel.Normal
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Head
                    }
                }
            };

            var armorBodyPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 10,
                            AbsorbtionLevel = PersonRuleLevel.Lesser
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Body
                    }
                }
            };

            var armorHeadProp = new Equipment(armorHeadPropScheme, System.Array.Empty<ITacticalActScheme>());
            var armorBodyProp = new Equipment(armorBodyPropScheme, System.Array.Empty<ITacticalActScheme>());

            // ACT
            person.GetModule<IEquipmentModule>()[0] = armorHeadProp;
            person.GetModule<IEquipmentModule>()[1] = armorBodyProp;

            // ASSERT
            person.CombatStats.DefenceStats.Armors[0].Impact.Should().Be(ImpactType.Kinetic);
            person.CombatStats.DefenceStats.Armors[0].ArmorRank.Should().Be(10 + 9 + 6);
            person.CombatStats.DefenceStats.Armors[0].AbsorbtionLevel.Should().Be(PersonRuleLevel.Lesser);
        }

        /// <summary>
        /// Тест проверяет, что если экипировать несколько предметов с бронёй,
        /// то вся броня будет учитываться.
        /// </summary>
        [Test]
        public void HumanPerson_MultipleArmor_ArmorRankIncreased3()
        {
            // ARRANGE

            var personScheme = new TestPersonScheme
            {
                Slots = new[]{
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Head
                    },
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Body
                    }
                }
            };

            var defaultAct = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Efficient = new Roll(6, 1)
                }
            };

            var evolutionData = CreateEvolutionFakeModule();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var person = new HumanPerson(personScheme, defaultAct, evolutionData, survivalRandomSource);

            var armorHeadPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 1,
                            AbsorbtionLevel = PersonRuleLevel.Lesser
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Head
                    }
                }
            };

            var armorBodyPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 18,
                            AbsorbtionLevel = PersonRuleLevel.Grand
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Body
                    }
                }
            };

            var armorHeadProp = new Equipment(armorHeadPropScheme);
            var armorBodyProp = new Equipment(armorBodyPropScheme);

            // ACT
            person.GetModule<IEquipmentModule>()[0] = armorHeadProp;
            person.GetModule<IEquipmentModule>()[1] = armorBodyProp;

            // ASSERT
            person.CombatStats.DefenceStats.Armors[0].Impact.Should().Be(ImpactType.Kinetic);
            person.CombatStats.DefenceStats.Armors[0].ArmorRank.Should().Be(1 + 9 + 6 * 2);
            person.CombatStats.DefenceStats.Armors[0].AbsorbtionLevel.Should().Be(PersonRuleLevel.Lesser);
        }

        /// <summary>
        /// Тест проверяет, что если экипировать несколько предметов с бронёй,
        /// то вся броня будет учитываться.
        /// </summary>
        [Test]
        public void HumanPerson_MultipleArmor_ArmorRankIncreased4()
        {
            // ARRANGE

            var personScheme = new TestPersonScheme
            {
                Slots = new[]{
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Head
                    },
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Body
                    }
                }
            };

            var defaultAct = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Efficient = new Roll(6, 1)
                }
            };

            var evolutionData = CreateEvolutionFakeModule();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var person = new HumanPerson(personScheme, defaultAct, evolutionData, survivalRandomSource);

            var armorHeadPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 1,
                            AbsorbtionLevel = PersonRuleLevel.Normal
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Head
                    }
                }
            };

            var armorBodyPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 18,
                            AbsorbtionLevel = PersonRuleLevel.Normal
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Body
                    }
                }
            };

            var armorHeadProp = new Equipment(armorHeadPropScheme, System.Array.Empty<ITacticalActScheme>());
            var armorBodyProp = new Equipment(armorBodyPropScheme, System.Array.Empty<ITacticalActScheme>());

            // ACT
            person.GetModule<IEquipmentModule>()[0] = armorHeadProp;
            person.GetModule<IEquipmentModule>()[1] = armorBodyProp;

            // ASSERT
            person.CombatStats.DefenceStats.Armors[0].Impact.Should().Be(ImpactType.Kinetic);
            person.CombatStats.DefenceStats.Armors[0].ArmorRank.Should().Be(1 + 9);
            person.CombatStats.DefenceStats.Armors[0].AbsorbtionLevel.Should().Be(PersonRuleLevel.Normal);
        }

        /// <summary>
        /// Тест проверяет, что если экипировать несколько предметов с бронёй,
        /// то вся броня будет учитываться.
        /// </summary>
        [Test]
        public void HumanPerson_MultipleDifferentArmor_AllArmorsInStats()
        {
            // ARRANGE

            var personScheme = new TestPersonScheme
            {
                Slots = new[]{
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Head
                    },
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Body
                    }
                }
            };

            var defaultAct = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Efficient = new Roll(6, 1)
                }
            };

            var evolutionData = CreateEvolutionFakeModule();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var person = new HumanPerson(personScheme, defaultAct, evolutionData, survivalRandomSource);

            var armorHeadPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 1,
                            AbsorbtionLevel = PersonRuleLevel.Lesser
                        },
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Psy,
                            ArmorRank = 10,
                            AbsorbtionLevel = PersonRuleLevel.Normal
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Head
                    }
                }
            };

            var armorBodyPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Armors = new IPropArmorItemSubScheme[]
                    {
                        new TestPropArmorItemSubScheme{
                            Impact = ImpactType.Kinetic,
                            ArmorRank = 18,
                            AbsorbtionLevel = PersonRuleLevel.Normal
                        }
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Body
                    }
                }
            };

            var armorHeadProp = new Equipment(armorHeadPropScheme, System.Array.Empty<ITacticalActScheme>());
            var armorBodyProp = new Equipment(armorBodyPropScheme, System.Array.Empty<ITacticalActScheme>());

            // ACT
            person.GetModule<IEquipmentModule>()[0] = armorHeadProp;
            person.GetModule<IEquipmentModule>()[1] = armorBodyProp;

            // ASSERT
            person.CombatStats.DefenceStats.Armors[0].Impact.Should().Be(ImpactType.Kinetic);
            person.CombatStats.DefenceStats.Armors[0].ArmorRank.Should().Be(1 + 9 + 6);
            person.CombatStats.DefenceStats.Armors[0].AbsorbtionLevel.Should().Be(PersonRuleLevel.Lesser);

            person.CombatStats.DefenceStats.Armors[1].Impact.Should().Be(ImpactType.Psy);
            person.CombatStats.DefenceStats.Armors[1].ArmorRank.Should().Be(10);
            person.CombatStats.DefenceStats.Armors[1].AbsorbtionLevel.Should().Be(PersonRuleLevel.Normal);
        }

        /// <summary>
        /// Тест проверяет, что если экипировать предмет бонусом к здоровью,
        /// то здоровье персонажа будет увеличено.
        /// </summary>
        [Test]
        public void HumanPerson_EquipPropWithHpBonus_HpIncreased()
        {
            // ARRANGE

            const int START_PERSON_HP = 10;
            const int LESSER_HP_BONUS = 1;

            const int EXPECTED_PERSON_MAX_HP = START_PERSON_HP + LESSER_HP_BONUS;
            const int EXPECTED_PERSON_HP = EXPECTED_PERSON_MAX_HP;


            var personScheme = new TestPersonScheme
            {
                Hp = START_PERSON_HP,
                Slots = new[]{
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Body
                    }
                }
            };

            var defaultAct = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Efficient = new Roll(6, 1)
                }
            };

            var evolutionData = CreateEvolutionFakeModule();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var person = new HumanPerson(personScheme, defaultAct, evolutionData, survivalRandomSource);

            var armorPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Rules = new[] {
                        new PersonRule(EquipCommonRuleType.Health, PersonRuleLevel.Lesser)
                    },
                    SlotTypes = new[] {
                        EquipmentSlotTypes.Body
                    }
                }
            };

            var armorProp = new Equipment(armorPropScheme, System.Array.Empty<ITacticalActScheme>());

            // ACT
            person.GetModule<IEquipmentModule>()[0] = armorProp;

            // ASSERT
            person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health).Range.Max.Should().Be(EXPECTED_PERSON_MAX_HP);
            person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health).Value.Should().Be(EXPECTED_PERSON_HP);
        }

        /// <summary>
        /// Тест проверяет, что если экипировать предмет бонусом к здоровью,
        /// то здоровье персонажа остаётся текущим.
        /// </summary>
        [Test]
        public void HumanPerson_EquipPropWithHpBonusWithNoHalfHp_HpStays()
        {
            // ARRANGE

            const int START_PERSON_HP = 10;
            const int NORMAL_HP_BONUS = 3;

            const int EXPECTED_PERSON_HP = 6;
            const int EXPECTED_PERSON_MAX_HP = START_PERSON_HP + NORMAL_HP_BONUS;

            var personScheme = new TestPersonScheme
            {
                Hp = START_PERSON_HP,
                Slots = new[]{
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Body
                    }
                }
            };

            var defaultAct = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Efficient = new Roll(6, 1)
                }
            };

            var evolutionData = CreateEvolutionFakeModule();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var person = new HumanPerson(personScheme, defaultAct, evolutionData, survivalRandomSource);

            var armorPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Rules = new[]
                    {
                        new PersonRule(EquipCommonRuleType.Health, PersonRuleLevel.Normal)
                    },
                    SlotTypes = new[]
                    {
                        EquipmentSlotTypes.Body
                    }
                }
            };

            var armorProp = new Equipment(armorPropScheme, System.Array.Empty<ITacticalActScheme>());

            var hpStat = person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
            person.Survival.DecreaseStat(SurvivalStatType.Health, hpStat.Value / 2);

            // ACT
            person.GetModule<IEquipmentModule>()[0] = armorProp;

            // ASSERT
            hpStat.Range.Max.Should().Be(EXPECTED_PERSON_MAX_HP);
            hpStat.Value.Should().Be(EXPECTED_PERSON_HP);
        }

        /// <summary>
        /// Тест проверяет, что если экипировать предмет бонусом к здоровью,
        /// то здоровье персонажа остаётся текущим.
        /// </summary>
        [Test]
        public void HumanPerson_EquipPropWithHungerResistance_DownPassOfSatietyDescreased()
        {
            // ARRANGE

            const int START_PERSON_HP = 10;
            const int EXPECTED_DOWNPASS = 2;

            var personScheme = new TestPersonScheme
            {
                Hp = START_PERSON_HP,
                Slots = new[]{
                    new PersonSlotSubScheme
                    {
                        Types = EquipmentSlotTypes.Aux
                    }
                },

                SurvivalStats = new[]
                {
                    new TestPersonSurvivalStatSubScheme
                    {
                        Type = PersonSurvivalStatType.Satiety,
                        MinValue = -100,
                        MaxValue = 100,
                        StartValue = 0
                    },

                    new TestPersonSurvivalStatSubScheme
                    {
                        Type = PersonSurvivalStatType.Hydration,
                        MinValue = -100,
                        MaxValue = 100,
                        StartValue = 0
                    }
                }
            };

            var defaultAct = new TestTacticalActScheme
            {
                Stats = new TestTacticalActStatsSubScheme
                {
                    Efficient = new Roll(6, 1)
                }
            };

            var evolutionData = CreateEvolutionFakeModule();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var person = new HumanPerson(personScheme, defaultAct, evolutionData, survivalRandomSource);

            var armorPropScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    Rules = new[]
                    {
                        new PersonRule(EquipCommonRuleType.HungerResistance, PersonRuleLevel.Normal)
                    },
                    SlotTypes = new[]
                    {
                        EquipmentSlotTypes.Aux
                    }
                }
            };

            var satietyStat = person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Satiety);

            var armorProp = new Equipment(armorPropScheme);

            // ACT
            person.GetModule<IEquipmentModule>()[0] = armorProp;

            // ASSERT
            satietyStat.DownPassRoll.Should().Be(EXPECTED_DOWNPASS);
        }
    }
}