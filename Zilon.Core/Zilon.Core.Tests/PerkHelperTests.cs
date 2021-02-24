using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PerkHelperTests
    {
        [Test]
        [TestCaseSource(typeof(PerkHelperTestCaseSource),
            nameof(PerkHelperTestCaseSource.ConvertTotalLevelPositiveTestCases))]
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
        [TestCaseSource(typeof(PerkHelperTestCaseSource),
            nameof(PerkHelperTestCaseSource.ConvertTotalLevelExceptonTestCases))]
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

        [Test]
        [TestCaseSource(typeof(PerkHelperTestCaseSource),
            nameof(PerkHelperTestCaseSource.GetNextLevelTestCases))]
        public void GetNextLevel_FromTestCases_ReturnsCorrectNextLevel(IPerkScheme perkScheme, PerkLevel currentLevel,
            PerkLevel expectedNextLevel)
        {
            // ACT
            var nextLevel = PerkHelper.GetNextLevel(perkScheme, currentLevel);

            // ASSERT
            nextLevel.Primary.Should().Be(expectedNextLevel.Primary);
            nextLevel.Sub.Should().Be(expectedNextLevel.Sub);
        }

        [Test]
        [TestCaseSource(typeof(PerkHelperTestCaseSource),
            nameof(PerkHelperTestCaseSource.HasNoNextLevelTestCases))]
        public void HasNextLevel_FromNegativeTestCases_AlwaysReturnsFalse(IPerkScheme perkScheme,
            PerkLevel currentLevel)
        {
            // ACT
            var factHasNextLevel = PerkHelper.HasNextLevel(perkScheme, currentLevel);

            // ASSERT
            factHasNextLevel.Should().BeFalse();
        }
    }
}