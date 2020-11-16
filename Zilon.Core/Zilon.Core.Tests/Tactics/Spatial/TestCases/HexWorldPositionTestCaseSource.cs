using System.Collections;

using NUnit.Framework;

namespace Zilon.Core.Tests.Tactics.Spatial.TestCases
{
    public static class HexWorldPositionTestCaseSource
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(0, 0).Returns(new[]
                {
                    0f,
                    0f
                });
                yield return new TestCaseData(1, 0).Returns(new[]
                {
                    1f,
                    0f
                });
                yield return new TestCaseData(0, 1).Returns(new[]
                {
                    0.5f,
                    3f / 4f
                });
            }
        }
    }
}