using System;
using System.Collections.Generic;
using Zilon.Core.Components;

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
            var rules = new List<EffectRule>();

            switch (Level)
            {
                case SurvivalStatHazardLevel.Lesser:
                    rules.Add(new EffectRule(RollEffectType.Efficient, PersonRuleLevel.Lesser));
                    break;

                case SurvivalStatHazardLevel.Strong:
                case SurvivalStatHazardLevel.Max:
                    rules.Add(new EffectRule(RollEffectType.Efficient, PersonRuleLevel.Lesser));
                    rules.Add(new EffectRule(RollEffectType.ToHit, PersonRuleLevel.Lesser));
                    break;

                default:
                    throw new NotSupportedException("Неизветный уровень угрозы выживания.");
            }

            return rules.ToArray();
        }

        public override string ToString()
        {
            return $"{Level} {Type}";
        }
    }
}
