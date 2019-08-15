using System.Diagnostics.CodeAnalysis;

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

        public IActor Actor => PlayerState.ActiveActor.Actor;

        public IActorViewModel ActorViewModel => PlayerState.ActiveActor;
    }
}