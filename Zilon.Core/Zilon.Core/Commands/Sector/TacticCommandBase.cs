using System;
using System.Diagnostics.CodeAnalysis;
using Zilon.Core.World;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для команд, связанных с изменением тактической обстановки в бою.
    /// </summary>
    public abstract class TacticCommandBase : ICommand
    {
        private readonly IGameLoop _gameLoop;
        private readonly IWorldManager _worldManager;

        protected virtual bool UpdateGameLoop => true;

        [ExcludeFromCodeCoverage]
        protected TacticCommandBase(IGameLoop gameLoop, IWorldManager worldManager)
        {
            _gameLoop = gameLoop;
            _worldManager = worldManager;
        }

        public abstract bool CanExecute();

        public void Execute()
        {
            var canExecute = CanExecute();
            if (!canExecute)
            {
                throw new InvalidOperationException("Попытка выполнить команду, которую нельзя выполнять в данный момент.");
            }

            ExecuteTacticCommand();

            //TODO Убрать из команд обновление мира
            // потому что эта операция длительная и должна вызываться явно.
            // Так же это позволит избавиться от 2-х зависимостей в команде - от геймлупа и от менеджера мира.
            if (UpdateGameLoop)
            {
                var globe = _worldManager.Globe;
                _gameLoop.UpdateAsync(globe).Wait();
            }
        }

        /// <summary>
        /// Выполнение тактических изменений.
        /// </summary>
        /// <returns>Возвращает реакцию на изменения.</returns>
        protected abstract void ExecuteTacticCommand();
    }
}
