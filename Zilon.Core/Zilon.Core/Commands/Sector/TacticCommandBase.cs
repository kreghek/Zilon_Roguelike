using System;
using System.Diagnostics.CodeAnalysis;
using Zilon.Core.Tactics;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для команд, связанных с изменением тактической обстановки в бою.
    /// </summary>
    public abstract class TacticCommandBase : ICommand
    {
        private readonly IGameLoop _gameLoop;

        protected virtual bool UpdateGameLoop => true;

        [ExcludeFromCodeCoverage]
        protected TacticCommandBase(IGameLoop gameLoop)
        {
            _gameLoop = gameLoop;
        }

        public abstract bool CanExecute();

        public void Execute()
        {
            var canExecute = CanExecute();
            if (!canExecute)
            {
                throw new InvalidOperationException("Попытка выполнить команду, которую нельзя выполнять в данный момент.");
            }

            ExecuteTacticCommand();

            if (UpdateGameLoop)
            {
                _gameLoop.UpdateAsync();
            }
        }

        /// <summary>
        /// Выполнение тактических изменений.
        /// </summary>
        /// <returns>Возвращает реакцию на изменения.</returns>
        protected abstract void ExecuteTacticCommand();
    }
}
