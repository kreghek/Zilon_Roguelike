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
        void Execute();
        bool CanExecute();
    }
}
