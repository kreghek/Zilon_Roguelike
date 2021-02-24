using System.Collections;

using NUnit.Framework;

using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests
{
    public static class PerkHelperTestCaseSource
    {
        public static IEnumerable TestCases
        {
            get
            {
                // IPerkScheme which used to convert level
                // int testedTotalLevel - the level to convert level/sub in the perk scheme
                // int expectedLevel - primary result level
                // int expectedSubLevel - primari result sub

                yield return new TestCaseData(CreateTestPerkScheme523(), 2, 1, 2);

                yield return new TestCaseData(CreateTestPerkScheme523(), 6, 2, 1);
            }
        }

        private static IPerkScheme CreateTestPerkScheme523()
        {
            return new TestPerkScheme
            {
                Levels = new[]
                {
                    new PerkLevelSubScheme
                    {
                        MaxValue = 5
                    },
                    new PerkLevelSubScheme
                    {
                        MaxValue = 2
                    },
                    new PerkLevelSubScheme
                    {
                        MaxValue = 3
                    }
                }
            };
        }
    }
}