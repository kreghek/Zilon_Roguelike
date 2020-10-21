using System;

using Zilon.Core.Players;

namespace Zilon.Core.Scoring
{
    public class PlayerEventLogService : IPlayerEventLogService
    {
        private IPlayerEvent _playerEvent;

        public PlayerEventLogService(IPlayer player)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public IPlayer Player { get; }

        public IPlayerEvent GetPlayerEvent()
        {
            return _playerEvent;
        }

        public void Log(IPlayerEvent playerEvent)
        {
            _playerEvent = playerEvent;
        }
    }
}