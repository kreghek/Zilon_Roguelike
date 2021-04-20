using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

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
        [TestCaseSource(typeof(AttackActorJobProgressTestCaseSource),
            nameof(AttackActorJobProgressTestCaseSource.PositiveTestCasesForWeaponUsage))]
        public int ApplyToJobs_PositiveJobsForWeaponUsage_ProgressIncreased(string[] equipmentTags,
            string[] jobWeaponTags, int currentJobProgress)
        {
            // ARRANGE

            var actor = Mock.Of<IActor>();

            var act = Mock.Of<ITacticalAct>(a => a.Equipment == new Equipment(new TestPropScheme
            {
                Equip = Mock.Of<IPropEquipSubScheme>(),
                Tags = equipmentTags
            }));

            var progress = new AttackActorJobProgress(actor, act);

            var jobs = new IJob[]
            {
                Mock.Of<IJob>(job => job.Scheme == Mock.Of<IJobSubScheme>(
                                         scheme => scheme.Type == JobType.AttacksActor
                                                   && scheme.Data == new[]
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
}