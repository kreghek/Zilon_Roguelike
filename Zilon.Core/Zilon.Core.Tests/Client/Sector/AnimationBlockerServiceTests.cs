using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

namespace Zilon.Core.Client.Sector.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [Timeout(5000)]
    public class AnimationBlockerServiceTests
    {
        [Test]
        public async Task WaitBlockersAsync_AddAndRelease2BlockersBeforeWaiting_NoWaiting()
        {
            // ARRANGE
            var animationBlockerService = new AnimationBlockerService();

            var blockerMock1 = new Mock<ICommandBlocker>();
            var blocker1 = blockerMock1.Object;

            var blockerMock2 = new Mock<ICommandBlocker>();
            var blocker2 = blockerMock1.Object;

            animationBlockerService.AddBlocker(blocker1);
            animationBlockerService.AddBlocker(blocker2);

            blockerMock1.Raise(x => x.Released += null, EventArgs.Empty);
            blockerMock2.Raise(x => x.Released += null, EventArgs.Empty);

            // ACT
            await animationBlockerService.WaitBlockersAsync().ConfigureAwait(false);

            // ASSERT
            Assert.Pass();
        }

        [Test]
        public async Task WaitBlockersAsync_AddAndReleaseBlockerBeforeWaiting_NoWaiting()
        {
            // ARRANGE
            var animationBlockerService = new AnimationBlockerService();

            var blockerMock = new Mock<ICommandBlocker>();
            var blocker = blockerMock.Object;

            animationBlockerService.AddBlocker(blocker);

            blockerMock.Raise(x => x.Released += null, EventArgs.Empty);

            // ACT
            await animationBlockerService.WaitBlockersAsync().ConfigureAwait(false);

            // ASSERT
            Assert.Pass();
        }

        [Test]
        public async Task WaitBlockersAsynс_AddAndDropBlocker2Times_WaitingContinuesExecution()
        {
            // ARRANGE
            var animationBlockerService = new AnimationBlockerService();

            var blockerMock = new Mock<ICommandBlocker>();
            var blocker = blockerMock.Object;

            var blockerMock2 = new Mock<ICommandBlocker>();
            var blocker2 = blockerMock2.Object;

            animationBlockerService.AddBlocker(blocker);

            using var semaphore = new SemaphoreSlim(0);

            await AddAndDropBlockerAsync(animationBlockerService, semaphore).ConfigureAwait(false);

            animationBlockerService.AddBlocker(blocker2);

            await AddAndDropBlockerAsync(animationBlockerService, semaphore).ConfigureAwait(false);

            // ASSERT
            Assert.Pass();
        }

        [Test]
        public async Task WaitBlockersAsynс_AddAndReleaseBlocker2Times_WaitingContinuesExecution()
        {
            // ARRANGE
            var animationBlockerService = new AnimationBlockerService();

            var blockerMock = new Mock<ICommandBlocker>();
            var blocker = blockerMock.Object;

            var blockerMock2 = new Mock<ICommandBlocker>();
            var blocker2 = blockerMock2.Object;

            animationBlockerService.AddBlocker(blocker);

            await WaitAndReleaseBlockerAsync(animationBlockerService, blockerMock).ConfigureAwait(false);

            animationBlockerService.AddBlocker(blocker2);
            await WaitAndReleaseBlockerAsync(animationBlockerService, blockerMock2).ConfigureAwait(false);

            // ASSERT
            Assert.Pass();
        }

        /// <summary>
        /// The test checks WaitBlockersAsync continued execution when blocker are released.
        /// The plan is next:
        /// 1. Add a blocker.
        /// 2. Run background task waiting some time.
        /// 3. In mean time, the AnimationBlockerService waits the blocker.
        /// Use semaphore to ensure the AnimationBlockerService starts to wait first. The blocker will released only after
        /// WaitBlockersAsync called.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task WaitBlockersAsynс_BlockerReleasedAfterServiceStartsWaiting_WaitingContinuesExecution()
        {
            // ARRANGE
            var animationBlockerService = new AnimationBlockerService();

            var blockerMock = new Mock<ICommandBlocker>();
            var blocker = blockerMock.Object;

            var isBlockerReallyReleased = false;
            blocker.Released += (s, e) =>
            {
                isBlockerReallyReleased = true;
            };

            animationBlockerService.AddBlocker(blocker);

            using var semaphore = new SemaphoreSlim(0);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                await semaphore.WaitAsync().ConfigureAwait(false);
                blockerMock.Raise(x => x.Released += null, EventArgs.Empty);
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // ACT
            var serviceTask = Task.Run(async () =>
            {
                await animationBlockerService.WaitBlockersAsync().ConfigureAwait(false);
            });

            isBlockerReallyReleased.Should().BeFalse();

            semaphore.Release();

            await serviceTask.ConfigureAwait(false);

            // ASSERT
            isBlockerReallyReleased.Should().BeTrue();
        }

        /// <summary>
        /// The test checks DropBlockers() releases all blockers.
        /// The plan is similar
        /// <see cref="WaitBlockersAsynс_BlockerReleasedAfterServiceStartsWaiting_WaitingContinuesExecution" />.
        /// </summary>
        [Test]
        public async Task WaitBlockersAsynс_DropBlockerCancelAwaitingOfBlockerRelease_WaitingContinuesExecution()
        {
            // ARRANGE
            var animationBlockerService = new AnimationBlockerService();

            var blockerMock = new Mock<ICommandBlocker>();
            var blocker = blockerMock.Object;

            animationBlockerService.AddBlocker(blocker);

            using var semaphore = new SemaphoreSlim(0);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                await semaphore.WaitAsync().ConfigureAwait(false);
                animationBlockerService.DropBlockers();
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // ACT
            var serviceTask = Task.Run(async () =>
            {
                await animationBlockerService.WaitBlockersAsync().ConfigureAwait(false);
            });

            semaphore.Release();

            await serviceTask.ConfigureAwait(false);

            // ASSERT
            Assert.Pass();
        }

        [Test]
        public async Task WaitBlockersAsynс_Real1()
        {
            var animationBlockerService = new AnimationBlockerService();

            var blockerMock = new Mock<ICommandBlocker>();
            var blocker = blockerMock.Object;

            animationBlockerService.AddBlocker(blocker);

            await WaitAndReleaseBlockerAsync(animationBlockerService, blockerMock).ConfigureAwait(false);

            var blockerMock2 = new Mock<ICommandBlocker>();
            var blocker2 = blockerMock2.Object;

            var blockerMock3 = new Mock<ICommandBlocker>();
            var blocker3 = blockerMock3.Object;

            animationBlockerService.AddBlocker(blocker2);
            animationBlockerService.AddBlocker(blocker3);

            var isBlockerReleasedCheck = false;
            using var semaphore = new SemaphoreSlim(0);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                await semaphore.WaitAsync().ConfigureAwait(false);
                isBlockerReleasedCheck = true;
                blockerMock2.Raise(x => x.Released += null, EventArgs.Empty);
                blockerMock3.Raise(x => x.Released += null, EventArgs.Empty);
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            var serviceTask = Task.Run(async () =>
            {
                await animationBlockerService.WaitBlockersAsync().ConfigureAwait(false);
            });

            isBlockerReleasedCheck.Should().BeFalse();

            semaphore.Release();

            await serviceTask.ConfigureAwait(false);

            // ASSERT
            isBlockerReleasedCheck.Should().BeTrue();
            Assert.Pass();
        }

        [Test]
        public async Task WaitBlockersAsynс_SecondBlockerWasAddedUntilFirstWasReleased_WaitingContinuesExecution()
        {
            // ARRANGE
            var animationBlockerService = new AnimationBlockerService();

            var blockerMock1 = new Mock<ICommandBlocker>();
            var blocker1 = blockerMock1.Object;

            var blockerMock2 = new Mock<ICommandBlocker>();
            var blocker2 = blockerMock2.Object;

            animationBlockerService.AddBlocker(blocker1);

            using var semaphore = new SemaphoreSlim(0);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                await semaphore.WaitAsync().ConfigureAwait(false);

                animationBlockerService.AddBlocker(blocker2);

                blockerMock1.Raise(x => x.Released += null, EventArgs.Empty);

                await Task.Delay(100).ConfigureAwait(false);
                blockerMock2.Raise(x => x.Released += null, EventArgs.Empty);
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // ACT
            var serviceTask = Task.Run(async () =>
            {
                await animationBlockerService.WaitBlockersAsync().ConfigureAwait(false);
            });

            semaphore.Release();

            await serviceTask.ConfigureAwait(false);

            // ASSERT
            Assert.Pass();
        }

        private static async Task AddAndDropBlockerAsync(AnimationBlockerService animationBlockerService,
            SemaphoreSlim semaphore)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                await semaphore.WaitAsync().ConfigureAwait(false);
                animationBlockerService.DropBlockers();
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // ACT
            var serviceTask = Task.Run(async () =>
            {
                await animationBlockerService.WaitBlockersAsync().ConfigureAwait(false);
            });

            semaphore.Release();

            await serviceTask.ConfigureAwait(false);
        }

        private static async Task WaitAndReleaseBlockerAsync(AnimationBlockerService animationBlockerService,
            Mock<ICommandBlocker> blockerMock)
        {
            using var semaphore = new SemaphoreSlim(0, 1);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                await semaphore.WaitAsync().ConfigureAwait(false);
                blockerMock.Raise(x => x.Released += null, EventArgs.Empty);
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            var serviceTask = Task.Run(async () =>
            {
                await animationBlockerService.WaitBlockersAsync().ConfigureAwait(false);
            });

            semaphore.Release();

            await serviceTask.ConfigureAwait(false);
        }
    }
}