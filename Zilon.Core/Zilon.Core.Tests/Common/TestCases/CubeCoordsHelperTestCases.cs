using System.Collections;

using NUnit.Framework;

namespace Zilon.Core.Tests.Common.TestCases
{
    public static class CubeCoordsHelperTestCases
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(0, 0, 5, 7);
                yield return new TestCaseData(0, 0, 15, 17);
                yield return new TestCaseData(0, 4, 6, 0);
                yield return new TestCaseData(0, 4, 0, 2);
                yield return new TestCaseData(0, 3, 0, 3);
            }
        }
    }
}
