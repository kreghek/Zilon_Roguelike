using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с открытием модальных окон.
    /// </summary>
    public abstract class ShowModalCommandBase : UiCommandBase
    {
        protected readonly ISectorModalManager ModalManager;

        [ExcludeFromCodeCoverage]
        protected ShowModalCommandBase(ISectorModalManager modalManager)
        {
            ModalManager = modalManager;
        }
    }
}