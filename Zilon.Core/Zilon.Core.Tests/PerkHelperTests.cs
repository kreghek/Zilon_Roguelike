using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PerkHelperTests
    {
        [Test]
        [TestCaseSource(typeof(PerkHelperTestCaseSource), nameof(PerkHelperTestCaseSource.PositiveTestCases))]
        public void ConvertTotalLevel_FromTestCases_ReturnsCorrectLevelAndSublevel(IPerkScheme perkScheme,
            int testedTotalLevel, int expectedLevel, int expectedSubLevel)
        {
            // ACT
            var perkLevel = PerkHelper.ConvertTotalLevel(perkScheme, testedTotalLevel);

            // ASSERT
            perkLevel.Primary.Should().Be(expectedLevel);
            perkLevel.Sub.Should().Be(expectedSubLevel);
        }

        [Test]
        [TestCaseSource(typeof(PerkHelperTestCaseSource), nameof(PerkHelperTestCaseSource.ExceptonTestCases))]
        public void ConvertTotalLevel_FromTestCases_ThrowsExceptions(IPerkScheme perkScheme,
            int testedTotalLevel)
        {
            // ACT
            Action act = () =>
            {
                var _ = PerkHelper.ConvertTotalLevel(perkScheme, testedTotalLevel);
            };

            // ASSERT
            act.Should().Throw<Exception>();
        }
    }
}