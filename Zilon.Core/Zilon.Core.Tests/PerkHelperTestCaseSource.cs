using System;
using System.Collections;

using NUnit.Framework;

using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests
{
    public static class PerkHelperTestCaseSource
    {
        /// <summary>
        /// Test cases wich fails ckecks.
        /// </summary>
        public static IEnumerable ExceptonTestCases
        {
            get
            {
                // IPerkScheme which used to convert level
                // int testedTotalLevel - the level to convert level/sub in the perk scheme

                yield return new TestCaseData(null, 0);
                yield return new TestCaseData(CreateTestPerkScheme523(), 0);
                yield return new TestCaseData(new TestPerkScheme { Levels = Array.Empty<PerkLevelSubScheme>() }, 0);
                yield return new TestCaseData(
                    new TestPerkScheme { Levels = new[] { new PerkLevelSubScheme { MaxValue = 0 } } }, 1);
                yield return new TestCaseData(
                    new TestPerkScheme { Levels = new[] { new PerkLevelSubScheme { MaxValue = 1 } } }, 2);
            }
        }

        public static IEnumerable PositiveTestCases
        {
            get
            {
                // IPerkScheme which used to convert level
                // int testedTotalLevel - the level to convert level/sub in the perk scheme
                // int expectedLevel - primary result level
                // int expectedSubLevel - primari result sub

                yield return new TestCaseData(CreateTestPerkScheme523(), 2, 1, 2);

                yield return new TestCaseData(CreateTestPerkScheme523(), 6, 2, 1);

                yield return new TestCaseData(CreateTestPerkScheme523(), 1, 1, 1);

                yield return new TestCaseData(CreateTestPerkScheme523(), 3, 1, 3);

                yield return new TestCaseData(CreateTestPerkScheme523(), 5, 1, 5);

                yield return new TestCaseData(CreateTestPerkScheme111(), 2, 2, 1);

                yield return new TestCaseData(CreateTestPerkScheme111(), 3, 3, 1);
            }
        }

        private static IPerkScheme CreateTestPerkScheme111()
        {
            return new TestPerkScheme
            {
                Levels = new[]
                {
                    new PerkLevelSubScheme
                    {
                        MaxValue = 1
                    },
                    new PerkLevelSubScheme
                    {
                        MaxValue = 1
                    },
                    new PerkLevelSubScheme
                    {
                        MaxValue = 1
                    }
                }
            };
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