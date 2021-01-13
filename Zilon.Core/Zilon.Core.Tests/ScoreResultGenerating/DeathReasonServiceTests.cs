using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Localization;
using Zilon.Core.Scoring;

namespace Zilon.Core.ScoreResultGenerating.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DeathReasonServiceTests
    {
        /// <summary>
        /// Test checks death reason service correctly describe all available death reason on all available languages.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(DeathReasonServiceTestCaseSource), nameof(DeathReasonServiceTestCaseSource.TestCases))]
        public void GetDeathReasonSummaryTest(IPlayerEvent playerEvent, Language language)
        {
            // ARRANGE

            var service = new DeathReasonService();

            // ACT
            var fact = service.GetDeathReasonSummary(playerEvent, language);

            // ASSERT
            fact.Should().NotBeNullOrEmpty();
        }
    }
}