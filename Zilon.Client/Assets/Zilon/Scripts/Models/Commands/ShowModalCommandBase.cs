using Assets.Zilon.Scripts.Services;
using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с открытием модальных окон.
    /// </summary>
    abstract class ShowModalCommandBase : ICommand
    {
        protected readonly ISectorModalManager ModalManager;

        protected ShowModalCommandBase(ISectorModalManager modalManager)
        {
            ModalManager = modalManager;
        }

        public abstract void Execute();

        public abstract bool CanExecute();
    }
}