using Zilon.Core.Tactics;

namespace Zilon.Core.Scoring
{
    public interface IPlayerEventLogService
    {
        void Log(IPlayerEvent playerEvent);

        IActor Actor { get; set; }

        IPlayerEvent GetPlayerEvent();
    }
}
