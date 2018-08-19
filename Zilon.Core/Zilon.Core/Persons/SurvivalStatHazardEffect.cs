using System;
using Zilon.Core.Components;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public class SurvivalStatHazardEffect : IPersonEffect, IActorStateEffect
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

        public void Apply(IActorState actorState)
        {
            if (Level == SurvivalStatHazardLevel.Max)
            {
                actorState.TakeDamage(5);
            }
        }

        public void ApplyEveryTurn(ICombatStats combatStats)
        {
            // Этот эффект не влияет на характерстики каждый ход.
        }

        public void ApplyOnce(ICombatStats combatStats)
        {
            var q = 1f;
            switch (Level)
            {
                case SurvivalStatHazardLevel.Lesser:
                    q = 0.9f;
                    break;

                case SurvivalStatHazardLevel.Strong:
                    q = 0.7f;
                    break;

                case SurvivalStatHazardLevel.Max:
                    q = 0.5f;
                    break;

                default:
                    throw new NotSupportedException("Неизветный уровень угрозы выживания.");
            }

            foreach (var item in combatStats.Stats)
            {
                item.Value = q * item.Value;
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
                new EffectRule{
                    Level = ruleLevel,
                    StatType = CombatStatType.Ballistic
                },
                new EffectRule{
                    Level = ruleLevel,
                    StatType = CombatStatType.Medic
                },
                new EffectRule{
                    Level = ruleLevel,
                    StatType = CombatStatType.Melee
                },
                new EffectRule{
                    Level = ruleLevel,
                    StatType = CombatStatType.Psy
                },
                new EffectRule{
                    Level = ruleLevel,
                    StatType = CombatStatType.Social
                },
                new EffectRule{
                    Level = ruleLevel,
                    StatType = CombatStatType.Tech
                },
            };
        }
    }
}
