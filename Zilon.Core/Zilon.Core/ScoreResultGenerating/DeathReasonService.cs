using System;

using Zilon.Core.Localization;
using Zilon.Core.Persons;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Zilon.Core.ScoreResultGenerating
{
    public class DeathReasonService : IDeathReasonService
    {
        private static string GetActorName(PlayerDamagedEvent playerDamagedEvent, Language language)
        {
            var monsterPerson = playerDamagedEvent?.Damager?.Person as MonsterPerson;

            if (monsterPerson == null)
            {
                throw new InvalidOperationException();
            }

            return language switch
            {
                Language.Ru => monsterPerson.Scheme.Name?.Ru ?? monsterPerson.Scheme.Sid,
                Language.En => monsterPerson.Scheme.Name?.En ?? monsterPerson.Scheme.Sid,
                _ => throw new InvalidOperationException()
            };
        }

        private static string? GetDeathReasonString(IPlayerEvent dominateEvent, Language language)
        {
            if (dominateEvent is null)
            {
                return null;
            }

            return dominateEvent switch
            {
                PlayerDamagedEvent playerDamagedEvent => GetActorName(playerDamagedEvent, language),
                SurvivalEffectDamageEvent survivalEffectDamageEvent => GetSurvivalEffectName(survivalEffectDamageEvent,
                    language),
                EndOfLifeEvent _ => "End of Life",
                _ => throw new InvalidOperationException()
            };
        }

        private static string GetSurvivalEffectName(SurvivalEffectDamageEvent survivalEffectDamageEvent,
            Language language)
        {
            switch (survivalEffectDamageEvent.Effect.Type)
            {
                case SurvivalStatType.Satiety:
                    return language switch
                    {
                        Language.Ru => "Голод",
                        Language.En => "Hunger",
                        _ => throw new InvalidOperationException()
                    };
                case SurvivalStatType.Hydration:
                    return language switch
                    {
                        Language.Ru => "Жажда",
                        Language.En => "Thirst",
                        _ => throw new InvalidOperationException()
                    };
                case SurvivalStatType.Intoxication:
                    return language switch
                    {
                        Language.Ru => "Токсикация",
                        Language.En => "Intoxication",
                        _ => throw new InvalidOperationException()
                    };
                default:
                    throw new InvalidOperationException();
            }
        }

        public string? GetDeathReasonSummary(IPlayerEvent playerEvent, Language language)
        {
            if (playerEvent is null)
            {
                throw new ArgumentNullException(nameof(playerEvent));
            }

            return GetDeathReasonString(playerEvent, language);
        }
    }
}