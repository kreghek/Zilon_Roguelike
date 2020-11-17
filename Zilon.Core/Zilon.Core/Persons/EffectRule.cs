using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    public sealed class EffectRule
    {
        public EffectRule(RollEffectType? rollType, PersonRuleLevel level)
        {
            RollType = rollType;
            Level = level;
        }

        public RollEffectType? RollType { get; }

        public PersonRuleLevel Level { get; }
    }
}