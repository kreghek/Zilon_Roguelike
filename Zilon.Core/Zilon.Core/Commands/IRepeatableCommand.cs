namespace Zilon.Core.Commands
{
    /// <summary>
    /// Повторяемая команда.
    /// </summary>
    public interface IRepeatableCommand<TContext> : ICommand<TContext>
    {
        /// <summary>
        /// Метод определяет, может ли команда выполнить очередное повторение.
        /// </summary>
        /// <returns>
        /// Возвращает true - если команду можно повторить. Иначе, false.
        /// </returns>
        bool CanRepeat(TContext context);
    }
}
