using System;

using Zilon.Core.Persons;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Zilon.Core.ScoreResultGenerating
{
    public class DeathReasonService
    {
        public string GetDeathReasonSummary(IPlayerEvent playerEvent)
        {
            if (playerEvent is null)
            {
                throw new ArgumentNullException(nameof(playerEvent));
            }

            return GetDeathReasonString(playerEvent);
        }

        private static string GetDeathReasonString(IPlayerEvent dominateEvent)
        {
            if (dominateEvent is null)
            {
                return null;
            }

            switch (dominateEvent)
            {
                case PlayerDamagedEvent playerDamagedEvent:
                    return $"{playerDamagedEvent.Damager}";

                case SurvivalEffectDamageEvent survivalEffectDamageEvent:
                    return $"{survivalEffectDamageEvent.Effect.Type}";

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
