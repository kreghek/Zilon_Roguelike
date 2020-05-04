using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.LogicCalculations;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons.Auxiliary;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж, находящийся под управлением игрока.
    /// </summary>
    public class HumanPerson : PersonBase
    {
        private readonly ITacticalActScheme _defaultActScheme;
        private readonly ISurvivalRandomSource _survivalRandomSource;

        /// <inheritdoc/>
        public override int Id { get; set; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IPersonScheme Scheme { get; }

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        public override PhysicalSize PhysicalSize { get => PhysicalSize.Size1; }

        public HumanPerson([NotNull] IPersonScheme scheme,
            [NotNull] ITacticalActScheme defaultActScheme) : base()
        {
            _defaultActScheme = defaultActScheme ?? throw new ArgumentNullException(nameof(defaultActScheme));

            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));

            Name = scheme.Sid;

            evolutionModule.PerkLeveledUp += EvolutionData_PerkLeveledUp;

            ClearCalculatedStats();
            CalcCombatStats();

            var perks = GetPerksSafe();
            combatActModule.Acts = CalcActs(_defaultActScheme, equipmentModule, effects, perks);

            var diseaseModule = new DiseaseModule();
            AddModule(diseaseModule);
        }

        private void CalcCombatStats()
        {
            var bonusDict = new Dictionary<SkillStatType, float>();

            if (this.GetModuleSafe<IEvolutionModule>().Perks != null)
            {
                CalcPerkBonuses(bonusDict);
            }

            if (this.GetModuleSafe<IEvolutionModule>().Stats != null)
            {
                foreach (var bonusItem in bonusDict)
                {
                    var stat = this.GetModule<IEvolutionModule>().Stats.SingleOrDefault(x => x.Stat == bonusItem.Key);
                    if (stat != null)
                    {
                        ApplyBonusToStat(bonusItem.Value, stat);
                    }
                }

                foreach (var statItem in this.GetModule<IEvolutionModule>().Stats)
                {
                    statItem.Value = (float)Math.Round(statItem.Value, 1);
                }
            }

            RecalculatePersonArmor();
        }

        /// <summary>
        /// Применение бонуса к характеристике навыка.
        /// </summary>
        /// <param name="bonusValue"> Величина бонуса. </param>
        /// <param name="stat"> Характеристика навыка. </param>
        private static void ApplyBonusToStat(float bonusValue, SkillStatItem stat)
        {
            stat.Value += stat.Value * bonusValue;

            if (stat.Value <= 1)
            {
                stat.Value = 1;
            }
        }

        /// <summary>
        /// Расчёт бонусов, которые дают перки.
        /// </summary>
        /// <param name="bonusDict"> Текущее состояние бонусов. </param>
        private void CalcPerkBonuses(Dictionary<SkillStatType, float> bonusDict)
        {
            var archievedPerks = this.GetModule<IEvolutionModule>().GetArchievedPerks();
            foreach (var archievedPerk in archievedPerks)
            {
                var currentLevel = archievedPerk.CurrentLevel;
                var currentLevelScheme = archievedPerk.Scheme.Levels[currentLevel.Primary];

                if (currentLevelScheme.Rules == null)
                {
                    continue;
                }

                for (var i = 0; i <= currentLevel.Sub; i++)
                {
                    foreach (var rule in currentLevelScheme.Rules)
                    {
                        CalcRuleBonuses(rule, bonusDict);
                    }
                }
            }
        }

        /// <summary>
        /// Расчёт бонуса, который даёт правило перка.
        /// </summary>
        /// <param name="rule"> Правило перка. </param>
        /// <param name="bonusDict"> Текущее состояние бонусов. </param>
        private static void CalcRuleBonuses(PerkRuleSubScheme rule, Dictionary<SkillStatType, float> bonusDict)
        {
            switch (rule.Type)
            {
                case PersonRuleType.Melee:
                    AddStatToDict(bonusDict, SkillStatType.Melee, rule.Level, PersonRuleDirection.Positive);
                    break;

                case PersonRuleType.Ballistic:
                    AddStatToDict(bonusDict, SkillStatType.Ballistic, rule.Level, PersonRuleDirection.Positive);
                    break;

                    //case PersonRuleType.Undefined:
                    //default:
                    //    throw new ArgumentOutOfRangeException($"Тип правила перка {rule.Type} не поддерживается.");
            }
        }

        /// <summary>
        /// Пересчёт показателей брони персонажа.
        /// </summary>
        private void RecalculatePersonArmor()
        {
            var equipmentModule = this.GetModule<IEquipmentModule>();

            var equipmentArmors = new List<PersonArmorItem>();
            foreach (var equipment in equipmentModule)
            {
                if (equipment == null)
                {
                    continue;
                }

                var equipStats = equipment.Scheme.Equip;

                if (equipStats.Armors != null)
                {
                    var currentEquipmentArmors = GetEquipmentArmors(equipStats.Armors);
                    equipmentArmors.AddRange(currentEquipmentArmors);
                }
            }

            var mergedArmors = MergeArmor(equipmentArmors);

            this.GetModule<ICombatStatsModule>().DefenceStats.SetArmors(mergedArmors.ToArray());
        }

        private static IEnumerable<PersonArmorItem> GetEquipmentArmors(IEnumerable<IPropArmorItemSubScheme> armors)
        {
            foreach (var propArmor in armors)
            {
                var personArmorItem = new PersonArmorItem(propArmor.Impact,
                    propArmor.AbsorbtionLevel,
                    propArmor.ArmorRank);

                yield return personArmorItem;
            }
        }

        private static IEnumerable<PersonArmorItem> MergeArmor(IEnumerable<PersonArmorItem> equipmentArmors)
        {
            var armorGroups = equipmentArmors.GroupBy(x => x.Impact).OrderBy(x => x.Key);

            foreach (var armorGroup in armorGroups)
            {
                var orderedArmors = from armor in armorGroup
                                    orderby armor.AbsorbtionLevel, armor.ArmorRank
                                    select armor;

                float? rankRaw = null;
                PersonRuleLevel? armorLevel = null;
                var minInited = false;
                foreach (var armor in orderedArmors)
                {
                    //т.к. вся броня упорядочена от худшей
                    // первым будет обработан элемент с худшими показателями
                    if (!minInited)
                    {
                        minInited = true;
                        rankRaw = armor.ArmorRank;
                        armorLevel = armor.AbsorbtionLevel;
                    }
                    else
                    {
                        rankRaw += armor.ArmorRank * 0.5f;

                        var levelDiff = GetLevelDiff(armor.AbsorbtionLevel, armorLevel.Value);
                        if (levelDiff > 0)
                        {
                            rankRaw += armor.ArmorRank * 0.33f * levelDiff;
                        }
                    }
                }

                if (rankRaw != null)
                {
                    var totalRankRaw = Math.Round(rankRaw.Value);

                    yield return new PersonArmorItem(armorGroup.Key,
                        armorLevel.Value,
                        (int)totalRankRaw);
                }
            }
        }

        private static int GetLevelDiff(PersonRuleLevel level, PersonRuleLevel baseLevel)
        {
            var a = GetArmorModifierByLevel(level);
            var b = GetArmorModifierByLevel(baseLevel);
            return a - b;
        }

        private static int GetArmorModifierByLevel(PersonRuleLevel level)
        {
            switch (level)
            {
                case PersonRuleLevel.None:
                    return 0;

                case PersonRuleLevel.Lesser:
                    return 1;

                case PersonRuleLevel.Normal:
                    return 2;

                case PersonRuleLevel.Grand:
                    return 3;

                case PersonRuleLevel.Absolute:
                    return 5;

                default:
                    throw new ArgumentException($"Неизвестное значение уровня {level}.", nameof(level));
            }
        }

        private static void AddStatToDict(Dictionary<SkillStatType, float> bonusDict,
            SkillStatType targetStatType,
            PersonRuleLevel level,
            PersonRuleDirection direction)
        {
            bonusDict.TryGetValue(targetStatType, out float value);

            float q;
            switch (level)
            {
                case PersonRuleLevel.Lesser:
                    q = 0.1f;
                    break;

                case PersonRuleLevel.Normal:
                    q = 0.3f;
                    break;

                case PersonRuleLevel.Grand:
                    q = 0.5f;
                    break;

                case PersonRuleLevel.None:
                    throw new NotSupportedException();

                case PersonRuleLevel.Absolute:
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException($"Неизветный уровень угрозы выживания {level}.");
            }

            switch (direction)
            {
                case PersonRuleDirection.Positive:
                    // Бонус изначально расчитывается, как положительный. Ничего не делаем.
                    break;
                case PersonRuleDirection.Negative:
                    q *= -1;
                    break;

                default:
                    throw new NotSupportedException($"Неизветный уровень угрозы выживания {direction}.");
            }

            value += q;

            bonusDict[targetStatType] = value;
        }

        private void EquipmentModule_EquipmentChanged(object sender, EventArgs e)
        {
            var equipmentModule = this.GetModule<IEquipmentModule>();

            ClearCalculatedStats();

            CalcCombatStats();

            var perks = GetPerksSafe();
            this.GetModule<ICombatActModule>().Acts = CalcActs(_defaultActScheme, equipmentModule, this.GetModule<IEffectsModule>(), perks);

            CalcSurvivalStats();
        }

        private IEnumerable<IPerk> GetPerksSafe()
        {
            var perks = this.GetModuleSafe<IEvolutionModule>()?.GetArchievedPerks();
            if (perks == null)
            {
                perks = Array.Empty<IPerk>();
            }

            return perks;
        }

        private void EvolutionData_PerkLeveledUp(object sender, PerkEventArgs e)
        {
            ClearCalculatedStats();

            CalcCombatStats();

            var perks = GetPerksSafe();

            var equipmentModule = this.GetModule<IEquipmentModule>();
            this.GetModule<ICombatActModule>().Acts = CalcActs(_defaultActScheme, equipmentModule, this.GetModule<IEffectsModule>(), perks);

            CalcSurvivalStats();
        }

        private void Survival_StatCrossKeyValue(object sender, SurvivalStatChangedEventArgs e)
        {
            if (e.Stat.KeySegments is null)
            {
                return;
            }

            PersonEffectHelper.UpdateSurvivalEffect(
                this.GetModule<IEffectsModule>(),
                e.Stat,
                e.Stat.KeySegments,
                _survivalRandomSource,
                PlayerEventLogService);
        }


        private void Effects_CollectionChanged(object sender, EffectEventArgs e)
        {
            ClearCalculatedStats();

            if (this.GetModuleSafe<IEvolutionModule>() != null)
            {
                CalcCombatStats();
            }

            var perks = GetPerksSafe();

            var equipmentModule = this.GetModule<IEquipmentModule>();
            this.GetModule<ICombatActModule>().Acts = CalcActs(_defaultActScheme, equipmentModule, this.GetModule<IEffectsModule>(), perks);

            CalcSurvivalStats();
        }

        private static ITacticalAct[] CalcActs(ITacticalActScheme defaultActScheme,
            IEnumerable<Equipment> equipments,
            IEffectsModule effects,
            IEnumerable<IPerk> perks)
        {
            if (equipments == null)
            {
                throw new ArgumentNullException(nameof(equipments));
            }

            var actList = new List<ITacticalAct>();

            var defaultAct = CreateTacticalAct(defaultActScheme, equipment: null, effects: effects, perks: perks);
            actList.Insert(0, defaultAct);

            foreach (var equipment in equipments)
            {
                if (equipment == null)
                {
                    continue;
                }

                foreach (var actScheme in equipment.Acts)
                {
                    var act = CreateTacticalAct(actScheme, equipment, effects, perks);

                    actList.Insert(0, act);
                }
            }

            return actList.ToArray();
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
                        switch (rule.Type)
                        {
                            case PersonRuleType.Damage:
                                efficientModifierValue = GetRollModifierByPerkRule(equipment, efficientModifierValue, rule);
                                break;

                            case PersonRuleType.ToHit:
                                toHitModifierValue = GetRollModifierByPerkRule(equipment, toHitModifierValue, rule);
                                break;
                        }
                    }
                }
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

        private void ClearCalculatedStats()
        {
            foreach (var stat in this.GetModule<IEvolutionModule>().Stats)
            {
                stat.Value = 10;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}