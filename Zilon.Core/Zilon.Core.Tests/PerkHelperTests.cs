﻿using System;

using FluentAssertions;

using NUnit.Framework;

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
            nameof(PerkHelperTestCaseSource.ConvertLevelSubsToTotalExceptionTestCases))]
        public void ConvertLevelSubsToTotal_FromExceptionTestCases_ThrowsException(IPerkScheme perkScheme,
            int primaryLevel, int subLevel)
        {
            // ACT
            Action act = () =>
            {
                PerkHelper.ConvertLevelSubsToTotal(perkScheme, primaryLevel, subLevel);
            };

            act.Should().Throw<Exception>();
        }

        [Test]
        [TestCaseSource(typeof(PerkHelperTestCaseSource),
            nameof(PerkHelperTestCaseSource.ConvertLevelSubsToTotalPositiveTestCases))]
        public int ConvertLevelSubsToTotal_FromPositiveTestCases_ReturnsCorrectFact(IPerkScheme perkScheme,
            int primaryLevel, int subLevel)
        {
            // ACT
            var factTotal = PerkHelper.ConvertLevelSubsToTotal(perkScheme, primaryLevel, subLevel);

            // ASSERT
            return factTotal;
        }

        [Test]
        [TestCaseSource(typeof(PerkHelperTestCaseSource),
            nameof(PerkHelperTestCaseSource.ConvertTotalLevelPositiveTestCases))]
        public void ConvertTotalLevelToLevelSubs_FromTestCases_ReturnsCorrectLevelAndSublevel(IPerkScheme perkScheme,
            int testedTotalLevel, int expectedLevel, int expectedSubLevel)
        {
            // ACT
            var isSuccess = PerkHelper.TryConvertTotalLevelToLevelSubs(perkScheme, testedTotalLevel, out var perkLevel);

            // ASSERT
            isSuccess.Should().BeTrue();
            perkLevel.Primary.Should().Be(expectedLevel);
            perkLevel.Sub.Should().Be(expectedSubLevel);
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