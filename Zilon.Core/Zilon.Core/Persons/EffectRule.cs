using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    public sealed class EffectRule
    {
        public EffectRule(SkillStatType? statType, RollEffectType? roll, PersonRuleLevel level)
        {
            StatType = statType;
            Roll = roll;
            Level = level;
        }

        public SkillStatType? StatType { get; }

        public RollEffectType? Roll { get; }

        public PersonRuleLevel Level { get; }
    }
}
