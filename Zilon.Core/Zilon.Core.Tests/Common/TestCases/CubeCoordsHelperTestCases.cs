using NUnit.Framework;

using System.Collections;

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
            }
        }
    }
}
