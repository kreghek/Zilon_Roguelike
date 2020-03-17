using System;

using Zilon.Core.Localization;
using Zilon.Core.Persons;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Zilon.Core.ScoreResultGenerating
{
    public class DeathReasonService : IDeathReasonService
    {
        public string GetDeathReasonSummary(IPlayerEvent playerEvent, Language language)
        {
            if (playerEvent is null)
            {
                throw new ArgumentNullException(nameof(playerEvent));
            }

            return GetDeathReasonString(playerEvent, language);
        }

        private static string GetDeathReasonString(IPlayerEvent dominateEvent, Language language)
        {
            if (dominateEvent is null)
            {
                return null;
            }

            switch (dominateEvent)
            {
                case PlayerDamagedEvent playerDamagedEvent:
                    return GetActorName(playerDamagedEvent, language);

                case SurvivalEffectDamageEvent survivalEffectDamageEvent:
                    return GetSurvivalEffectName(survivalEffectDamageEvent, language);

                default:
                    throw new InvalidOperationException();
            }
        }

        private static string GetSurvivalEffectName(SurvivalEffectDamageEvent survivalEffectDamageEvent, Language language)
        {
            switch (survivalEffectDamageEvent.Effect.Type)
            {
                case SurvivalStatType.Satiety:
                    switch (language)
                    {
                        case Language.Ru:
                            return "Голод";
                        case Language.En:
                            return "Hunger";
                        default:
                            throw new InvalidOperationException();
                    }

                case SurvivalStatType.Hydration:
                    switch (language)
                    {
                        case Language.Ru:
                            return "Жажда";
                        case Language.En:
                            return "Thirst";
                        default:
                            throw new InvalidOperationException();
                    }

                case SurvivalStatType.Intoxication:
                    switch (language)
                    {
                        case Language.Ru:
                            return "Токсикация";
                        case Language.En:
                            return "Intoxication";
                        default:
                            throw new InvalidOperationException();
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        private static string GetActorName(PlayerDamagedEvent playerDamagedEvent, Language language)
        {
            var monsterPerson = playerDamagedEvent?.Damager?.Person as MonsterPerson;

            if (monsterPerson == null)
            {
                throw new InvalidOperationException();
            }

            switch (language)
            {
                case Language.Ru:
                    return monsterPerson.Scheme.Name.Ru;

                case Language.En:
                    return monsterPerson.Scheme.Name.En;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
