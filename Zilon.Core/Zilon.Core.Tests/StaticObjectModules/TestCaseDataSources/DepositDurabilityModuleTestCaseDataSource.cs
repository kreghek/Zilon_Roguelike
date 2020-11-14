using System.Collections;

using NUnit.Framework;

namespace Zilon.Core.Tests.StaticObjectModules.TestCaseDataSources
{
    public static class DepositDurabilityModuleTestCaseDataSource
    {
        public static IEnumerable DestroyTestCases
        {
            get
            {
                // StockValue, for example 1.
                // DamagePerMineUnit, for example 10.
                // damages, for example new [] { 6, 4 }.
                yield return new TestCaseData(1, 10, new[] { 6, 4 });
                yield return new TestCaseData(2, 10, new[] { 6, 4, 5, 5 });
                yield return new TestCaseData(1, 1, new[] { 2 });
                yield return new TestCaseData(1, 10, new[] { 10 });
                yield return new TestCaseData(1, 10, new[] { 6, 6 });
            }
        }
    }
}