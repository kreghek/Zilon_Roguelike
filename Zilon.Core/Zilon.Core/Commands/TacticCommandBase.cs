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
            ExecuteTacticCommand();
        }

        public TacticCommandBase()
        {
            
        }

        /// <summary>
        /// Выполнение тактических изменений.
        /// </summary>
        /// <returns>Возвращает реакцию на изменения.</returns>
        protected abstract void ExecuteTacticCommand();
    }
}
