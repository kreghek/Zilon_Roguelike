using System;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;

namespace Zilon.TextClient
{
    internal class CommandLoop
    {
        private readonly IPlayer _player;
        private readonly ICommandManager _commandManager;

        public CommandLoop(IPlayer player, ICommandManager commandManager)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _commandManager = commandManager ?? throw new ArgumentNullException(nameof(commandManager));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var playerPersonSurvivalModule = _player.MainPerson.GetModule<ISurvivalModule>();
                while (!playerPersonSurvivalModule.IsDead)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        ExecuteCommands();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }, cancellationToken);
        }

        private void ExecuteCommands()
        {
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
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Не удалось выполнить команду {command}.", exception);
            }
        }
    }
}
