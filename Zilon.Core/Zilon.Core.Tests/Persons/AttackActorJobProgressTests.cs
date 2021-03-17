using NUnit.Framework;
using Zilon.Core.Persons;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Zilon.Core.Tactics;
using FluentAssertions;
using Zilon.Core.Schemes;
using Newtonsoft.Json;
using Zilon.Core.Props;
using Zilon.Core.Common;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture()]
    public class AttackActorJobProgressTests
    {
        [Test()]
        public void ApplyToJobsTest()
        {
            // ARRANGE

            var actor = Mock.Of<IActor>();

            var act = Mock.Of<ITacticalAct>(a => a.Equipment == new Equipment(new TestPropScheme {
                Equip = Mock.Of<IPropEquipSubScheme>(),
                Tags = new[] { PropTags.Equipment.WeaponClass.Mace }
            }));

            var progress = new AttackActorJobProgress(actor, act);

            var jobs = new IJob[] {
                Mock.Of<IJob>(job => job.Scheme == Mock.Of<IJobSubScheme>(
                    scheme => scheme.Type == JobType.AttacksActor && scheme.Data == new []{
                        JsonConvert.SerializeObject(new {
                            WeaponTags = new[]{ PropTags.Equipment.WeaponClass.Mace }
                        })
                        }
                    ))
            };

            // ACT

            var factModifiedJobs = progress.ApplyToJobs(jobs);

            // ASSERT
            factModifiedJobs[0].Progress.Should().Be(1);
        }
    }
}