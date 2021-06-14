using System.Collections;

using NUnit.Framework;

namespace Zilon.Core.Tests.Tactics.Spatial.TestCases
{
    public static class EquipmentCarrierTestsCaseSource
    {
        public const string Sword = "sword";
        public const string Axe = "axe";
        public const string WoodenShield = "wooden-shield";
        public const string SteelShield = "steel-shield";
        public const string Colt = "colt";
        public const string Magnum = "magnum";

        public static IEnumerable TestCases
        {
            get
            {
                // см. матрицу из https://github.com/kreghek/Zilon_Roguelike/issues/45#issuecomment-447586671

                yield return new TestCaseData(null, null, false);
                yield return new TestCaseData(null, Sword, false);
                yield return new TestCaseData(null, Axe, false);
                yield return new TestCaseData(null, WoodenShield, false);
                yield return new TestCaseData(null, SteelShield, false);
                yield return new TestCaseData(null, Colt, false);
                yield return new TestCaseData(null, Magnum, false);

                yield return new TestCaseData(Sword, null, false);
                yield return new TestCaseData(Sword, Sword, false);
                yield return new TestCaseData(Sword, Axe, false);
                yield return new TestCaseData(Sword, WoodenShield, false);
                yield return new TestCaseData(Sword, SteelShield, false);
                yield return new TestCaseData(Sword, Colt, true);
                yield return new TestCaseData(Sword, Magnum, true);

                yield return new TestCaseData(Axe, null, false);
                yield return new TestCaseData(Axe, Sword, false);
                yield return new TestCaseData(Axe, Axe, false);
                yield return new TestCaseData(Axe, WoodenShield, false);
                yield return new TestCaseData(Axe, SteelShield, false);
                yield return new TestCaseData(Axe, Colt, true);
                yield return new TestCaseData(Axe, Magnum, true);

                yield return new TestCaseData(WoodenShield, null, false);
                yield return new TestCaseData(WoodenShield, Sword, false);
                yield return new TestCaseData(WoodenShield, Axe, false);
                yield return new TestCaseData(WoodenShield, WoodenShield, true);
                yield return new TestCaseData(WoodenShield, SteelShield, true);
                yield return new TestCaseData(WoodenShield, Colt, false);
                yield return new TestCaseData(WoodenShield, Magnum, false);

                yield return new TestCaseData(SteelShield, null, false);
                yield return new TestCaseData(SteelShield, Sword, false);
                yield return new TestCaseData(SteelShield, Axe, false);
                yield return new TestCaseData(SteelShield, WoodenShield, true);
                yield return new TestCaseData(SteelShield, SteelShield, true);
                yield return new TestCaseData(SteelShield, Colt, false);
                yield return new TestCaseData(SteelShield, Magnum, false);

                yield return new TestCaseData(Colt, null, false);
                yield return new TestCaseData(Colt, Sword, true);
                yield return new TestCaseData(Colt, Axe, true);
                yield return new TestCaseData(Colt, WoodenShield, false);
                yield return new TestCaseData(Colt, SteelShield, false);
                yield return new TestCaseData(Colt, Colt, true);
                yield return new TestCaseData(Colt, Magnum, true);

                yield return new TestCaseData(Magnum, null, false);
                yield return new TestCaseData(Magnum, Sword, true);
                yield return new TestCaseData(Magnum, Axe, true);
                yield return new TestCaseData(Magnum, WoodenShield, false);
                yield return new TestCaseData(Magnum, SteelShield, false);
                yield return new TestCaseData(Magnum, Colt, true);
                yield return new TestCaseData(Magnum, Magnum, true);
            }
        }
    }
}