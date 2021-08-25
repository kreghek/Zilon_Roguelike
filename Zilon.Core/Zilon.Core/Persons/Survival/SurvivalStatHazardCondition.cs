﻿using System;
using System.Collections.Generic;

using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Scoring;

namespace Zilon.Core.Persons
{
    public class SurvivalStatHazardCondition : IPersonCondition, ISurvivalStatCondition
    {
        private readonly ISurvivalRandomSource _survivalRandomSource;
        private SurvivalStatHazardLevel _level;
        private ConditionRule[] _rules;

        public SurvivalStatHazardCondition(SurvivalStatType type,
            SurvivalStatHazardLevel level,
            ISurvivalRandomSource survivalRandomSource)
        {
            Type = type;
            Level = level;

            _survivalRandomSource =
                survivalRandomSource ?? throw new ArgumentNullException(nameof(survivalRandomSource));

            _rules = CalcRules();
        }

        public SurvivalStatHazardLevel Level
        {
            get => _level;
            set
            {
                if (_level != value)
                {
                    _level = value;

                    _rules = CalcRules();

                    Changed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public IPlayerEventLogService? PlayerEventLogService { get; set; }

        public SurvivalStatType Type { get; }

        public override string ToString()
        {
            return $"{Level} {Type}";
        }

        private ConditionRule[] CalcRules()
        {
            var rules = new List<ConditionRule>();

            switch (Level)
            {
                case SurvivalStatHazardLevel.Lesser:
                    rules.Add(new ConditionRule(RollEffectType.Efficient, PersonRuleLevel.Lesser));
                    break;

                case SurvivalStatHazardLevel.Strong:
                case SurvivalStatHazardLevel.Max:
                    rules.Add(new ConditionRule(RollEffectType.Efficient, PersonRuleLevel.Lesser));
                    rules.Add(new ConditionRule(RollEffectType.ToHit, PersonRuleLevel.Lesser));
                    break;

                case SurvivalStatHazardLevel.Undefined:
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException("Неизветный уровень угрозы выживания.");
            }

            return rules.ToArray();
        }

        private static int GetSuccessHazardDamageRoll()
        {
            // В будущем это значение будет расчитывать исходя из характеристик, перков и экипировки персонжа.
            return 4;
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

        public ConditionRule[] GetRules()
        {
            return _rules;
        }

        public event EventHandler? Changed;

        /// <summary>
        /// Применяет эффект к указанным данным.
        /// </summary>
        /// <param name="survivalData"> Данные, на коорые влияет эффект. </param>
        public void Apply(ISurvivalModule survivalData)
        {
            if (survivalData is null)
            {
                throw new ArgumentNullException(nameof(survivalData));
            }

            if (Level == SurvivalStatHazardLevel.Max && Type != SurvivalStatType.Health)
            {
                if (Type != SurvivalStatType.Energy)
                {
                    var roll = _survivalRandomSource.RollMaxHazardDamage();
                    var successRoll = GetSuccessHazardDamageRoll();
                    if (roll >= successRoll)
                    {
                        survivalData.DecreaseStat(SurvivalStatType.Health, 1);
                        LogPlayerEvent();
                    }
                }
                else
                {
                    survivalData.DecreaseStat(SurvivalStatType.Health, 1);
                    LogPlayerEvent();
                }
            }
        }
    }
}