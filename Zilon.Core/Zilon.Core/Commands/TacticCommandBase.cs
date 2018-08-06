using System;

using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для команд, связанных с изменением тактической обстановки в бою.
    /// </summary>
    public abstract class TacticCommandBase : ICommand
    {
        protected readonly ISectorManager _sectorManager;

        /// <summary>
        /// Признак того, что в конце команды необходимо выполнить обновление сектора.
        /// Означает, что команда немедленно выполнить передод к следующему игровому ходу.
        /// </summary>
        protected bool NeedToUpdateSector { get; }

        public abstract bool CanExecute();

        public void Execute()
        {
            var canExecute = CanExecute();
            if (!canExecute)
            {
                throw new InvalidOperationException("Попытка выполнить команду, которую нельзя выполнять в данный момент.");
            }

            ExecuteTacticCommand();

            if (NeedToUpdateSector)
            {
                var sector = _sectorManager.CurrentSector;
                sector.Update();
            }
        }

        protected TacticCommandBase(ISectorManager sectorManager)
        {
            _sectorManager = sectorManager;

            NeedToUpdateSector = true;
        }

        /// <summary>
        /// Выполнение тактических изменений.
        /// </summary>
        /// <returns>Возвращает реакцию на изменения.</returns>
        protected abstract void ExecuteTacticCommand();
    }
}
