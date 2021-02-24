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
        [TestCaseSource(typeof(PerkHelperTestCaseSource), nameof(PerkHelperTestCaseSource.TestCases))]
        public void ConvertTotalLevel_FromTestCases_ReturnsCorrectLevelAndSublevel(IPerkScheme perkScheme, int testedTotalLevel, int expectedLevel, int expectedSubLevel)
        {
            // ACT
            PerkHelper.ConvertTotalLevel(perkScheme, testedTotalLevel,
                out var factLevel,
                out var factSubLevel);

            // ASSERT
            factLevel.Should().Be(expectedLevel);
            factSubLevel.Should().Be(expectedSubLevel);
        }
    }
}