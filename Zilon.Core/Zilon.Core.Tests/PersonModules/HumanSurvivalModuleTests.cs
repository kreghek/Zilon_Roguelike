﻿using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.Tests.Persons.TestCases;

namespace Zilon.Core.Tests.PersonModules
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class HumanSurvivalModuleTests
    {
        /// <summary>
        /// Проверяет, что при конституции больше базового значения увеличивается запас ХП.
        /// </summary>
        [Test]
        public void Constructor_ConstitutionAboveBase_HpIncreased()
        {
            // ARRANGE

            const int PERSON_HP = 1;
            const int EXPECTED_HP = PERSON_HP + 2;
            const int CONSTITUTION_BASE = 10;
            const int CONSTITUTION_VALUE = CONSTITUTION_BASE + 1;

            var personScheme = CreatePersonScheme();
            var survivalRandomSource = CreateSurvivalRandomSource();

            personScheme.Hp = PERSON_HP;

            var attributesModuleMock = new Mock<IAttributesModule>();
            attributesModuleMock.Setup(x => x.GetAttribute(PersonAttributeType.Constitution))
                .Returns(new PersonAttribute(PersonAttributeType.Constitution, CONSTITUTION_VALUE));
            var attributesModule = attributesModuleMock.Object;

            // ACT
            var survivalData = new HumanSurvivalModule(personScheme, survivalRandomSource, attributesModule);

            // ASSERT
            survivalData.Stats.Single(x => x.Type == SurvivalStatType.Health).Value.Should().Be(EXPECTED_HP);
        }

        /// <summary>
        /// Проверяет, что при конституции ниже базового значения увеличивается запас ХП.
        /// </summary>
        [Test]
        public void Constructor_ConstitutionBelowBase_HpDecreased()
        {
            // ARRANGE

            const int PERSON_HP = 3;
            const int EXPECTED_HP = PERSON_HP - 2;
            const int CONSTITUTION_BASE = 10;
            const int CONSTITUTION_VALUE = CONSTITUTION_BASE - 1;

            var personScheme = CreatePersonScheme();
            var survivalRandomSource = CreateSurvivalRandomSource();

            personScheme.Hp = PERSON_HP;

            var attributesModuleMock = new Mock<IAttributesModule>();
            attributesModuleMock.Setup(x => x.GetAttribute(PersonAttributeType.Constitution))
                .Returns(new PersonAttribute(PersonAttributeType.Constitution, CONSTITUTION_VALUE));
            var attributesModule = attributesModuleMock.Object;

            // ACT
            var survivalData = new HumanSurvivalModule(personScheme, survivalRandomSource, attributesModule);

            // ASSERT
            survivalData.Stats.Single(x => x.Type == SurvivalStatType.Health).Value.Should().Be(EXPECTED_HP);
        }

        public static IPersonScheme CreatePersonScheme()
        {
            var personScheme = new TestPersonScheme
            {
                SurvivalStats = new[]
                {
                    new TestPersonSurvivalStatSubScheme
                    {
                        Type = PersonSurvivalStatType.Satiety,
                        MinValue = -100,
                        MaxValue = 100,
                        StartValue = 0,
                        KeyPoints = new[]
                        {
                            new TestPersonSurvivalStatKeySegmentSubScheme
                            {
                                Level = PersonSurvivalStatKeypointLevel.Lesser,
                                Start = 0.25f,
                                End = 0.75f
                            },
                            new TestPersonSurvivalStatKeySegmentSubScheme
                            {
                                Level = PersonSurvivalStatKeypointLevel.Strong,
                                Start = 0.12f,
                                End = 0.25f
                            },
                            new TestPersonSurvivalStatKeySegmentSubScheme
                            {
                                Level = PersonSurvivalStatKeypointLevel.Max,
                                Start = 0,
                                End = 0.12f
                            }
                        }
                    },

                    new TestPersonSurvivalStatSubScheme
                    {
                        Type = PersonSurvivalStatType.Hydration,
                        MinValue = -100,
                        MaxValue = 100,
                        StartValue = 0,
                        KeyPoints = new[]
                        {
                            new TestPersonSurvivalStatKeySegmentSubScheme
                            {
                                Level = PersonSurvivalStatKeypointLevel.Lesser,
                                Start = 0.25f,
                                End = 0.75f
                            },
                            new TestPersonSurvivalStatKeySegmentSubScheme
                            {
                                Level = PersonSurvivalStatKeypointLevel.Strong,
                                Start = 0.12f,
                                End = 0.25f
                            },
                            new TestPersonSurvivalStatKeySegmentSubScheme
                            {
                                Level = PersonSurvivalStatKeypointLevel.Max,
                                Start = 0,
                                End = 0.12f
                            }
                        }
                    }
                }
            };

            return personScheme;
        }

        /// <summary>
        /// Тест проверяет, что при восстановлении Хп текущее значение не выходит за рамки максимального.
        /// </summary>
        [Test]
        public void RestoreHp_RestoreHp_HpNotGreaterThatMaxPersonHp()
        {
            // ARRANGE
            const int initialHp = 2;
            const int personHp = 3;
            const int restoreHpValue = 2;
            const int expectedHp = personHp;

            var personScheme = CreatePersonScheme();
            var survivalRandomSource = CreateSurvivalRandomSource();

            personScheme.Hp = personHp;

            var survivalData = CreateSurvivalData(personScheme, survivalRandomSource);

            var stat = survivalData.Stats.Single(x => x.Type == SurvivalStatType.Health);
            stat.Value = initialHp;

            // ACT
            survivalData.RestoreStat(SurvivalStatType.Health, restoreHpValue);

            // ASSERT
            var factStat = survivalData.Stats.Single(x => x.Type == SurvivalStatType.Health);
            factStat.Value.Should().Be(expectedHp);
        }

        /// <summary>
        /// Проверяет, что при потере всего здоровья выстреливает событие смерти.
        /// </summary>
        [Test]
        public void TakeDamage_FatalDamage_FiresEvent()
        {
            // ARRANGE

            var personScheme = CreatePersonScheme();
            var survivalRandomSource = CreateSurvivalRandomSource();

            const int personHp = 1;
            const int damageValue = 2;

            personScheme.Hp = personHp;

            var survivalData = CreateSurvivalData(personScheme, survivalRandomSource);

            // ACT
            using var monitor = survivalData.Monitor();
            survivalData.DecreaseStat(SurvivalStatType.Health, damageValue);

            // ASSERT
            monitor.Should().Raise(nameof(ISurvivalModule.Dead));
        }

        /// <summary>
        /// Тест проверяет, что характеристика с изменённым DownPass корректно
        /// изменяется при указанных результатах броска кости.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(SurvivalDataTestCasesSource), nameof(SurvivalDataTestCasesSource.DownPassTestCases))]
        public int Update_ModifiedDownPass_StatDownCorrectly(int statDownPass, int downPassRoll)
        {
            // ARRANGE

            const int STAT_RATE = 1;
            const int MIN_STAT_VALUE = 0;
            const int MAX_STAT_VALUE = 1;
            const int START_STAT_VALUE = MAX_STAT_VALUE;
            const SurvivalStatType STAT_TYPE = SurvivalStatType.Satiety;

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            survivalRandomSourceMock.Setup(x => x.RollSurvival(It.IsAny<SurvivalStat>()))
                .Returns(downPassRoll);
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var survivalStats = new[]
            {
                new SurvivalStat(START_STAT_VALUE, MIN_STAT_VALUE, MAX_STAT_VALUE)
                {
                    Type = STAT_TYPE,
                    Rate = STAT_RATE,
                    DownPassRoll = statDownPass
                }
            };

            var survivalData = new HumanSurvivalModule(survivalStats, survivalRandomSource);

            // ACT
            survivalData.Update();

            // ASSERT
            return survivalStats[0].Value;
        }

        private static ISurvivalModule CreateSurvivalData(IPersonScheme personScheme,
            ISurvivalRandomSource survivalRandomSource)
        {
            var attributesModuleMock = new Mock<IAttributesModule>();
            var attributesModule = attributesModuleMock.Object;

            var survivalData = new HumanSurvivalModule(personScheme, survivalRandomSource, attributesModule);
            return survivalData;
        }

        private static ISurvivalRandomSource CreateSurvivalRandomSource()
        {
            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            return survivalRandomSourceMock.Object;
        }
    }
}