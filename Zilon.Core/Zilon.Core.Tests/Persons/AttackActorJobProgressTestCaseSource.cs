using System.Collections;

using NUnit.Framework;

using Zilon.Core.Common;

namespace Zilon.Core.Persons.Tests
{
    public static class AttackActorJobProgressTestCaseSource
    {
        public static IEnumerable PositiveTestCasesForWeaponUsage
        {
            get
            {
                // Params:
                // Tactical Act Equipment tags
                // Job weapon tags
                // Current job progress
                // returns expected progress

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Mace },
                    new[] { PropTags.Equipment.WeaponClass.Mace },
                    0).Returns(1);

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Axe },
                    new[] { PropTags.Equipment.WeaponClass.Axe },
                    0).Returns(1);

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Bow },
                    new[] { PropTags.Equipment.WeaponClass.Bow },
                    0).Returns(1);

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Pistol },
                    new[] { PropTags.Equipment.WeaponClass.Pistol },
                    0).Returns(1);

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Pole },
                    new[] { PropTags.Equipment.WeaponClass.Pole },
                    0).Returns(1);

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Staff },
                    new[] { PropTags.Equipment.WeaponClass.Staff },
                    0).Returns(1);

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Sword },
                    new[] { PropTags.Equipment.WeaponClass.Sword },
                    0).Returns(1);

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Wand },
                    new[] { PropTags.Equipment.WeaponClass.Wand },
                    0).Returns(1);

                // more complext scenarios

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Mace },
                    new[] { PropTags.Equipment.WeaponClass.Mace },
                    1).Returns(2);

                yield return new TestCaseData(
                    new[] { PropTags.Equipment.WeaponClass.Mace, PropTags.Equipment.WeaponClass.Pistol },
                    new[] { PropTags.Equipment.WeaponClass.Mace, PropTags.Equipment.WeaponClass.Pistol },
                    0).Returns(1);
            }
        }
    }
}