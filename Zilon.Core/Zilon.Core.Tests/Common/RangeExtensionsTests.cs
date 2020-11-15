using Zilon.Core.Common;

namespace Zilon.Core.Tests.Common
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class RangeExtensionsTests
    {
        /// <summary>
        /// This is the range being checked.
        /// </summary>
        [TestCase(-2, 2, 1, ExpectedResult = 1)]
        [TestCase(-2, 2, -2, ExpectedResult = -2)]
        public int GetBounded_ValueInsideRange_ValueHaventBeCorrected(int min, int max, int value)
        {
            // ARRANGE
            var range = new Range<int>(min, max);

            // ACT
            var factValue = range.GetBounded(value);

            // ASSERT
            return factValue;
        }

        /// <summary>
        /// This is the range being checked.
        /// </summary>
        [TestCase(-2, 2, 3, ExpectedResult = 2)]
        [TestCase(-2, 2, -3, ExpectedResult = -2)]
        [TestCase(-2, 2, -30, ExpectedResult = -2)]
        public int ValueSetter_ValueOutsideRange_ValueShiftedIntoRange(int min, int max, int value)
        {
            // ARRANGE
            var range = new Range<int>(min, max);

            // ACT
            var factValue = range.GetBounded(value);

            // ASSERT
            return factValue;
        }
    }
}