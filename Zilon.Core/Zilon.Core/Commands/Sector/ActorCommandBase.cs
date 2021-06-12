using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с изменением состояния актёра.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class ActorCommandBase : TacticCommandBase
    {
        protected ActorCommandBase(ISectorUiState playerState)
        {
            PlayerState = playerState;
        }

        /// <summary>
        /// Текущий активный актёр.
        /// </summary>
        public IActor? CurrentActor => PlayerState.ActiveActor?.Actor;

        /// <summary>
        /// Модель представления текущего актёра.
        /// </summary>
        public IActorViewModel? CurrentActorViewModel => PlayerState.ActiveActor;

        protected ISectorUiState PlayerState { get; }
    }
}