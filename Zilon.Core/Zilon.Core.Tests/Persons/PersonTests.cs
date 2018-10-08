using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

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
            var slotSchemes = new[] {
                new PersonSlotSubScheme{
                    Types = EquipmentSlotTypes.Hand
                }
            };

            var personScheme = new PersonScheme
            {
                Slots = slotSchemes
            };

            var defaultActScheme = new TacticalActScheme {
                Stats = new TacticalActStatsSubScheme
                {
                    Efficient = new Roll(3, 1)
                },
                Dependency = new[] {
                    new TacticalActDependencySubScheme(SkillStatType.Melee, 1)
                }
            };

            var person = new HumanPerson(personScheme, defaultActScheme, null);

            var propScheme = new PropScheme
            {
                Equip = new TestPropEquipSubScheme
                {
                    SlotTypes = new[] { EquipmentSlotTypes.Hand }
                }
            };

            var tacticalActScheme = new TacticalActScheme
            {
                Stats = new TacticalActStatsSubScheme
                {
                    Efficient = new Roll(3, 1),
                },
                Dependency = new[] {
                    new TacticalActDependencySubScheme(SkillStatType.Undefined, 1)
                }
            };

            var equipment = new Equipment(propScheme, new[] { tacticalActScheme });

            const int expectedSlotIndex = 0;



            // ACT

            person.EquipmentCarrier.SetEquipment(equipment, expectedSlotIndex);



            // ARRANGE
            person.TacticalActCarrier.Acts[0].Stats.Should().Be(tacticalActScheme.Stats);
        }

        /// <summary>
        /// Тест проверяет, что при получении перка характеристики персонажа пересчитываются.
        /// </summary>
        [Test]
        [Ignore("Нуждается в переработке")]
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

            var defaultActScheme = new TacticalActScheme
            {
                Stats = new TacticalActStatsSubScheme
                {
                    Efficient = new Roll(3, 1)
                },
                Dependency = new[] {
                    new TacticalActDependencySubScheme(SkillStatType.Melee, 1)
                }
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
                                Level = PersonRuleLevel.Normal
                            }
                        }
                    }
                }
            });
            var perk = perkMock.Object;

            var evolutionDataMock = new Mock<IEvolutionData>();
            evolutionDataMock.SetupGet(x => x.Perks)
                .Returns(new[] { perk });
            var evolutionData = evolutionDataMock.Object;



            // ACT
            var person = new HumanPerson(personScheme, defaultActScheme, evolutionData);



            // ASSERT
            //var testedStat = person.CombatStats.Stats.Single(x => x.Stat == SkillStatType.Ballistic);
            //testedStat.Value.Should().Be(11);
        }
    }
}