using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Zilon.Core.Components;
using Zilon.Core.Scoring;

namespace Zilon.Core.Persons
{
    public class SurvivalStatHazardEffect : IPersonEffect, ISurvivalStatEffect
    {
        private SurvivalStatHazardLevel _level;
        private EffectRule[] _rules;
        private readonly ISurvivalRandomSource _survivalRandomSource;

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        public SurvivalStatHazardEffect(SurvivalStatType type,
            SurvivalStatHazardLevel level,
            [NotNull] ISurvivalRandomSource survivalRandomSource)
        {
            Type = type;
            Level = level;

            _survivalRandomSource = survivalRandomSource ?? throw new ArgumentNullException(nameof(survivalRandomSource));

            _rules = CalcRules();
        }

        public SurvivalStatType Type { get; }

        public SurvivalStatHazardLevel Level
        {
            get => _level;
            set
            {
                _level = value;

                _rules = CalcRules();

                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public EffectRule[] GetRules()
        {
            return _rules;
        }

        public event EventHandler Changed;

        /// <summary>
        /// Применяет эффект к указанным данным.
        /// </summary>
        /// <param name="survivalData"> Данные, на коорые влияет эффект. </param>
        public void Apply(ISurvivalData survivalData)
        {
            if (survivalData is null)
            {
                throw new ArgumentNullException(nameof(survivalData));
            }

            if (Level == SurvivalStatHazardLevel.Max && Type != SurvivalStatType.Health)
            {
                var roll = _survivalRandomSource.RollMaxHazardDamage();
                var successRoll = GetSuccessHazardDamageRoll();
                if (roll >= successRoll)
                {
                    survivalData.DecreaseStat(SurvivalStatType.Health, 1);
                    LogPlayerEvent();
                }
            }
        }

        private void LogPlayerEvent()
        {
            if (PlayerEventLogService is null)
            {
                return;
            }

            var playerEvent = new SurvivalEffectDamageEvent(this);
            PlayerEventLogService.Log(playerEvent);
        }

        private static int GetSuccessHazardDamageRoll()
        {
            // В будущем это значение будет расчитывать исходя из характеристик, перков и экипировки персонжа.
            return 4;
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

                case SurvivalStatHazardLevel.Undefined:
                    throw new NotSupportedException();

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
