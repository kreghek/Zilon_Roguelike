using System.Collections;
using NUnit.Framework;

namespace Zilon.Core.Tests.Tactics.Spatial.TestCases
{
    public static class ConvertOffsetToCubeTestCaseSource
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(1, 2, 0, -2, 2);
                yield return new TestCaseData(2, 1, 2, -3, 1);
                yield return new TestCaseData(1, 0, 1, -1, 0);
                yield return new TestCaseData(1, 1, 1, -2, 1);
                yield return new TestCaseData(1, 4, -1, -3, 4);
            }
        }
    }
}