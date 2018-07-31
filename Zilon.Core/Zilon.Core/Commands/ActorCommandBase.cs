using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с изменением состояния актёра.
    /// </summary>
    public abstract class ActorCommandBase : TacticCommandBase
    {
        protected readonly IPlayerState _playerState;
        protected readonly ISectorManager _sectorManager;

        public ActorCommandBase(ISectorManager sectorManager,
            IPlayerState playerState)
        {
            _playerState = playerState;
            _sectorManager = sectorManager;
        }
    }
}