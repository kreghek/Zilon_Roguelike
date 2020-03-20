namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Базовая команда для всех команд, связанных с взаимодействием с пользовательским интерфейсом.
    /// </summary>
    public abstract class UiCommandBase<TContext> : ICommand<TContext>
    {
        /// <summary>
        /// Выполнение команды.
        /// </summary>
        public abstract void Execute(TContext context);

        /// <summary>Проверяет, возможно ли выполнение команды.</summary>
        /// <returns>
        /// Возвращает true, если команду можно выполнить. Иначе возвращает false.
        /// </returns>
        public abstract bool CanExecute(TContext context);
    }
}