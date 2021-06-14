using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Players;

namespace Zilon.Core.Scoring
{
    [ExcludeFromCodeCoverage]
    public class PlayerEventLogService : IPlayerEventLogService
    {
        private IPlayerEvent? _playerEvent;

        public PlayerEventLogService(IPlayer player)
        {
            Player = player;
        }

        public IPlayer Player { get; }

        public IPlayerEvent? GetPlayerEvent()
        {
            if (_playerEvent is null)
            {
                return null;
            }

            return _playerEvent;
        }

        public void Log(IPlayerEvent playerEvent)
        {
            _playerEvent = playerEvent;
        }
    }
}