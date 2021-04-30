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
                // hex width in the world
                // hex height in the world

                yield return new TestCaseData(0, 0, 1, 1).Returns(new OffsetCoords(0, 0));
                yield return new TestCaseData(0, 0, 50, 50).Returns(new OffsetCoords(0, 0));
                yield return new TestCaseData(50, 0, 50, 50).Returns(new OffsetCoords(1, 0));
            }
        }
    }
}