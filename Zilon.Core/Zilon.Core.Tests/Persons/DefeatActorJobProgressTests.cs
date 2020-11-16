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
    [Parallelizable(ParallelScope.All)]
    public class DefeatActorJobProgressTests
    {
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
            progress.ApplyToJobs(new[]
            {
                testedJob,
                otherJob
            });

            // ASSERT
            testedJob.Progress.Should().Be(expectedProgress);
            otherJob.Progress.Should().Be(expectedOtherProgress);
        }

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
            progress.ApplyToJobs(new[]
            {
                job
            });

            // ASSERT
            job.Progress.Should().Be(expectedProgress);
        }

        /// <summary>
        /// Тест проверяет, что применение прогресса возвращает работы, которые были изменены.
        /// </summary>
        [Test]
        public void ApplyToJobs_OneDefeatJob_ReturnsChangedJobs()
        {
            // ARRANGE
            const int startProgress = 1;

            var actor = CreateActor();
            var job = CreateJob(startProgress, JobType.Defeats);
            var progress = CreateJobProgress(actor);

            // ACT
            var changedJobs = progress.ApplyToJobs(new[]
            {
                job
            });

            // ASSERT
            changedJobs.Should().HaveCount(1);
            changedJobs[0].Should().Be(job);
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

        private static IJobProgress CreateJobProgress(IActor actor)
        {
            return new DefeatActorJobProgress(actor);
        }
    }
}