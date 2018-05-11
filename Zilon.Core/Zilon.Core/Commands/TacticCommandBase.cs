using Zilon.Core.Services.CombatEvents;
using Zilon.Core.Tactics.Events;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для команд, связанных с изменением тактической обстановки в бою.
    /// </summary>
    public abstract class TacticCommandBase : ICommand
    {
        protected readonly IEventManager _eventManager;

        public abstract bool CanExecute();
        public void Execute()
        {
            var tacticEvents = ExecuteTacticCommand();

            if (tacticEvents != null)
            {
                _eventManager.SetEvents(tacticEvents);
            }
        }

        public TacticCommandBase(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        /// <summary>
        /// Выполнение тактических изменений.
        /// </summary>
        /// <returns>Возвращает реакцию на изменения.</returns>
        protected abstract ITacticEvent[] ExecuteTacticCommand();
    }
}
