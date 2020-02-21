using Zilon.Core.Localization;
using Zilon.Core.Scoring;

namespace Zilon.Core.ScoreResultGenerating
{
    public interface IDeathReasonService
    {
        string GetDeathReasonSummary(IPlayerEvent playerEvent, Language language);
    }
}