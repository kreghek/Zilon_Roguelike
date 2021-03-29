using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

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
        /// The test checks the command executes if it is in the command pool.
        /// 1. Start the <see cref="CommandLoopUpdater" />. It can perform some idle iteration without command.
        /// 2. Push the command into the pool. And wait until ICommand.Execute() will be called.
        /// 3. Command processing loop pops the command from pool and executes it.
        /// </summary>
        [Test]
        [Timeout(5000)]
        public async Task StartAsync_CommandInPoolAfterStart_ExecutesCommand()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.Setup(x => x.WaitForUpdate(CancellationToken.None)).Returns(Task.CompletedTask);
            var context = contextMock.Object;

            var tcs = new TaskCompletionSource<bool>();

            var testTask = tcs.Task;

            var commandMock = new Mock<ICommand>();
            commandMock.Setup(x => x.Execute()).Callback(() => tcs.SetResult(true));
            var command = commandMock.Object;

            var commandPool = new TestCommandPool();

            var commandLoopUpdater = new CommandLoopUpdater(context, commandPool);

            // ACT

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            commandLoopUpdater.StartAsync(CancellationToken.None).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            commandPool.Push(command);

            // Delay to take some time to command loop updater to perform some iterations.
            await testTask.ConfigureAwait(false);

            // ASSERT

            commandMock.Verify(x => x.Execute(), Times.Once);
        }

        /// <summary>
        /// The test checks the command executes if it is in the command pool.
        /// 1. Push the command into the pool.
        /// 2. Start the <see cref="CommandLoopUpdater" />. It must takes the command from pool during first iteration.
        /// 3. Wait until Execute() called.
        /// </summary>
        [Test]
        [Timeout(5000)]
        public async Task StartAsync_CommandInPoolBeforeStart_ExecutesCommand()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.Setup(x => x.WaitForUpdate(CancellationToken.None)).Returns(Task.CompletedTask);
            var context = contextMock.Object;

            var tcs = new TaskCompletionSource<bool>();
            var testTask = tcs.Task;

            var commandMock = new Mock<ICommand>();
            commandMock.Setup(x => x.Execute()).Callback(() => tcs.SetResult(true));
            var command = commandMock.Object;

            var commandPool = new TestCommandPool();
            commandPool.Push(command);

            var commandLoopUpdater = new CommandLoopUpdater(context, commandPool);

            // ACT

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            commandLoopUpdater.StartAsync(CancellationToken.None).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await testTask.ConfigureAwait(false);

            // ASSERT

            commandMock.Verify(x => x.Execute(), Times.Once);
        }

        /// <summary>
        /// The test checks the event of the command processed was raised after command executed.
        /// </summary>
        [Test]
        [Timeout(5000)]
        public async Task StartAsync_CommandInPoolBeforeStart_RaisesCompleteCommandEvent()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.Setup(x => x.WaitForUpdate(CancellationToken.None)).Returns(Task.CompletedTask);
            var context = contextMock.Object;

            var tcs = new TaskCompletionSource<bool>();
            var testTask = tcs.Task;

            var commandMock = new Mock<ICommand>();
            var command = commandMock.Object;

            var commandPool = new TestCommandPool();
            commandPool.Push(command);

            var commandLoopUpdater = new CommandLoopUpdater(context, commandPool);
            commandLoopUpdater.CommandProcessed += (s, e) =>
            {
                tcs.SetResult(true);
            };

            // ACT
            using var monitor = commandLoopUpdater.Monitor();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            commandLoopUpdater.StartAsync(CancellationToken.None).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await testTask.ConfigureAwait(false);

            // ASSERT

            monitor.Should().Raise(nameof(commandLoopUpdater.CommandProcessed));
        }

        /// <summary>
        /// The test checks the command executes twice if it is repeatable and can repeat once.
        /// Note! Command must be abel to repeate and execute. Do not think that ability to repeat automaticaly means command can
        /// execute.
        /// </summary>
        [Test]
        [Timeout(5000)]
        public async Task StartAsync_CommandIsRepeatableOnce_ExecutesCommandTwice()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.Setup(x => x.WaitForUpdate(CancellationToken.None)).Returns(Task.CompletedTask);
            var context = contextMock.Object;

            var tcs = new TaskCompletionSource<bool>();
            var testTask = tcs.Task;

            var repeatIteration = 0;
            var commandMock = new Mock<IRepeatableCommand>();
            commandMock.Setup(x => x.Execute()).Callback(() =>
            {
                if (repeatIteration >= 1)
                {
                    tcs.SetResult(true);
                }
            });
            commandMock.Setup(x => x.CanRepeat()).Callback(() => { repeatIteration++; })
                .Returns(() => repeatIteration <= 1);
            commandMock.Setup(x => x.CanExecute()).Returns(true);

            var command = commandMock.Object;

            var commandPool = new TestCommandPool();
            commandPool.Push(command);

            var commandLoopUpdater = new CommandLoopUpdater(context, commandPool);

            // ACT

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            commandLoopUpdater.StartAsync(CancellationToken.None).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await testTask.ConfigureAwait(false);

            // ASSERT

            commandMock.Verify(x => x.Execute(), Times.Exactly(2));
        }

        /// <summary>
        /// The test checks the event of the command processed was raised then the command throws exception.
        /// </summary>
        [Test]
        [Timeout(5000)]
        public async Task StartAsync_CommandThrowsInvalidOperationException_RaisesErrorOccuredEvent()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.Setup(x => x.WaitForUpdate(CancellationToken.None)).Returns(Task.CompletedTask);
            var context = contextMock.Object;

            var tcs = new TaskCompletionSource<bool>();
            var testTask = tcs.Task;

            var commandMock = new Mock<ICommand>();
            commandMock.Setup(x => x.Execute()).Callback(() => { throw new InvalidOperationException(); });
            var command = commandMock.Object;

            var commandPool = new TestCommandPool();
            commandPool.Push(command);

            var commandLoopUpdater = new CommandLoopUpdater(context, commandPool);
            ErrorOccuredEventArgs raisedErrorArgs = null;
            commandLoopUpdater.ErrorOccured += (s, e) =>
            {
                raisedErrorArgs = e;
                tcs.SetResult(true);
            };

            // ACT
            using var monitor = commandLoopUpdater.Monitor();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            commandLoopUpdater.StartAsync(CancellationToken.None).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await testTask.ConfigureAwait(false);

            // ASSERT

            raisedErrorArgs.Should().NotBeNull();
            raisedErrorArgs.Should().BeOfType<CommandErrorOccuredEventArgs>();
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