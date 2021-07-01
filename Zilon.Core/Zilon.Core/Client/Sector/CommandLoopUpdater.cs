using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.Commands;

namespace Zilon.Core.Client.Sector
{
    public sealed class CommandLoopUpdater : ICommandLoopUpdater, IDisposable
    {
        private const int WAIT_FOR_CHANGES_MILLISECONDS = 100;
        private readonly ICommandLoopContext _commandLoopContext;
        private readonly ICommandPool _commandPool;

        private readonly SemaphoreSlim _semaphoreSlim;

        private bool _hasPendingCommand;

        [ExcludeFromCodeCoverage]
        public CommandLoopUpdater(ICommandLoopContext commandLoopContext, ICommandPool commandPool)
        {
            _commandLoopContext = commandLoopContext;
            _commandPool = commandPool;

            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        private void DoErrorOccured(ICommand? commandWithError, InvalidOperationException exception)
        {
            if (commandWithError != null)
            {
                ErrorOccured?.Invoke(this, new CommandErrorOccuredEventArgs(commandWithError, exception));
            }
            else
            {
                ErrorOccured?.Invoke(this, new ErrorOccuredEventArgs(exception));
            }
        }

        private async Task HandleRepeatableAsync(ICommand? command)
        {
            if (command is IRepeatableCommand repeatableCommand)
            {
                // It is necesary because CanRepeate and CanExecute can perform early that globe updates its state.
                await WaitGlobeIterationPerformedAsync().ConfigureAwait(false);

                if (repeatableCommand.CanRepeat() && repeatableCommand.CanExecute().IsSuccess)
                {
                    repeatableCommand.IncreaceIteration();
                    _commandPool.Push(repeatableCommand);
                    CommandAutoExecuted?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    await SetHasPendingCommandsAsync(false).ConfigureAwait(false);

                    CommandProcessed?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                await SetHasPendingCommandsAsync(false).ConfigureAwait(false);

                CommandProcessed?.Invoke(this, EventArgs.Empty);
            }
        }

        private async Task SetHasPendingCommandsAsync(bool value)
        {
            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                _hasPendingCommand = value;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<ICommand?> TryExecuteCommandAsync(ICommand? lastCommand)
        {
            ICommand? commandWithError = null;
            ICommand? newLastCommand = null;

            var errorOccured = false;

            var command = _commandPool.Pop();

            try
            {
                if (command != null)
                {
                    await SetHasPendingCommandsAsync(true).ConfigureAwait(false);

                    try
                    {
                        command.Execute();
                    }
                    catch (InvalidOperationException)
                    {
                        commandWithError = command;
                        throw;
                    }

                    await HandleRepeatableAsync(command).ConfigureAwait(false);

                    newLastCommand = command;
                }
                else
                {
                    await SetHasPendingCommandsAsync(false).ConfigureAwait(false);

                    if (lastCommand != null)
                    {
                        CommandProcessed?.Invoke(this, EventArgs.Empty);
                        newLastCommand = null;
                    }
                }
            }
            catch (InvalidOperationException exception)
            {
                errorOccured = true;
                DoErrorOccured(commandWithError, exception);

                newLastCommand = null;
            }
            finally
            {
                if (errorOccured)
                {
                    await SetHasPendingCommandsAsync(false).ConfigureAwait(false);

                    CommandProcessed?.Invoke(this, EventArgs.Empty);
                    newLastCommand = null;
                }
            }

            return newLastCommand;
        }

        private async Task WaitGlobeIterationPerformedAsync()
        {
            var fuseCounter = 100;
            while (fuseCounter > 0)
            {
                await Task.Yield();
                await Task.Delay(WAIT_FOR_CHANGES_MILLISECONDS).ConfigureAwait(false);

                if (_commandLoopContext.CanPlayerGiveCommand)
                {
                    break;
                }

                fuseCounter--;
            }
        }

        public event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;
        public event EventHandler? CommandAutoExecuted;
        public event EventHandler? CommandProcessed;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokebSourceInner = new CancellationTokenSource();
            var internalToken = _cancellationTokebSourceInner.Token;

            _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(internalToken, cancellationToken);

            IsStarted = true;
            return Task.Run(async () =>
            {
                ICommand? lastCommand = null;

                while (_commandLoopContext.HasNextIteration)
                {
                    if (!_commandLoopContext.CanPlayerGiveCommand)
                    {
                        // If player can't gives command right now the loop sleep some time (100ms).
                        // Because this can wait a little to start new attempt of command execution.
                        await Task.Yield();
                        await Task.Delay(WAIT_FOR_CHANGES_MILLISECONDS).ConfigureAwait(false);
                        continue;
                    }

                    try
                    {
                        lastCommand = await TryExecuteCommandAsync(lastCommand: lastCommand).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        ErrorOccured?.Invoke(this, new ErrorOccuredEventArgs(exception));
                    }

                    await Task.Yield();
                    await Task.Delay(WAIT_FOR_CHANGES_MILLISECONDS).ConfigureAwait(false);
                }
            }, _linkedCts.Token);
        }

        private CancellationTokenSource _linkedCts;
        private CancellationTokenSource _cancellationTokebSourceInner;

        public bool IsStarted { get; private set; }

        public bool HasPendingCommands()
        {
            _semaphoreSlim.WaitAsync().Wait();
            try
            {
                return _hasPendingCommand;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public Task StopAsync()
        {
            if (_cancellationTokebSourceInner is not null)
            {
                _cancellationTokebSourceInner.Cancel();
                _cancellationTokebSourceInner.Dispose();
            }

            if (_linkedCts is not null)
            {
                _linkedCts.Dispose();
            }

            if (_semaphoreSlim.CurrentCount == 0)
            {
                _semaphoreSlim.Release();
            }

            IsStarted = false;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_cancellationTokebSourceInner is not null)
            {
                _cancellationTokebSourceInner.Dispose();
            }

            if (_linkedCts is not null)
            {
                _linkedCts.Dispose();
            }
        }
    }
}