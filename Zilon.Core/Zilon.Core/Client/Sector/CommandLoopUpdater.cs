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
        private readonly IPlayer _player;
        private readonly ICommandManager _commandManager;

        public event EventHandler<ErrorOccuredEventArgs>? ErrorOccured;
        public event EventHandler? CommandAutoExecuted;

        public CommandLoopUpdater(IPlayer player, ICommandManager commandManager)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _commandManager = commandManager ?? throw new ArgumentNullException(nameof(commandManager));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var mainPerson = _player.MainPerson;
            if (mainPerson is null)
            {
                throw new InvalidOperationException("Main person is not defined to process commands.");
            }

            return Task.Run(() =>
            {
                var playerPersonSurvivalModule = mainPerson.GetModule<ISurvivalModule>();
                while (!playerPersonSurvivalModule.IsDead)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        ExecuteCommands();
                    }
                    catch (Exception exception)
                    {
                        ErrorOccured?.Invoke(this, new ErrorOccuredEventArgs(exception));
                    }
                }
            }, cancellationToken);
        }

        private bool _hasPendingCommand;

        private readonly object _lockObj = new object();

        private void ExecuteCommands()
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
                            }
                        }
                        else
                        {
                            _hasPendingCommand = false;
                        }
                    }
                    else
                    {
                        _hasPendingCommand = false;
                    }
                }
                catch (Exception exception)
                {
                    errorOccured = true;
                    throw new InvalidOperationException($"Не удалось выполнить команду {command}.", exception);
                }
                finally
                {
                    if (errorOccured)
                    {
                        _hasPendingCommand = false;
                    }
                }
            }
        }

        public bool HasPendingCommands()
        {
            lock (_lockObj)
            {
                return _hasPendingCommand;
            }
        }
    }
}
