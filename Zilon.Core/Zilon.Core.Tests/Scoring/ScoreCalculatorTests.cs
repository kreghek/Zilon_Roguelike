using FluentAssertions;

using NUnit.Framework;

namespace Zilon.Core.Scoring.Tests
{
    [TestFixture]
    public class ScoreCalculatorTests
    {
        /// <summary>
        /// Test checks that turns converted to days and hours correctly.
        /// </summary>
        [Test]
        public void ConvertTurnsToDetailed_PositiveTurns_ReturnsExpectedDetailed()
        {
            // ARRANGE
            var expected = new DetailedLifetime(0, 0);

            // ACT
            var fact = ScoreCalculator.ConvertTurnsToDetailed(0);

            // ASSERT
            fact.Should().Be(expected);
        }
    }
}