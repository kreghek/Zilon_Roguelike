using System;
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
        public async Task WaitBlockersAsyncTestAsync()
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
            animationBlockerService.HasBlockers.Should().BeFalse();
        }
    }
}