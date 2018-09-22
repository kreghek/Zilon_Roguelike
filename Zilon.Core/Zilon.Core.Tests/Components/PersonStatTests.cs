using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Components;

namespace Zilon.Core.Tests.Components
{
    [TestFixture]
    public class PersonStatTests
    {
        /// <summary>
        /// Тест проверяет, что характеристика на первом уровне при отсутствии бонусов равна базовой.
        /// </summary>
        [Test]
        public void GetActualValue_1LvlWithoutBonuses_ReturnBaseValue()
        {
            // ARRANGE
            const int expectedValue = 10;
            const int baseValue = 10;
            const int incrementValue = 1;

            var personStat = new PersonStat
            {
                Base = baseValue,
                LevelInc = incrementValue
            };



            // ACT
            var factValue = personStat.GetActualValue(1, 0);



            // ASSERT
            factValue.Should().Be(expectedValue);
        }
    }
}