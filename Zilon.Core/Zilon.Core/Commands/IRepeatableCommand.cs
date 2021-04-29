namespace Zilon.Core.Commands
{
    /// <summary>
    /// Повторяемая команда.
    /// </summary>
    public interface IRepeatableCommand : ICommand
    {
        /// <summary>
        /// Iteration of command in auto-execution. Used to debug.
        /// </summary>
        int RepeatIteration { get; }

        /// <summary>
        /// Метод определяет, может ли команда выполнить очередное повторение.
        /// </summary>
        /// <returns>
        /// Возвращает true - если команду можно повторить. Иначе, false.
        /// </returns>
        bool CanRepeat();

        /// <summary>
        /// Fix next iteration of command in aut-execution. Used to debug in <see cref="CommandLoopUpdater"/>.
        /// </summary>
        void IncreaceIteration();
    }
}