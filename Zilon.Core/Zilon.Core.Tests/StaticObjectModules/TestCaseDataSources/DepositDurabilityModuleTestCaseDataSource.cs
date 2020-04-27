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
                // StockValue = 1;
                // DamagePerMineUnit = 10;
                // damages = new int[] { 6, 4 };
                yield return new TestCaseData(1, 10, new int[] { 6, 4 });
                yield return new TestCaseData(2, 10, new int[] { 6, 4, 5, 5 });
                yield return new TestCaseData(1, 1, new int[] { 2 });
                yield return new TestCaseData(1, 10, new int[] { 10 });
                yield return new TestCaseData(1, 10, new int[] { 6, 6 });
            }
        }
    }
}
