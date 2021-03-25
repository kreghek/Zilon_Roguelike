using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using Zilon.Core.Client.Sector;

namespace Tests
{
    public class GlobeLoopUpdaterTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void Stop_GameLoopContextIsNotUpdatedAfterUpdaterStoped()
        {
            // ARRANGE

            var context = new TestGameLoopContext();

            var animationBlocker = new TestAnimationBlocker();

            var gameLoopUpdater = new GlobeLoopUpdater(context, animationBlocker);

            // ACT

            gameLoopUpdater.Start();

            // await analog
            Task.Delay(100).GetAwaiter().GetResult();

            gameLoopUpdater.Stop();

            Assert.IsTrue(context.IsUpdated);

            context.IsUpdated = false;

            Task.Delay(100).GetAwaiter().GetResult();

            Assert.IsFalse(context.IsUpdated);
        }

        sealed class TestGameLoopContext : IGameLoopContext
        {
            public bool IsUpdated { get; set; }

            public bool HasNextIteration => true;

            public async Task UpdateAsync(CancellationToken cancellationToken)
            {
                IsUpdated = true;
                await Task.Delay(1);
            }
        }

        sealed class TestAnimationBlocker : IAnimationBlockerService
        {
            public bool HasBlockers { get; }

            public void AddBlocker(ICommandBlocker commandBlocker)
            {
                throw new System.NotImplementedException();
            }

            public void DropBlockers()
            {
                throw new System.NotImplementedException();
            }

            public Task WaitBlockersAsync()
            {
                return Task.CompletedTask;
            }
        }
    }
}
