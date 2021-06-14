using System.Collections;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    public static class UsePropHelperTestCaseSource
    {
        public static IEnumerable NoCriticalEffectTestCases
        {
            get
            {
                yield return new TestCaseData(SurvivalStatType.Satiety, UsageRestrictionRule.NoStarvation);
                yield return new TestCaseData(SurvivalStatType.Hydration, UsageRestrictionRule.NoDehydration);
                yield return new TestCaseData(SurvivalStatType.Intoxication, UsageRestrictionRule.NoOverdose);
            }
        }
    }
}