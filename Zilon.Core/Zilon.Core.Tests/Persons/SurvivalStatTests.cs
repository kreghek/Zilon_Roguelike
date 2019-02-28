
using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Tests.Persons.TestCases;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class SurvivalStatTests
    {
        /// <summary>
        /// Тест проверяет, что после добавление/вычитания целого значения
        /// получается ожидаемый результат.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(SurvivalStatTestCasesSource), nameof(SurvivalStatTestCasesSource.TestCases))]
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