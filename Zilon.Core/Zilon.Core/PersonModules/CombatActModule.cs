using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

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
        private readonly IEffectsModule _effectsModule;
        private readonly IEvolutionModule _evolutionModule;

        public CombatActModule(
            ITacticalActScheme defaultActScheme,
            IEquipmentModule equipmentModule,
            IEffectsModule effectsModule,
            IEvolutionModule evolutionModule)
        {
            IsActive = true;

            _defaultActScheme = defaultActScheme;
            _equipmentModule = equipmentModule;
            _effectsModule = effectsModule;
            _evolutionModule = evolutionModule;
        }

        public string Key { get => nameof(ICombatActModule); }
        public bool IsActive { get; set; }

        public IEnumerable<ITacticalAct> CalcCombatActs()
        {
            var perks = GetPerksSafe();
            return CalcActs(_defaultActScheme, _equipmentModule, _effectsModule, perks);
        }

        private static IEnumerable<ITacticalAct> CalcActs(ITacticalActScheme defaultActScheme,
            IEnumerable<Equipment> equipments,
            IEffectsModule effects,
            IEnumerable<IPerk> perks)
        {
            var defaultAct = CreateTacticalAct(defaultActScheme, equipment: null, effects: effects, perks: perks);
            yield return defaultAct;

            var equipmentActs = CalcActsFromEquipments(equipments, effects, perks);
            foreach (var act in equipmentActs)
            {
                yield return act;
            }
        }

        private static IEnumerable<ITacticalAct> CalcActsFromEquipments(
            IEnumerable<Equipment> equipments,
            IEffectsModule effects,
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
                    var act = CreateTacticalAct(actScheme, equipment, effects, perks);

                    yield return act;
                }
            }
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

        private static ITacticalAct CreateTacticalAct([NotNull] ITacticalActScheme scheme,
            [NotNull] Equipment equipment,
            [NotNull] IEffectsModule effects,
            [NotNull, ItemNotNull] IEnumerable<IPerk> perks)
        {
            var toHitModifierValue = 0;
            var efficientModifierValue = 0;
            var efficientRollUnmodified = scheme.Stats.Efficient;
            CalcSurvivalHazardOnTacticalAct(effects, ref toHitModifierValue, ref efficientModifierValue);
            CalcPerkBonusesOnTacticalAct(perks, equipment, ref toHitModifierValue, ref efficientModifierValue);

            var toHitRoll = CreateTacticalActRoll(6, 1, toHitModifierValue);
            var efficientRoll = CreateTacticalActRoll(efficientRollUnmodified.Dice,
                efficientRollUnmodified.Count,
                efficientModifierValue);

            return new TacticalAct(scheme, efficientRoll, toHitRoll, equipment);
        }

        private static void CalcPerkBonusesOnTacticalAct([NotNull, ItemNotNull] IEnumerable<IPerk> archievedPerks,
            [NotNull] Equipment equipment,
            ref int toHitModifierValue,
            ref int efficientModifierValue)
        {
            foreach (var perk in archievedPerks)
            {
                if (perk.Scheme.IsBuildIn)
                {
                    if (perk.Scheme.Levels != null)
                    {
                        foreach (var rule in perk.Scheme.Levels[0].Rules)
                        {
                            GetRuleModifierValue(rule, equipment, ref toHitModifierValue, ref efficientModifierValue);
                        }
                    }
                }
                else
                {
                    var currentLevel = perk.CurrentLevel;
                    var currentLevelScheme = perk.Scheme.Levels[currentLevel.Primary];

                    if (currentLevelScheme.Rules == null)
                    {
                        continue;
                    }

                    for (var i = 0; i <= currentLevel.Sub; i++)
                    {
                        foreach (var rule in currentLevelScheme.Rules)
                        {
                            GetRuleModifierValue(rule, equipment, ref toHitModifierValue, ref efficientModifierValue);
                        }
                    }
                }
            }
        }

        private static void GetRuleModifierValue(PerkRuleSubScheme rule, Equipment equipment, ref int toHitModifierValue, ref int efficientModifierValue)
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
                    throw new InvalidOperationException("Правило не определно.");

                default:
                    // Все остальные правила игнорируем, потому что этот модуль их не обрабатывает.
                    break;
            }
        }

        private static int GetRollModifierByPerkRule(Equipment equipment, int efficientModifierValue, PerkRuleSubScheme rule)
        {
            if (string.IsNullOrWhiteSpace(rule.Params))
            {
                efficientModifierValue = RuleCalculations.CalcEfficientByRuleLevel(efficientModifierValue, rule.Level);
            }
            else
            {
                var damagePerkParams = JsonConvert.DeserializeObject<DamagePerkParams>(rule.Params);
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
                        efficientModifierValue = RuleCalculations.CalcEfficientByRuleLevel(efficientModifierValue, rule.Level);
                    }
                }
            }

            return efficientModifierValue;
        }

        private static void CalcSurvivalHazardOnTacticalAct(IEffectsModule effects,
            ref int toHitModifierValue,
            ref int efficientModifierValue)
        {
            var greaterSurvivalEffect = effects.Items.OfType<SurvivalStatHazardEffect>()
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

        private static Roll CreateTacticalActRoll(int dice, int count, int modifierValue)
        {
            if (modifierValue == 0)
            {
                return new Roll(dice, count);
            }
            else
            {
                var modifier = new RollModifiers(modifierValue);
                return new Roll(dice, count, modifier);
            }
        }
    }
}