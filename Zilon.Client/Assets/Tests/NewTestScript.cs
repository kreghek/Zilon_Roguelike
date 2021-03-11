using System.Collections;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

using NUnit.Framework;

using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void GameLoopUpdater_Stop_GameLoopContextIsNotUpdatedAfterUpdaterStoped2()
        {
            // ARRANGE

            var context = new TestGameLoopContext();

            var animationBlocker = new TestAnimationBlocker();

            var gameLoopUpdater = new GameLoopUpdater(context, animationBlocker);

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

            public async Task UpdateAsync()
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

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator GameLoopUpdater_Stop_GameLoopContextIsNotUpdatedAfterUpdaterStoped()
        {
            // Use the Assert class to test conditions

            yield return null;
        }
    }

    public static class TaskExtensions
    {
        public static IEnumerator AsIEnumeratorReturnNull<T>(this Task<T> task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }

            yield return null;
        }
    }
}
