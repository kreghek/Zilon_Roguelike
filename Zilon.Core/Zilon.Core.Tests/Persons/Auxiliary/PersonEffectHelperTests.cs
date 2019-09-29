//using System.Linq;

//using FluentAssertions;

//using Moq;

//using NUnit.Framework;

//using Zilon.Core.Persons;
//using Zilon.Core.Persons.Auxiliary;
//using Zilon.Core.Persons.Survival;

//namespace Zilon.Core.Tests.Persons.Auxiliary
//{
//    [TestFixture]
//    public class PersonEffectHelperTests
//    {
//        /// <summary>
//        /// Тест проверяет, что если значение увеличилось выше, чем ключевая точка,
//        /// то эффект изымается.
//        /// </summary>
//        [Test]
//        public void UpdateSurvivalEffect_HasLesserEffectAndValueMoreThatKetValueInNegativeKeyPoints_HasNoEffect()
//        {
//            //ARRANGE

//            var survivalRandomSource = CreateMaxRollsRandomSource();
//            var currentEffects = new EffectCollection();

//            var testedEffect = new SurvivalStatHazardEffect(SurvivalStatType.Satiety,
//                SurvivalStatHazardLevel.Lesser,
//                survivalRandomSource);

//            currentEffects.Add(testedEffect);

//            // Значение - 1, а ключевая точка - 0
//            var stat = new SurvivalStat(startValue: 1, min: -10, max: 10)
//            {
//                Type = SurvivalStatType.Satiety,
//                KeySegments = new[] {
//                    new SurvivalStatKeySegment(0.5f, 1f, SurvivalStatHazardLevel.Lesser)
//                }
//            };



//            // ACT
//            PersonEffectHelper.UpdateSurvivalEffect(currentEffects,
//                stat, 
//                stat.KeySegments,
//                survivalRandomSource);



//            // ASSERT
//            var factEffect = currentEffects.Items.OfType<SurvivalStatHazardEffect>()
//                .SingleOrDefault(x => x.Type == SurvivalStatType.Satiety);
//            factEffect.Should().BeNull();
//        }

//        /// <summary>
//        /// Тест проверяет, что если значение увеличилось выше, чем ключевая точка эффекта выше уровнем,
//        /// то эффект снижает уровень. Рассматриваем отрицательные значения ключевых точек.
//        /// </summary>
//        [Test]
//        public void UpdateSurvivalEffect_HasStrongEffectAndValueMoreThatKeyValueInNegativeKeyPoints_HasLesserEffect()
//        {
//            //ARRANGE

//            const SurvivalStatType expectedSurvivalHazardType = SurvivalStatType.Satiety;

//            var survivalRandomSource = CreateMaxRollsRandomSource();

//            var currentEffects = new EffectCollection();

//            var testedEffect = new SurvivalStatHazardEffect(expectedSurvivalHazardType,
//                SurvivalStatHazardLevel.Strong,
//                survivalRandomSource);

//            currentEffects.Add(testedEffect);

//            // Предполагаем, что первоначально эффект был Strong на -10.
//            // Затем значение изменилось на -5.
//            // Мы прошли ключевую точку -10 Strong на увеличение.
//            // Это должно снизить уровень эффекта на Lesser.
//            var stat = new SurvivalStat(startValue: -5, min: -10, max: 10)
//            {
//                Type = expectedSurvivalHazardType,
//                KeyPoints = new[] {
//                    new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, 0),
//                    new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, -10)
//                }
//            };



//            // ACT
//            PersonEffectHelper.UpdateSurvivalEffect(currentEffects,
//                stat,
//                new[] { stat.KeyPoints[1] },
//                survivalRandomSource);



//            // ASSERT
//            var factEffect = currentEffects.Items
//                .OfType<SurvivalStatHazardEffect>()
//                .Single(x => x.Type == expectedSurvivalHazardType);

//            factEffect.Level.Should().Be(SurvivalStatHazardLevel.Lesser);
//        }

