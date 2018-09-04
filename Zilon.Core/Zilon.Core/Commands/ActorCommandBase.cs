using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с изменением состояния актёра.
    /// </summary>
    public abstract class ActorCommandBase : TacticCommandBase
    {
        protected readonly IPlayerState _playerState;

        protected ActorCommandBase(IPlayerState playerState) : base()
        {
            _playerState = playerState;
        }
    }
}