using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Components;

namespace Zilon.Core.LogicCalculations.Tests
{
    [TestFixture]
    public class RuleCalculationsTests
    {
        /// <summary>
        /// Test checks no exceptions in result of adding new levels.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(RuleCalculationsTestCaseSource), nameof(RuleCalculationsTestCaseSource.AllKnownLevels))]
        public void CalcEfficientByRuleLevel_AllKnownLevels_NotThrowException(int currentModifier, PersonRuleLevel level)
        {
            // ACT
            Action act = () =>
            {
                var _ = RuleCalculations.CalcEfficientByRuleLevel(currentModifier, level);
            };

            // ASSERT
            act.Should().NotThrow<NotSupportedException>();
        }
    }
}