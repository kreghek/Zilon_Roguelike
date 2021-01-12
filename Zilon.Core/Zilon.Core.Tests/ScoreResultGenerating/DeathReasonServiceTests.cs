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