namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Базовая команда для всех команд, связанных с взаимодействием с пользовательским интерфейсом.
    /// </summary>
    public abstract class UiCommandBase : ICommand
    {
        public abstract void Execute();

        public abstract bool CanExecute();
    }
}