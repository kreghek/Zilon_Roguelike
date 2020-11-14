using System.Diagnostics.CodeAnalysis;
using Zilon.Core.Client.Windows;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с открытием модальных окон.
    /// </summary>
    public abstract class ShowModalCommandBase : UiCommandBase
    {
        /// <summary>
        /// Создаёт экземпляр <see cref="ShowModalCommandBase"/>.
        /// </summary>
        /// <param name="modalManager"> Менеджер модальных окон. Реализация на клиенте. </param>
        [ExcludeFromCodeCoverage]
        protected ShowModalCommandBase(ISectorModalManager modalManager)
        {
            ModalManager = modalManager;
        }

        /// <summary>
        /// Менеджер модальных окон.
        /// </summary>
        protected ISectorModalManager ModalManager { get; }
    }
}