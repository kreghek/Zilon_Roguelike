using System;

using Zilon.Core.Scoring;

namespace Zilon.Core.Persons
{
    public class SurvivalEffectDamageEvent : IPlayerEvent
    {
        public SurvivalEffectDamageEvent(SurvivalStatHazardCondition effect)
        {
            Effect = effect ?? throw new ArgumentNullException(nameof(effect));
        }

        public SurvivalStatHazardCondition Effect { get; }
        public string Key => $"{Effect.Type}";
        public int Weight => (int)Effect.Level;
    }
}