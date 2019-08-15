using System.Collections;

using NUnit.Framework;

namespace Zilon.Core.Tests.World
{
    public static class GlobeHelperTestCaseSource
    {
        public static IEnumerable TestCases {
            get {
                yield return new TestCaseData(3, 1, 1);
                yield return new TestCaseData(2, 1, 1);
                yield return new TestCaseData(10, 5, 5);
                yield return new TestCaseData(11, 5, 5);
                yield return new TestCaseData(6, 3, 3);
            }
        }
    }
}
