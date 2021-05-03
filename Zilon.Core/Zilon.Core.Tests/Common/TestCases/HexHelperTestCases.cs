using System;
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
                static float sqrt(float a)
                {
                    return (float)Math.Sqrt(a);
                }

                static int round(float a)
                {
                    return (int)Math.Round(a, MidpointRounding.ToEven);
                }

                // Step left is pointy top hex width on x-axis.
                // Step depends on hex size (radius of covered round).
                static int stepLeft(float size)
                {
                    return round(size * sqrt(3));
                }

                // Step top is vertical spacing between hex.
                static int stepTop(float size)
                {
                    return round(size * 2 * 3f / 4f);
                }

                const int HEX_SIZE = 50;

                // Params:
                // world x coordinate
                // world Y coordinate
                // hex size in the world. Size is distance from center to a edge.

                yield return new TestCaseData(0, 0, 1).Returns(new OffsetCoords(0, 0));
                yield return new TestCaseData(0, 0, HEX_SIZE).Returns(new OffsetCoords(0, 0));
                yield return new TestCaseData(stepLeft(HEX_SIZE), 0, HEX_SIZE).Returns(new OffsetCoords(1, 0));

                yield return new TestCaseData(stepLeft(HEX_SIZE) / 2, stepTop(HEX_SIZE), HEX_SIZE).Returns(
                    new OffsetCoords(0, 1));
                yield return new TestCaseData(stepLeft(HEX_SIZE) + (stepLeft(HEX_SIZE) / 2), stepTop(HEX_SIZE),
                    HEX_SIZE).Returns(
                    new OffsetCoords(1, 1));
                yield return new TestCaseData((3 * stepLeft(HEX_SIZE)) + (stepLeft(HEX_SIZE) / 2), stepTop(HEX_SIZE),
                    HEX_SIZE).Returns(
                    new OffsetCoords(3, 1));

                yield return new TestCaseData(0, 2 * stepTop(HEX_SIZE), HEX_SIZE).Returns(new OffsetCoords(0, 2));

                yield return new TestCaseData(0, 4 * stepTop(HEX_SIZE), HEX_SIZE).Returns(new OffsetCoords(0, 4));
            }
        }
    }
}