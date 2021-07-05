#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Commands;

namespace Zilon.Core.Client.Sector.Tests
{
    /// <remarks>
    /// - If tests are parallel they fail in containers of CI/CD randomly. So we set NonParallelizableAttribute implicitly.
    /// May be this situation occured because CI/CD has few CPU resources.
    /// </remarks>
    [TestFixture]
    [Timeout(5000)]
    public class CommandLoopUpdaterTests
    {
        /// <summary>
        /// The test checks the command executes if it is in the command pool.
        /// 1. Start the <see cref="CommandLoopUpdater" />. It can perform some idle iteration without command.
        /// 2. Push the command into the pool. And wait until ICommand.Execute() will be called.
        /// 3. Command processing loop pops the command from pool and executes it.
        /// </summary>
        [Test]
        public async Task StartAsync_CommandInPoolAfterStart_ExecutesCommand()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.SetupGet(x => x.CanPlayerGiveCommand).Returns(true);
            var context = contextMock.Object;

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

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
        public async Task StartAsync_CommandInPoolBeforeStart_ExecutesCommand()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.SetupGet(x => x.CanPlayerGiveCommand).Returns(true);
            var context = contextMock.Object;

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var waitCommandExecutedTask = tcs.Task;

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

            await waitCommandExecutedTask.ConfigureAwait(false);

            // ASSERT

            commandMock.Verify(x => x.Execute(), Times.Once);
        }

        /// <summary>
        /// The test checks the event of the command processed was raised after command executed.
        /// </summary>
        [Test]
        public async Task StartAsync_CommandInPoolBeforeStart_RaisesCompleteCommandEvent()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.SetupGet(x => x.CanPlayerGiveCommand).Returns(true);
            var context = contextMock.Object;

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var eventTask = tcs.Task;

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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            commandLoopUpdater.StartAsync(CancellationToken.None).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // ASSERT

            var expectedEventWasRaised = await eventTask.ConfigureAwait(false);

            expectedEventWasRaised.Should().BeTrue();
        }

        /// <summary>
        /// The test checks the command executes twice if it is repeatable and can repeat once.
        /// Note! Command must be abel to repeate and execute. Do not think that ability to repeat automaticaly means command can
        /// execute.
        /// </summary>
        [Test]
        public async Task StartAsync_CommandIsRepeatableOnce_ExecutesCommandTwice()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.SetupGet(x => x.CanPlayerGiveCommand).Returns(true);
            var context = contextMock.Object;

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var waitCommandExecutedTask = tcs.Task;

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
            commandMock.Setup(x => x.CanExecute()).Returns(new CanExecuteCheckResult { IsSuccess = true });

            var command = commandMock.Object;

            var commandPool = new TestCommandPool();
            commandPool.Push(command);

            var commandLoopUpdater = new CommandLoopUpdater(context, commandPool);

            // ACT

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            commandLoopUpdater.StartAsync(CancellationToken.None).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await waitCommandExecutedTask.ConfigureAwait(false);

            // ASSERT

            commandMock.Verify(x => x.Execute(), Times.Exactly(2));
        }

        /// <summary>
        /// The test checks the event of the command processed was raised then the command throws exception.
        /// </summary>
        [Test]
        public async Task StartAsync_CommandThrowsInvalidOperationException_RaisesErrorOccuredEvent()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.SetupGet(x => x.CanPlayerGiveCommand).Returns(true);
            var context = contextMock.Object;

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var eventWasInvokedTask = tcs.Task;

            var commandMock = new Mock<ICommand>();
            commandMock.Setup(x => x.Execute()).Throws<InvalidOperationException>();
            var command = commandMock.Object;

            var commandPool = new TestCommandPool();
            commandPool.Push(command);

            var commandLoopUpdater = new CommandLoopUpdater(context, commandPool);
            ErrorOccuredEventArgs? expectedRaisedErrorArgs = null;
            var raiseCount = 0;
            commandLoopUpdater.ErrorOccured += (s, e) =>
            {
                expectedRaisedErrorArgs = e;

                // Count raises to prevent errors with multiple events.
                raiseCount++;

                tcs.SetResult(true);
            };

            // ACT
            using var monitor = commandLoopUpdater.Monitor();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            commandLoopUpdater.StartAsync(CancellationToken.None).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            var expectedEventWasRaised = await eventWasInvokedTask.ConfigureAwait(false);

            // ASSERT

            expectedEventWasRaised.Should().BeTrue();
            expectedRaisedErrorArgs.Should().BeOfType<CommandErrorOccuredEventArgs>();
            raiseCount.Should().Be(1);
        }

        [Test]
        public async Task StopAsync_StopAfterStart_CommandsAreNotRequestedAfterStop()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.SetupGet(x => x.CanPlayerGiveCommand).Returns(true);
            var context = contextMock.Object;

            var command = Mock.Of<ICommand>();

            var commandPoolMock = new Mock<ICommandPool>();
            var semaphore = new SemaphoreSlim(0, 1);
            commandPoolMock.Setup(x => x.Pop()).Returns(() => 
            {
                Task.Delay(100).Wait();
                semaphore.Release();
                return command;
            });
            var commandPool = commandPoolMock.Object;

            var commandLoop = new CommandLoopUpdater(context, commandPool);

            // ACT
            commandLoop.StartAsync(CancellationToken.None);

            await semaphore.WaitAsync().ConfigureAwait(false);

            await commandLoop.StopAsync().ConfigureAwait(false);

            // ASSERT
            commandPoolMock.Verify(x => x.Pop(), Times.AtMost(2));
        }

        private sealed class TestCommandPool : ICommandPool
        {
            private readonly SemaphoreSlim _semaphore;

            private ICommand? _storedCommand;

            public TestCommandPool()
            {
                _semaphore = new SemaphoreSlim(1, 1);
            }

            public bool IsEmpty { get; }

            public ICommand? Pop()
            {
                _semaphore.Wait();

                try
                {
                    var commandToPop = _storedCommand;
                    _storedCommand = null;
                    return commandToPop;
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            public void Push(ICommand command)
            {
                _semaphore.Wait();

                try
                {
                    _storedCommand = command;
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            public event EventHandler? CommandPushed;
        }
    }
}

#nullable disable