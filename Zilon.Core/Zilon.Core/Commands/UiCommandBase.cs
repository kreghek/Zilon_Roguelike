using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с взаимодействием с пользовательским интерфейсом.
    /// </summary>
    abstract class UiCommandBase : ICommand
    {
        public abstract void Execute();

        public abstract bool CanExecute();
    }
}