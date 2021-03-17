using System.Collections;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture]
    public class AttackActorJobProgressTests
    {
        /// <summary>
        /// The test checks jobs with JobType.AttacksActor and weapon tags increases progress then used equipment has all tags.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(AttackActorJobProgressTestCaseSource), nameof(AttackActorJobProgressTestCaseSource.PositiveTestCasesForWeaponUsage))]
        public int ApplyToJobs_PositiveJobsForWeaponUsage_ProgressIncreased(string[] equipmentTags, string[] jobWeaponTags, int currentJobProgress)
        {
            // ARRANGE

            var actor = Mock.Of<IActor>();

            var act = Mock.Of<ITacticalAct>(a => a.Equipment == new Equipment(new TestPropScheme
            {
                Equip = Mock.Of<IPropEquipSubScheme>(),
                Tags = equipmentTags
            }));

            var progress = new AttackActorJobProgress(actor, act);

            var jobs = new IJob[] {
                Mock.Of<IJob>(job => job.Scheme == Mock.Of<IJobSubScheme>(
                    scheme => scheme.Type == JobType.AttacksActor
                    && scheme.Data == new []
                    {
                        // anonymus object like AttackActorJobData
                        // because there is no interface to make test object with setter
                        JsonConvert.SerializeObject(new { WeaponTags = jobWeaponTags })
                    }
                ) &&
                job.Progress == currentJobProgress)
            };

            // ACT

            var factModifiedJobs = progress.ApplyToJobs(jobs);

            // ASSERT
            return factModifiedJobs[0].Progress;
        }
    }

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

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Shield },
                new[] { PropTags.Equipment.WeaponClass.Shield },
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

                yield return new TestCaseData(new[] { PropTags.Equipment.WeaponClass.Mace, PropTags.Equipment.WeaponClass.Pistol },
                new[] { PropTags.Equipment.WeaponClass.Mace, PropTags.Equipment.WeaponClass.Pistol },
                0).Returns(1);
            }
        }
    }
}