using System.Collections.Generic;
using Zilon.Core.Tactics;

namespace Zilon.Core.Scoring
{
    public class PlayerEventLogService : IPlayerEventLogService
    {
        private readonly List<IPlayerEvent> _playerEvents;

        public PlayerEventLogService()
        {
            _playerEvents = new List<IPlayerEvent>();
        }

        public IActor Actor { get; }

        public void Log(IPlayerEvent playerEvent)
        {
            _playerEvents.Add(playerEvent);
        }
    }
}
