using System;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.Commands;

namespace Zilon.Core.Client.Sector
{
    public sealed class CommandLoopUpdater : ICommandLoopUpdater
    {
        private readonly ICommandLoopContext _commandLoopContext;
        private readonly ICommandPool _commandPool;

        private readonly SemaphoreSlim _semaphoreSlim;

        private bool _hasPendingCommand;

        public CommandLoopUpdater(ICommandLoopContext commandLoopContext, ICommandPool commandPool)
        {
            _commandLoopContext = commandLoopContext;
            _commandPool = commandPool;

            _semaphoreSlim = new SemaphoreSlim(1, 1);
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
                    await _semaphoreSlim.WaitAsync();
                    try
                    {
                        _hasPendingCommand = true;
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }

                    try
                    {
                        command.Execute();
                    }
                    catch (InvalidOperationException)
                    {
                        commandWithError = command;
                        throw;
                    }

                    if (command is IRepeatableCommand repeatableCommand)
                    {
                        // It is necesary because CanRepeate and CanExecute can perform early that globe updates its state.
                        await WaitGlobeIterationPerformedAsync();

                        if (repeatableCommand.CanRepeat() && repeatableCommand.CanExecute().IsSuccess)
                        {
                            repeatableCommand.IncreaceIteration();
                            _commandPool.Push(repeatableCommand);
                            CommandAutoExecuted?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            await _semaphoreSlim.WaitAsync();
                            try
                            {
                                _hasPendingCommand = false;
                            }
                            finally
                            {
                                _semaphoreSlim.Release();
                            }

                            CommandProcessed?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        await _semaphoreSlim.WaitAsync();
                        try
                        {
                            _hasPendingCommand = false;
                        }
                        finally
                        {
                            _semaphoreSlim.Release();
                        }

                        CommandProcessed?.Invoke(this, EventArgs.Empty);
                    }

                    newLastCommand = command;
                }
                else
                {
                    await _semaphoreSlim.WaitAsync();
                    try
                    {
                        _hasPendingCommand = false;
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }

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

                if (commandWithError != null)
                {
                    ErrorOccured?.Invoke(this, new CommandErrorOccuredEventArgs(commandWithError, exception));
                }
                else
                {
                    ErrorOccured?.Invoke(this, new ErrorOccuredEventArgs(exception));
                }

                newLastCommand = null;
            }
            finally
            {
                if (errorOccured)
                {
                    await _semaphoreSlim.WaitAsync();
                    try
                    {
                        _hasPendingCommand = false;
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }

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
                await Task.Delay(100).ConfigureAwait(false);

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
            IsStarted = true;
            return Task.Run(async () =>
            {
                ICommand? lastCommand = null;

                while (_commandLoopContext.HasNextIteration)
                {
                    if (!_commandLoopContext.CanPlayerGiveCommand)
                    {
                        await Task.Delay(100).ConfigureAwait(false);
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

                    await Task.Delay(100).ConfigureAwait(false);
                }
            }, cancellationToken);
        }

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
    }
}