using System;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для команд, связанных с изменением тактической обстановки в бою.
    /// </summary>
    public abstract class TacticCommandBase : ICommand
    {

        public abstract bool CanExecute();

        public void Execute()
        {
            var canExecute = CanExecute();
            if (!canExecute)
            {
                throw new InvalidOperationException("Попытка выполнить команду, которую нельзя выполнять в данный момент.");
            }

            ExecuteTacticCommand();
        }

        /// <summary>
        /// Выполнение тактических изменений.
        /// </summary>
        /// <returns>Возвращает реакцию на изменения.</returns>
        protected abstract void ExecuteTacticCommand();
    }
}
