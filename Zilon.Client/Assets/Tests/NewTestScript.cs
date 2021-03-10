using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions

            // ARRANGE

            var context = new TestGameLoopContext();

            var animationBlocker = new TestAnimationBlocker();

            var gameLoopUpdater = new GameLoopUpdater(context, animationBlocker);

            // ACT

            gameLoopUpdater.Start();
        }

        sealed class TestGameLoopContext : IGameLoopContext
        {
            public bool HasNextIteration => true;

            public async Task UpdateAsync()
            {
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
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
