using Assets.Zilon.Scripts.Models.SectorScene;
using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с изменением состояния актёра.
    /// </summary>
    abstract class ActorCommandBase : TacticCommandBase
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