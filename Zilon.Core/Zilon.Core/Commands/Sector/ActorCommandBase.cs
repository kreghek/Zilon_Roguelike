using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с изменением состояния актёра.
    /// </summary>
    public abstract class ActorCommandBase : TacticCommandBase
    {
        protected readonly ISectorManager SectorManager;
        protected readonly ISectorUiState PlayerState;

        [ExcludeFromCodeCoverage]
        protected ActorCommandBase(IGameLoop gameLoop,
            ISectorManager sectorManager,
            ISectorUiState playerState) : base(gameLoop)
        {
            SectorManager = sectorManager;
            PlayerState = playerState;
        }

        /// <summary>
        /// Текущий активный актёр.
        /// </summary>
        [CanBeNull]
        public IActor CurrentActor => PlayerState.ActiveActor?.Actor;

        /// <summary>
        /// Модель представления текущего актёра.
        /// </summary>
        [CanBeNull]
        public IActorViewModel CurrentActorViewModel => PlayerState.ActiveActor;
    }
}