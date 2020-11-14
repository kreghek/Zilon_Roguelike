using Zilon.Core.Components;

namespace Zilon.Core.Tests.Components
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PersonStatTests
    {
        /// <summary>
        ///     Тест проверяет, что характеристика на первом уровне при отсутствии бонусов равна базовой.
        /// </summary>
        [Test]
        public void GetActualValue_1LvlWithoutBonuses_ReturnBaseValue()
        {
            // ARRANGE
            const int baseValue = 10;
            const int incrementValue = 1;
            const int level = 1;
            const int expectedValue = baseValue;

            PersonStat personStat = new PersonStat(baseValue, incrementValue);


            // ACT
            float factValue = personStat.GetActualValue(level, 0);


            // ASSERT
            factValue.Should().Be(expectedValue);
        }

        /// <summary>
        ///     Тест проверяет, что характеристика на первом уровне + бонус равна базовому значению + бонус.
        /// </summary>
        [Test]
        public void GetActualValue_1LvlWithBonuses_ReturnBaseValuePlusBonuses()
        {
            // ARRANGE
            const int baseValue = 10;
            const int incrementValue = 1;
            const int bonusValue = 1;
            const int level = 1;
            const int expectedValue = baseValue + bonusValue;

            PersonStat personStat = new PersonStat(baseValue, incrementValue);

            PersonStat[] bonuses = {new PersonStat(bonusValue)};


            // ACT
            float factValue = personStat.GetActualValue(level,
                0,
                bonuses);


            // ASSERT
            factValue.Should().Be(expectedValue);
        }
    }
}