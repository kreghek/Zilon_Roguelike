namespace Zilon.Core.Commands
{
    /// <summary>
    /// Интерфейс команды.
    /// </summary>
    /// <remarks>
    /// Изменение состояния модели возможно только через команды.
    /// </remarks>
    public interface ICommand
    {
        /// <summary>
        /// Проверяет, возможно ли выполнение команды.
        /// </summary>
        /// <returns> Возвращает true, если команду можно выполнить. Иначе возвращает false. </returns>
        CanExecuteCheckResult CanExecute();

        /// <summary>
        /// Выполнение команды.
        /// </summary>
        void Execute();
    }

    public struct CanExecuteCheckResult
    {
        public string FailureReason { get; set; }
        public bool IsSuccess { get; set; }
    }
}