//        /// <summary>
//        /// Тест проверяет, что если значение увеличилось выше, чем ключевая точка эффекта выше уровнем,
//        /// то эффект снижает уровень.
//        /// </summary>
//        [Test]
//        public void UpdateSurvivalEffect_HasMaxEffectAndValueMoreThatKeyValueInNegativeKeyPoints_HasStrongEffect()
//        {
//            //ARRANGE

//            const SurvivalStatType expectedSurvivalHazardType = SurvivalStatType.Satiety;

//            var survivalRandomSource = CreateMaxRollsRandomSource();

//            var currentEffects = new EffectCollection();

//            var testedEffect = new SurvivalStatHazardEffect(expectedSurvivalHazardType,
//                SurvivalStatHazardLevel.Max,
//                survivalRandomSource);

//            currentEffects.Add(testedEffect);

//            // Предполагаем, что первоначально эффект был Max на -10.
//            // Затем значение изменилось на -5.
//            // Мы прошли ключевую точку -10 Max на увеличение.
//            // Это должно снизить уровень эффекта на Strong.
//            var stat = new SurvivalStat(startValue: -5, min: -10, max: 10)
//            {
//                Type = expectedSurvivalHazardType,
//                KeyPoints = new[] {
//                    // Внимание!
//                    // Сейчас все ключевые чтоки должны быть либо слева от нуля включая ноль.
//                    // Либо справа от нуля.
//                    new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, 0),
//                    new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, -2),
//                    new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, -10)
//                }
//            };



//            // ACT
//            PersonEffectHelper.UpdateSurvivalEffect(currentEffects,
//                stat,
//                new[] { stat.KeyPoints[2] },
//                survivalRandomSource);



//            // ASSERT
//            var factEffect = currentEffects.Items
//                .OfType<SurvivalStatHazardEffect>()
//                .Single(x => x.Type == expectedSurvivalHazardType);

//            factEffect.Level.Should().Be(SurvivalStatHazardLevel.Strong);
//        }

//        /// <summary>
//        /// Тест проверяет, что при прохождении ключевой точки Max эффект понижает уровень.
//        /// Данный тест работает с положительными ключевыми точками.
//        /// </summary>
//        [Test]
//        public void UpdateSurvivalEffect_HasMaxEffectAndValueLessThatKeyValueInPositiveKeyPoints_HasStrongEffect()
//        {
//            //ARRANGE

//            const SurvivalStatType expectedSurvivalHazardType = SurvivalStatType.Intoxication;

//            var survivalRandomSource = CreateMaxRollsRandomSource();

//            var currentEffects = new EffectCollection();

//            var testedEffect = new SurvivalStatHazardEffect(expectedSurvivalHazardType,
//                SurvivalStatHazardLevel.Max,
//                survivalRandomSource);

//            currentEffects.Add(testedEffect);

//            // Предполагаем, что первоначально эффект был Max на 111.
//            // Затем значение изменилось на 110. Мы прошли ключевую точку 111 Max в сторону меньшего уровня.
//            // Это должно снизить уровень эффекта на Strong.
//            var stat = new SurvivalStat(110, 0, 150)
//            {
//                Type = expectedSurvivalHazardType,
//                KeyPoints = new[] {
//                    new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, 30),
//                    new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, 60),
//                    new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, 111)
//                }
//            };



//            // ACT
//            PersonEffectHelper.UpdateSurvivalEffect(currentEffects,
//                stat,
//                new[] { stat.KeyPoints[2] },
//                survivalRandomSource);



//            // ASSERT
//            var factEffect = currentEffects.Items
//                .OfType<SurvivalStatHazardEffect>()
//                .Single(x => x.Type == expectedSurvivalHazardType);

//            factEffect.Level.Should().Be(SurvivalStatHazardLevel.Strong);
//        }


//        private static ISurvivalRandomSource CreateMaxRollsRandomSource()
//        {
//            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
//            survivalRandomSourceMock.Setup(x => x.RollMaxHazardDamage()).Returns(6);
//            var survivalRandomSource = survivalRandomSourceMock.Object;

//            return survivalRandomSource;
//        }
//    }
//}