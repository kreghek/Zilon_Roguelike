using System.Collections;

namespace Zilon.Core.Tests.Persons.TestCases
{
    internal static class SurvivalStatTestCasesSource
    {
        public static IEnumerable RangeTestCases
        {
            get
            {
                yield return new TestCaseData(5, 0, 10, 0, 13).Returns(6);

                yield return new TestCaseData(1, 1, 3, 1, 1).Returns(1);
            }
        }

        public static IEnumerable ValueTestCases
        {
            get
            {
                yield return new TestCaseData(0, 0, 10, 1).Returns(1);
                yield return new TestCaseData(0, 0, 13, 1).Returns(1);
                yield return new TestCaseData(0, 0, 26, 1).Returns(1);
                yield return new TestCaseData(0, 0, 7, 1).Returns(1);
                yield return new TestCaseData(10, 0, 10, -1).Returns(9);
                yield return new TestCaseData(13, 0, 13, -1).Returns(12);
                yield return new TestCaseData(99, 0, 99, -1).Returns(98);

                yield return new TestCaseData(0, 0, 10, 3).Returns(3);
                yield return new TestCaseData(1, 1, 10, 3).Returns(4);
            }
        }
    }
}