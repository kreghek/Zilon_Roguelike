using Zilon.Core.Tactics;

namespace Zilon.Core.Scoring
{
    public class PlayerEventLogService : IPlayerEventLogService
    {
        private IPlayerEvent _playerEvent;

        public IActor Actor { get; set; }

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