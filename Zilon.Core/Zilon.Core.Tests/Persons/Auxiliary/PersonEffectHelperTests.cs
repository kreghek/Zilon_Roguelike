using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Persons.Auxiliary;

namespace Zilon.Core.Tests.Persons.Auxiliary
{
    [TestFixture()]
    public class PersonEffectHelperTests
    {
        /// <summary>
        /// Тест проверяет, что если значение увеличилось выше, чем ключевая точка,
        /// то эффект изымается.
        /// </summary>
        [Test()]
        public void UpdateSurvivalEffect_HasLesserEffectAndValueMoreThatKetValue_HasNoEffect()
        {
            //ARRANGE

            var currentEffects = new EffectCollection();

            var testedEffect = new SurvivalStatHazardEffect(SurvivalStatType.Satiety, SurvivalStatHazardLevel.Lesser);

            currentEffects.Add(testedEffect);

            var stat = new SurvivalStat {
                Type = SurvivalStatType.Satiety,
                Value = 1,
                KeyPoints = new[] {
                    new SurvivalStatKeyPoint{
                        Level = SurvivalStatHazardLevel.Lesser,
                        Value = 0
                    }
                }
            };



            // ACT
            PersonEffectHelper.UpdateSurvivalEffect(currentEffects, stat, stat.KeyPoints[0]);



            // ASSERT
            var factEffect = currentEffects.Items.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == SurvivalStatType.Satiety);
            factEffect.Should().BeNull();
        }

        /// <summary>
        /// Тест проверяет, что если значение увеличилось выше, чем ключевая точка эффекта выше уровнем,
        /// то эффект снижает уровень.
        /// </summary>
        [Test]
        public void UpdateSurvivalEffect_HasStrongEffectAndValueMoreThatKetValue_HasLesserEffect()
        {
            //ARRANGE

            var currentEffects = new EffectCollection();

            var testedEffect = new SurvivalStatHazardEffect(SurvivalStatType.Satiety, SurvivalStatHazardLevel.Lesser);

            currentEffects.Add(testedEffect);

            var stat = new SurvivalStat
            {
                Type = SurvivalStatType.Satiety,
                Value = -5,
                KeyPoints = new[] {
                    new SurvivalStatKeyPoint{
                        Level = SurvivalStatHazardLevel.Lesser,
                        Value = 0
                    },
                    new SurvivalStatKeyPoint{
                        Level = SurvivalStatHazardLevel.Strong,
                        Value = -10
                    }
                }
            };



            // ACT
            PersonEffectHelper.UpdateSurvivalEffect(currentEffects, stat, stat.KeyPoints[1]);



            // ASSERT
            var factEffect = currentEffects.Items.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == SurvivalStatType.Satiety);

            factEffect.Level.Should().Be(SurvivalStatHazardLevel.Lesser);
        }
    }
}