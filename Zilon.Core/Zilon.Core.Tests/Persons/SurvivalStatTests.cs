using System.Linq;

using FluentAssertions;
using NUnit.Framework;
using Zilon.Core.Persons;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class SurvivalStatTests
    {
        /// <summary>
        /// This is the range being checked.
        /// </ summary>
        [TestCase(0, -2, 2, 1, ExpectedResult = 1)]
        [TestCase(0, -2, 2, -2, ExpectedResult = -2)]
        public int ValueSetter_ValueInsideRange_ValueHaventBeCorrected(int start, int min, int max, int newValue)
        {
            // ARRANGE
            var survivalStat = new SurvivalStat(start, min, max)
            {
                // ACT
                Value = newValue
            };
            // ASSERT
            return survivalStat.Value;
        }

        /// <summary>
        /// This is the range being checked.
        /// </ summary>
        [TestCase(0, -2, 2, 3, ExpectedResult = 2)]
        [TestCase(0, -2, 2, -3, ExpectedResult = -2)]
        public int ValueSetter_ValueOutsideRange_ValueShiftedIntoRange(int start, int min, int max, int newValue)
        {
            // ARRANGE
            var survivalStat = new SurvivalStat(start, min, max)
            {
                // ACT
                Value = newValue
            };
            // ASSERT
            return survivalStat.Value;
        }
    }
}
