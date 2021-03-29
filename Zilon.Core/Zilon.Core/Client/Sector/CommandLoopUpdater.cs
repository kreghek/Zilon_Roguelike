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

        private readonly object _lockObj;

        private bool _hasPendingCommand;

        public CommandLoopUpdater(ICommandLoopContext commandLoopContext, ICommandPool commandPool)
        {
            _commandLoopContext = commandLoopContext ?? throw new ArgumentNullException(nameof(commandLoopContext));
            _commandPool = commandPool ?? throw new ArgumentNullException(nameof(commandPool));

            _lockObj = new object();
        }

        private ICommand? ExecuteCommandsInner(ICommand? lastCommand)
        {
            ICommand? commandWithError = null;
            ICommand? newLastCommand = null;

            lock (_lockObj)
            {
                _hasPendingCommand = true;

                var errorOccured = false;

                var command = _commandPool.Pop();

                try
                {
                    if (command != null)
                    {
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
                            if (repeatableCommand.CanRepeat() && repeatableCommand.CanExecute())
                            {
                                _commandPool.Push(repeatableCommand);
                                CommandAutoExecuted?.Invoke(this, EventArgs.Empty);
                            }
                            else
                            {
                                _hasPendingCommand = false;
                                CommandProcessed?.Invoke(this, EventArgs.Empty);
                            }
                        }
                        else
                        {
                            _hasPendingCommand = false;
                            CommandProcessed?.Invoke(this, EventArgs.Empty);
                        }

                        newLastCommand = command;
                    }
                    else
                    {
                        _hasPendingCommand = false;

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
                        _hasPendingCommand = false;
                        CommandProcessed?.Invoke(this, EventArgs.Empty);
                        newLastCommand = null;
                    }
                }
            }

            return newLastCommand;
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
                    await _commandLoopContext.WaitForUpdate(cancellationToken).ConfigureAwait(false);

                    try
                    {
                        lastCommand = ExecuteCommandsInner(lastCommand);
                    }
                    catch (Exception exception)
                    {
                        ErrorOccured?.Invoke(this, new ErrorOccuredEventArgs(exception));
                    }

                    await Task.Yield();
                }
            }, cancellationToken);
        }

        public bool IsStarted { get; private set; }

        public bool HasPendingCommands()
        {
            lock (_lockObj)
            {
                return _hasPendingCommand;
            }
        }
    }
}