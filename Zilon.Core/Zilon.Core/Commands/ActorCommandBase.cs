using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с изменением состояния актёра.
    /// </summary>
    public abstract class ActorCommandBase : TacticCommandBase
    {
        protected readonly ISectorManager _sectorManager;
        protected readonly IPlayerState _playerState;

        protected ActorCommandBase(IGameLoop gameLoop, ISectorManager sectorManager, IPlayerState playerState) : base(gameLoop)
        {
            _sectorManager = sectorManager;
            _playerState = playerState;
        }
    }
}