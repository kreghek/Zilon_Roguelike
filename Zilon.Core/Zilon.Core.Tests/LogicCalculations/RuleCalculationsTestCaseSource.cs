using System;
using System.Collections;
using System.Linq;

using NUnit.Framework;

using Zilon.Core.Components;

namespace Zilon.Core.LogicCalculations.Tests
{
    public static class RuleCalculationsTestCaseSource
    {
        public static IEnumerable AllKnownLevels
        {
            get
            {
                var levels = Enum.GetValues(typeof(PersonRuleLevel)).Cast<PersonRuleLevel>()
                    .Where(x => x != PersonRuleLevel.None);
                foreach (var level in levels)
                {
                    yield return new TestCaseData(0, level);
                }
            }
        }
    }
}