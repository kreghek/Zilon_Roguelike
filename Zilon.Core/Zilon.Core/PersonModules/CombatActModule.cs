using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Newtonsoft.Json;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.LogicCalculations;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Базовая реализация объекта для хранения сведений о тактических действиях персонажа.
    /// </summary>
    public sealed class CombatActModule : ICombatActModule
    {
        private readonly ITacticalActScheme _defaultActScheme;
        private readonly IEquipmentModule _equipmentModule;
        private readonly IEvolutionModule _evolutionModule;
        private readonly IConditionsModule _сonditionsModule;

        public CombatActModule(
            ITacticalActScheme defaultActScheme,
            IEquipmentModule equipmentModule,
            IConditionsModule сonditionsModule,
            IEvolutionModule evolutionModule)
        {
            IsActive = true;

            _defaultActScheme = defaultActScheme;
            _equipmentModule = equipmentModule;
            _сonditionsModule = сonditionsModule;
            _evolutionModule = evolutionModule;
        }

        private static IEnumerable<ITacticalAct> CalcActs(ITacticalActScheme defaultActScheme,
            IEnumerable<Equipment?> equipments,
            IConditionsModule сonditionModule,
            IEnumerable<IPerk> perks)
        {
            var defaultAct = CreateTacticalAct(defaultActScheme, null, сonditionModule, perks);
            yield return defaultAct;

            var equipmentActs = CalcActsFromEquipments(equipments, сonditionModule, perks);
            foreach (var act in equipmentActs)
            {
                yield return act;
            }
        }

        private static IEnumerable<ITacticalAct> CalcActsFromEquipments(
            IEnumerable<Equipment?> equipments,
            IConditionsModule сondition,
            IEnumerable<IPerk> perks)
        {
            if (equipments == null)
            {
                yield break;
            }

            foreach (var equipment in equipments)
            {
                if (equipment == null)
                {
                    continue;
                }

                foreach (var actScheme in equipment.Acts)
                {
                    var act = CreateTacticalAct(actScheme, equipment, сondition, perks);

                    yield return act;
                }
            }
        }

        private static void CalcPerksBonusesOnTacticalAct([NotNull] IEnumerable<IPerk> archievedPerks,
            [MaybeNull] Equipment? equipment,
            ref int toHitModifierValue,
            ref int efficientModifierValue)
        {
            foreach (var perk in archievedPerks)
            {
                if (perk.Scheme.IsBuildIn)
                {
                    if (perk.Scheme.Levels != null)
                    {
                        var perkLevelSubScheme0 = perk.Scheme.Levels[0];
                        if (perkLevelSubScheme0 is null)
                        {
                            continue;
                        }

                        var rules = perkLevelSubScheme0.Rules;

                        if (rules is null)
                        {
                            continue;
                        }

                        ProcessRulesBonuses(equipment, ref toHitModifierValue, ref efficientModifierValue, rules);
                    }
                }
                else
                {
                    var currentLevel = perk.CurrentLevel;
                    if (currentLevel is null)
                    {
                        continue;
                    }

                    if (perk.Scheme.Levels is null)
                    {
                        continue;
                    }

                    var currentLevelScheme = perk.Scheme.Levels[currentLevel.Primary - 1];

                    if (currentLevelScheme is null)
                    {
                        continue;
                    }

                    if (currentLevelScheme.Rules is null)
                    {
                        continue;
                    }

                    for (var i = 0; i <= currentLevel.Sub; i++)
                    {
                        var rules = currentLevelScheme.Rules;
                        ProcessRulesBonuses(equipment, ref toHitModifierValue, ref efficientModifierValue, rules);
                    }
                }
            }
        }

        private static void CalcSurvivalHazardOnTacticalAct(IConditionsModule сondition,
            ref int toHitModifierValue,
            ref int efficientModifierValue)
        {
            var greaterSurvivalEffect = сondition.Items.OfType<SurvivalStatHazardCondition>()
                .OrderByDescending(x => x.Level).FirstOrDefault();

            if (greaterSurvivalEffect == null)
            {
                return;
            }

            var effecientDebuffRule = greaterSurvivalEffect.GetRules()
                .FirstOrDefault(x => x.RollType == RollEffectType.Efficient);

            var toHitDebuffRule = greaterSurvivalEffect.GetRules()
                .FirstOrDefault(x => x.RollType == RollEffectType.ToHit);

            if (effecientDebuffRule != null)
            {
                efficientModifierValue += -1;
            }

            if (toHitDebuffRule != null)
            {
                toHitModifierValue += -1;
            }
        }

        private static ITacticalAct CreateTacticalAct([NotNull] ITacticalActScheme scheme,
            [MaybeNull] Equipment? equipment,
            [NotNull] IConditionsModule сonditionModule,
            [NotNull] IEnumerable<IPerk> perks)
        {
            var toHitModifierValue = 0;
            var efficientModifierValue = 0;
            var efficientRollUnmodified = scheme.Stats?.Efficient ?? new Roll(1, 1);
            CalcSurvivalHazardOnTacticalAct(сonditionModule, ref toHitModifierValue, ref efficientModifierValue);
            CalcPerksBonusesOnTacticalAct(perks, equipment, ref toHitModifierValue, ref efficientModifierValue);

            var toHitRoll = CreateTacticalActRoll(6, 1, toHitModifierValue);

            var efficientRoll = CreateTacticalActRoll(efficientRollUnmodified.Dice,
                efficientRollUnmodified.Count,
                efficientModifierValue);

            return new TacticalAct(scheme, efficientRoll, toHitRoll, equipment);
        }

        private static Roll CreateTacticalActRoll(int dice, int count, int modifierValue)
        {
            if (modifierValue == 0)
            {
                return new Roll(dice, count);
            }

            var modifier = new RollModifiers(modifierValue);
            return new Roll(dice, count, modifier);
        }

        private IEnumerable<IPerk> GetPerksSafe()
        {
            var perks = _evolutionModule?.GetArchievedPerks();
            if (perks == null)
            {
                perks = Array.Empty<IPerk>();
            }

            return perks;
        }

        private static int GetRollModifierByPerkRule(Equipment? equipment, int efficientModifierValue,
            PerkRuleSubScheme rule)
        {
            if (rule.Params is null || string.IsNullOrWhiteSpace(rule.Params))
            {
                efficientModifierValue = RuleCalculations.CalcEfficientByRuleLevel(efficientModifierValue, rule.Level);
            }
            else
            {
                var damagePerkParams = JsonConvert.DeserializeObject<DamagePerkParams>(rule.Params);

                if (damagePerkParams is null)
                {
                    throw new InvalidOperationException("Error n serialization of damagePerkParams.");
                }

                if (damagePerkParams.WeaponTags != null && equipment != null)
                {
                    var hasAllTags = true;
                    foreach (var requiredTag in damagePerkParams.WeaponTags)
                    {
                        if (equipment.Scheme.Tags?.Contains(requiredTag) != true)
                        {
                            hasAllTags = false;
                            break;
                        }
                    }

                    if (hasAllTags)
                    {
                        efficientModifierValue =
                            RuleCalculations.CalcEfficientByRuleLevel(efficientModifierValue, rule.Level);
                    }
                }
            }

            return efficientModifierValue;
        }

        private static void GetRuleModifierValue(PerkRuleSubScheme rule, Equipment? equipment,
            ref int toHitModifierValue, ref int efficientModifierValue)
        {
            switch (rule.Type)
            {
                case PersonRuleType.Damage:
                    efficientModifierValue = GetRollModifierByPerkRule(equipment, efficientModifierValue, rule);
                    break;

                case PersonRuleType.ToHit:
                    toHitModifierValue = GetRollModifierByPerkRule(equipment, toHitModifierValue, rule);
                    break;

                case PersonRuleType.Undefined:
                    throw new InvalidOperationException("Rule is not defined.");

                case PersonRuleType.Health:
                case PersonRuleType.HealthIfNoBody:
                case PersonRuleType.HungerResistance:
                case PersonRuleType.ThristResistance:
                    // This perk rule is not influence to combat.
                    break;

                default:
                    throw new InvalidOperationException($"Rule {rule.Type} unknown.");
            }
        }

        private static void ProcessRulesBonuses(Equipment? equipment, ref int toHitModifierValue,
            ref int efficientModifierValue, PerkRuleSubScheme?[] rules)
        {
            foreach (var rule in rules)
            {
                if (rule is null)
                {
                    continue;
                }

                GetRuleModifierValue(rule, equipment, ref toHitModifierValue, ref efficientModifierValue);
            }
        }

        public string Key => nameof(ICombatActModule);
        public bool IsActive { get; set; }

        public IEnumerable<ITacticalAct> CalcCombatActs()
        {
            var perks = GetPerksSafe();
            return CalcActs(_defaultActScheme, _equipmentModule, _сonditionsModule, perks);
        }
    }
}