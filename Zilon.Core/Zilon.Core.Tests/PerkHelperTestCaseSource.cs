using System;
using System.Collections;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests
{
    public static class PerkHelperTestCaseSource
    {
        /// <summary>
        /// Test cases wich fails checks.
        /// </summary>
        public static IEnumerable ConvertTotalLevelExceptonTestCases
        {
            get
            {
                // IPerkScheme which used to convert level
                // int testedTotalLevel - the level to convert level/sub in the perk scheme

                yield return new TestCaseData(null, 0)
                    .SetDescription("Fails because scheme is null");

                yield return new TestCaseData(CreateTestPerkScheme523(), 0)
                    .SetDescription("Fails because total is empty");

                yield return new TestCaseData(new TestPerkScheme { Levels = Array.Empty<PerkLevelSubScheme>() }, 0)
                    .SetDescription("Fails because scheme has no levels.");

                yield return new TestCaseData(
                        new TestPerkScheme { Levels = new[] { new PerkLevelSubScheme { MaxValue = 0 } } }, 1)
                    .SetDescription("Fails because subs sum is zero.");

                yield return new TestCaseData(
                        new TestPerkScheme { Levels = new[] { new PerkLevelSubScheme { MaxValue = -1 } } }, 1)
                    .SetDescription("Fails because one of the subs is negative.");

                yield return new TestCaseData(
                        new TestPerkScheme { Levels = new[] { new PerkLevelSubScheme { MaxValue = 1 } } }, 2)
                    .SetDescription("Fails because subs sum less that total.");
            }
        }

        public static IEnumerable ConvertTotalLevelPositiveTestCases
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

        public static IEnumerable GetNextLevelTestCases
        {
            get
            {
                // IPerkScheme which used to convert level
                // Current level
                // Expected next level

                yield return new TestCaseData(CreateTestPerkScheme523(), new PerkLevel(1, 1), new PerkLevel(1, 2));
                yield return new TestCaseData(CreateTestPerkScheme523(), new PerkLevel(1, 5), new PerkLevel(2, 1));
                yield return new TestCaseData(CreateTestPerkScheme111(), new PerkLevel(1, 1), new PerkLevel(2, 1));
            }
        }

        public static IEnumerable HasNoNextLevelTestCases
        {
            get
            {
                yield return new TestCaseData(CreateTestPerkScheme111(), new PerkLevel(3, 1));
                yield return new TestCaseData(CreateTestPerkScheme523(), new PerkLevel(3, 3));
            }
        }

        private static IPerkScheme CreateTestPerkScheme111()
        {
            return new TestPerkScheme
            {
                SystemDescription = "1-1-1",
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
                SystemDescription = "5-2-3",
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