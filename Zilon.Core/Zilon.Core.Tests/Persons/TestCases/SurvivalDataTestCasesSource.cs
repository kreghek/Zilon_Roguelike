using System.Collections;

using NUnit.Framework;

namespace Zilon.Core.Tests.Persons.TestCases
{
    static class SurvivalDataTestCasesSource
    {
        public static IEnumerable DownPassTestCases
        {
            get
            {
                // int statDownPass - DownPass характеристики
                // int downPassRoll - Фактическое значение броска
                // Возвращает итоговое значение характеристики.

                yield return new TestCaseData(4, 4).Returns(1);
                yield return new TestCaseData(3, 4).Returns(1);
                yield return new TestCaseData(4, 5).Returns(1);

                yield return new TestCaseData(4, 3).Returns(0);
            }
        }
    }
}