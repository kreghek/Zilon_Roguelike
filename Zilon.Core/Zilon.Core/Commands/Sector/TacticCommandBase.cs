using System;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для команд, связанных с изменением тактической обстановки в бою.
    /// </summary>
    public abstract class TacticCommandBase : ICommand
    {
        /// <summary>
        /// Выполнение тактических изменений.
        /// </summary>
        /// <returns>Возвращает реакцию на изменения.</returns>
        protected abstract void ExecuteTacticCommand();

        public abstract CanExecuteCheckResult CanExecute();

        public void Execute()
        {
            var canExecuteCheckResult = CanExecute();
            if (!canExecuteCheckResult.IsSuccess)
            {
                throw new InvalidOperationException(
                    $"Attempt to execute the inapplicable command. Reason: {canExecuteCheckResult.FailureReason}.");
            }

            ExecuteTacticCommand();
        }
    }
}