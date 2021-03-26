using System;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Zilon.Core.Commands;

namespace Zilon.Core.Client.Sector.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class CommandLoopUpdaterTests
    {
        /// <summary>
        /// The test checks the command executes if it in the command pool.
        /// 1. Start the <see cref="CommandLoopUpdater" />.
        /// 2. Push the command into the pool. And wait some times.
        /// 3. Command processing loop pops command from pool and executes it.        ///
        /// </summary>
        [Test]
        [Timeout(1000)]
        public async Task StartAsync_CommandInPool_ExecutesCommand()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.Setup(x => x.WaitForUpdate(CancellationToken.None)).Returns(Task.CompletedTask);
            var context = contextMock.Object;

            var commandMock = new Mock<ICommand>();
            var command = commandMock.Object;

            var commandPool = new TestCommandPool();

            var commandLoopUpdater = new CommandLoopUpdater(context, commandPool);

            // ACT

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            commandLoopUpdater.StartAsync(CancellationToken.None).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            commandPool.Push(command);

            // Delay to take some time to command loop updater to perform some iterations.
            await Task.Delay(100);

            // ASSERT

            commandMock.Verify(x => x.Execute(), Times.Once);
        }

        private sealed class TestCommandPool : ICommandPool
        {
            private readonly object _lockObject;

            private ICommand _storedCommand;

            public TestCommandPool()
            {
                _lockObject = new object();
            }

            public ICommand Pop()
            {
                lock (_lockObject)
                {
                    var commandToPop = _storedCommand;
                    _storedCommand = null;
                    return commandToPop;
                }
            }

            public void Push(ICommand command)
            {
                lock (_lockObject)
                {
                    _storedCommand = command;
                }
            }

            public event EventHandler CommandPushed;
        }
    }
}