using System;
using Zilon.Core.Components;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public class SurvivalStatHazardEffect : IPersonEffect, ISurvivalStatEffect
    {
        private SurvivalStatHazardLevel _level;

        public SurvivalStatHazardEffect(SurvivalStatType type, SurvivalStatHazardLevel level)
        {
            Type = type;
            Level = level;

            Rules = CalcRules();
        }

        public SurvivalStatType Type { get; }

        public SurvivalStatHazardLevel Level
        {
            get => _level;
            set
            {
                _level = value;

                Rules = CalcRules();

                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public EffectRule[] Rules { get; private set; }

        public event EventHandler Changed;

        public void Apply(ISurvivalData survivalData)
        {
            if (Level == SurvivalStatHazardLevel.Max)
            {
                survivalData.DecreaseStat(SurvivalStatType.Health, 1);
            }
        }

        public void Update()
        {
            // На персонажа нет влияния
        }

        private EffectRule[] CalcRules()
        {
            PersonRuleLevel ruleLevel;
            switch (Level)
            {
                case SurvivalStatHazardLevel.Lesser:
                    ruleLevel = PersonRuleLevel.Lesser;
                    break;

                case SurvivalStatHazardLevel.Strong:
                    ruleLevel = PersonRuleLevel.Normal;
                    break;

                case SurvivalStatHazardLevel.Max:
                    ruleLevel = PersonRuleLevel.Grand;
                    break;

                default:
                    throw new NotSupportedException("Неизветный уровень угрозы выживания.");
            }

            return new[] {
                new EffectRule(
                    statType:SkillStatType.Ballistic,
                    roll: null,
                    level: ruleLevel
                ),
                new EffectRule(
                    statType:SkillStatType.Medic,
                    roll: null,
                    level: ruleLevel
                ),
                new EffectRule(
                    statType:SkillStatType.Melee,
                    roll: null,
                    level: ruleLevel
                ),
                new EffectRule(
                    statType:SkillStatType.Psy,
                    roll: null,
                    level: ruleLevel
                ),
                new EffectRule(
                    statType:SkillStatType.Social,
                    roll: null,
                    level: ruleLevel
                ),
                new EffectRule(
                    statType:SkillStatType.Tech,
                    roll: null,
                    level: ruleLevel
                )
            };
        }

        public override string ToString()
        {
            return $"{Level} {Type}";
        }
    }
}
