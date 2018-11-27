using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class DefeatActorJobProgressTests
    {
        /// <summary>
        /// Тест проверяет, что прогресс на уничтожение противника применяется к задачам этого типа.
        /// </summary>
        [Test]
        public void ApplyToJobs_OneDefeatJob_DefeatJobProgressIncreased()
        {
            // ARRANGE
            const int startProgress = 1;
            const int expectedProgress = startProgress + 1;

            var actor = CreateActor();
            var job = CreateJob(startProgress, JobType.Defeats);
            var progress = CreateJobProgress(actor);




            // ACT
            progress.ApplyToJobs(new IJob[] { job });



            // ASSERT
            job.Progress.Should().Be(expectedProgress);
        }

        /// <summary>
        /// Тест проверяет, что прогресс на уничтожение противника применяется ТОЛЬКО к задачам этого типа.
        /// </summary>
        [Test]
        public void ApplyToJobs_DifferentJobTypes_DefeatJobProgressIncreased()
        {
            // ARRANGE
            const int startProgress = 1;
            const int expectedProgress = startProgress + 1;
            const int expectedOtherProgress = startProgress;

            var actor = CreateActor();

            var testedJob = CreateJob(startProgress, JobType.Defeats);
            var otherJob = CreateJob(startProgress, JobType.Hits);

            var progress = CreateJobProgress(actor);



            // ACT
            progress.ApplyToJobs(new IJob[] { testedJob, otherJob });



            // ASSERT
            testedJob.Progress.Should().Be(expectedProgress);
            otherJob.Progress.Should().Be(expectedOtherProgress);
        }


        private static IActor CreateActor()
        {
            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;
            return actor;
        }


        private static IJob CreateJob(int startProgress, JobType type)
        {
            var jobMock = new Mock<IJob>();
            jobMock.SetupProperty(x => x.Progress, startProgress);
            jobMock.SetupGet(x => x.Scheme).Returns(new TestJobSubScheme
            {
                Type = type,
                Value = 10
            });
            var job = jobMock.Object;
            return job;
        }

        private static DefeatActorJobProgress CreateJobProgress(IActor actor)
        {
            return new DefeatActorJobProgress(actor);
        }
    }
}