using System;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;

namespace Zilon.Core.Client.Sector
{
    public class CommandLoopUpdater : ICommandLoopUpdater
    {
        private readonly ICommandManager _commandManager;

        private readonly object _lockObj = new object();
        private readonly IPlayer _player;

        private bool _hasPendingCommand;

        public CommandLoopUpdater(IPlayer player, ICommandManager commandManager)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _commandManager = commandManager ?? throw new ArgumentNullException(nameof(commandManager));
        }

        private void ExecuteCommandsInner(ICommand? lastCommand)
        {
            lock (_lockObj)
            {
                _hasPendingCommand = true;

                var errorOccured = false;

                var command = _commandManager.Pop();

                try
                {
                    if (command != null)
                    {
                        command.Execute();

                        if (command is IRepeatableCommand repeatableCommand)
                        {
                            if (repeatableCommand.CanRepeat())
                            {
                                _commandManager.Push(repeatableCommand);
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

                        lastCommand = command;
                    }
                    else
                    {
                        if (lastCommand != null)
                        {
                            _hasPendingCommand = false;
                            CommandProcessed?.Invoke(this, EventArgs.Empty);
                            lastCommand = null;
                        }
                    }
                }
                catch (Exception exception)
                {
                    errorOccured = true;
                    ErrorOccured?.Invoke(this, new ErrorOccuredEventArgs(exception));
                    lastCommand = null;
                }
                finally
                {
                    if (errorOccured)
                    {
                        _hasPendingCommand = false;
                        CommandProcessed?.Invoke(this, EventArgs.Empty);
                        lastCommand = null;
                    }
                }
            }
        }

        public event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;
        public event EventHandler? CommandAutoExecuted;
        public event EventHandler? CommandProcessed;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var mainPerson = _player.MainPerson;
            if (mainPerson is null)
            {
                throw new InvalidOperationException("Main person is not defined to process commands.");
            }

            IsStarted = true;

            return Task.Run(() =>
            {
                var playerPersonSurvivalModule = mainPerson.GetModule<ISurvivalModule>();

                ICommand? lastCommand = null;

                while (!playerPersonSurvivalModule.IsDead)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        ExecuteCommandsInner(lastCommand);
                    }
                    catch (Exception exception)
                    {
                        ErrorOccured?.Invoke(this, new ErrorOccuredEventArgs(exception));
                    }
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