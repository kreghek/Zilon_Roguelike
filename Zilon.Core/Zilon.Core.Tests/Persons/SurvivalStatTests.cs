using NUnit.Framework;

using Zilon.Core.Persons.Survival;
using Zilon.Core.Tests.Persons.TestCases;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class SurvivalStatTests
    {
        /// <summary>
        /// Тест проверяет, что после изменения диапазона текущее значение изменяется пропорционально.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(SurvivalStatTestCasesSource), nameof(SurvivalStatTestCasesSource.RangeTestCases))]
        public int Value_IncrementDecrementRange_ExpectedResults(int startValue, int min, int max,
            int newMin, int newMax)
        {
            // ARRANGE
            var survivalStat = new SurvivalStat(startValue, min, max);

            // ACT
            survivalStat.ChangeStatRange(newMin, newMax);

            // ASSERT
            return survivalStat.Value;
        }

        /// <summary>
        /// Тест проверяет, что после добавление/вычитания целого значения
        /// получается ожидаемый результат.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(SurvivalStatTestCasesSource), nameof(SurvivalStatTestCasesSource.ValueTestCases))]
        public int Value_IncrementDecrementValue_ExpectedResults(int startValue, int min, int max, int diffValue)
        {
            // ARRANGE
            var survivalStat = new SurvivalStat(startValue, min, max);

            // ACT
            survivalStat.Value += diffValue;

            // ASSERT
            return survivalStat.Value;
        }
    }
}