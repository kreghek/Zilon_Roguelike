using System;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для команд, связанных с изменением тактической обстановки в бою.
    /// </summary>
    public abstract class TacticCommandBase : ICommand<SectorCommandContext>
    {
        public abstract bool CanExecute(SectorCommandContext context);

        public void Execute(SectorCommandContext context)
        {
            var canExecute = CanExecute(context);
            if (!canExecute)
            {
                throw new InvalidOperationException("Попытка выполнить команду, которую нельзя выполнять в данный момент.");
            }

            ExecuteTacticCommand(context);
        }

        /// <summary>
        /// Выполнение тактических изменений.
        /// </summary>
        /// <returns>Возвращает реакцию на изменения.</returns>
        protected abstract void ExecuteTacticCommand(SectorCommandContext context);
    }
}
