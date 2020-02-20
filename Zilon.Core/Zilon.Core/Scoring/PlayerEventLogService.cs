using System.Collections.Generic;

using Zilon.Core.Tactics;

namespace Zilon.Core.Scoring
{
    public class PlayerEventLogService : IPlayerEventLogService
    {
        private readonly List<IPlayerEvent> _playerEvents;

        public PlayerEventLogService()
        {
            _playerEvents = new List<IPlayerEvent>(10);
        }

        public IActor Actor { get; set; }

        public IPlayerEvent[] GetPlayerEvents()
        { 
            return _playerEvents.ToArray();
        }

        public void Log(IPlayerEvent playerEvent)
        {
            KeepStoredEventListCount();

            _playerEvents.Add(playerEvent);
        }

        private void KeepStoredEventListCount()
        {
            if (_playerEvents.Count >= 10)
            {
                var lengthDiff = _playerEvents.Count - 10 + 1;
                for (var i = 0; i < lengthDiff; i++)
                {
                    _playerEvents.RemoveAt(0);
                }
            }
        }
    }
}
