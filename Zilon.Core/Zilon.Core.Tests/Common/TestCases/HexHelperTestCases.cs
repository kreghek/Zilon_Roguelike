using System.Collections;

using NUnit.Framework;

namespace Zilon.Core.Tests.Common.TestCases
{
    public static class HexHelperTestCases
    {
        public static IEnumerable TestCases
        {
            get
            {
                // Params:
                // world x coordinate
                // world Y coordinate
                // hex size in the world. Size is distance from center to a edge.

                yield return new TestCaseData(0, 0, 1).Returns(new OffsetCoords(0, 0));
                yield return new TestCaseData(0, 0, 50).Returns(new OffsetCoords(0, 0));
                yield return new TestCaseData(50, 50, 50).Returns(new OffsetCoords(0, 1));
                yield return new TestCaseData(3 * 50, 50, 50).Returns(new OffsetCoords(1, 1));
            }
        }
    }
}