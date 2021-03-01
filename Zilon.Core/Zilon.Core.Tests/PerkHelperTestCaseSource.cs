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
        public static IEnumerable ConvertLevelSubsToTotalExceptionTestCases
        {
            get
            {
                // IPerkScheme which used to convert level
                // level
                // sub level

                yield return new TestCaseData(null, 1, 1)
                    .SetDescription("Fails because scheme is null");

                yield return new TestCaseData(CreateTestPerkScheme523(), 3, 4)
                    .SetDescription("Fails because subs is too big.");

                yield return new TestCaseData(CreateTestPerkScheme523(), 4, 1)
                    .SetDescription("Fails because primary level is too big.");
            }
        }

        public static IEnumerable ConvertLevelSubsToTotalPositiveTestCases
        {
            get
            {
                // IPerkScheme which used to convert level
                // level
                // sub level
                // Returns expected total

                yield return new TestCaseData(CreateTestPerkScheme523(), 1, 1).Returns(1);
                yield return new TestCaseData(CreateTestPerkScheme523(), 3, 3).Returns(10);
                yield return new TestCaseData(CreateTestPerkScheme111(), 3, 1).Returns(3);
            }
        }

        /// <summary>
        /// Test cases wich fails checks.
        /// </summary>
        public static IEnumerable ConvertTotalLevelArgumentExceptonTestCases
        {
            get
            {
                const string TOTAL_ARGUMENT_NAME = "totalLevel";
                const string SCHEME_ARGUMENT_NAME = "perkScheme";

                // IPerkScheme which used to convert level
                // int testedTotalLevel - the level to convert level/sub in the perk scheme

                yield return new TestCaseData(null, 0, SCHEME_ARGUMENT_NAME)
                    .SetDescription("Fails because scheme is null");

                yield return new TestCaseData(CreateTestPerkScheme523(), 0, TOTAL_ARGUMENT_NAME)
                    .SetDescription("Fails because total is empty");

                yield return new TestCaseData(
                        new TestPerkScheme
                        { SystemDescription = "Empty levels", Levels = Array.Empty<PerkLevelSubScheme>() },
                        1,
                        SCHEME_ARGUMENT_NAME)
                    .SetDescription("Fails because scheme has no levels.");

                yield return new TestCaseData(
                        new TestPerkScheme
                        {
                            SystemDescription = "Subs is zero",
                            Levels = new[] { new PerkLevelSubScheme { MaxValue = 0 } }
                        },
                        1,
                        SCHEME_ARGUMENT_NAME)
                    .SetDescription("Fails because subs sum is zero.");

                yield return new TestCaseData(
                        new TestPerkScheme
                        {
                            SystemDescription = "Subs is negative",
                            Levels = new[] { new PerkLevelSubScheme { MaxValue = -1 } }
                        },
                        1,
                        SCHEME_ARGUMENT_NAME)
                    .SetDescription("Fails because one of the subs is negative.");

                yield return new TestCaseData(
                        new TestPerkScheme
                        {
                            SystemDescription = "Subs is 1", Levels = new[] { new PerkLevelSubScheme { MaxValue = 1 } }
                        },
                        2,
                        TOTAL_ARGUMENT_NAME)
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