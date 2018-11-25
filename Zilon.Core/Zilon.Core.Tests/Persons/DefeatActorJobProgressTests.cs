using FluentAssertions;

using Moq;

using NUnit.Framework;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture]
    public class DefeatActorJobProgressTests
    {
        [Test]
        public void ApplyToJobsTest()
        {
            // ARRANGE
            const int startProgress = 1;
            const int expectedProgress = startProgress + 1;

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var jobMock = new Mock<IJob>();
            jobMock.SetupProperty(x => x.Progress, startProgress);
            jobMock.SetupGet(x=>x.Scheme).Returns(new JobSubScheme {

            })
            var job = jobMock.Object;

            var progress = new DefeatActorJobProgress(actor);




            // ACT
            progress.ApplyToJobs(new IJob[] { job });



            // ASSERT
            job.Progress.Should().Be(expectedProgress);
        }
    }
}