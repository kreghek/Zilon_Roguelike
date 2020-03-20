using System.Collections;

using NUnit.Framework;

namespace Zilon.CoreTestsTemp.Tactics.Behaviour.TestCases
{
    public static class FowHelperTestCaseDataSource
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(5, 2, 2, 1);
                yield return new TestCaseData(20, 10, 10, 5);
                yield return new TestCaseData(100, 50, 50, 5);
                yield return new TestCaseData(1000, 50, 50, 5);
            }
        }
    }
}
