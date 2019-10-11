using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.LogicCalculations;
using Zilon.Core.Persons.Auxiliary;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж, находящийся под управлением игрока.
    /// </summary>
    public class HumanPerson : IPerson
    {
        private readonly ITacticalActScheme _defaultActScheme;
        private readonly ISurvivalRandomSource _survivalRandomSource;

        public int Id { get; set; }

        public string Name { get; }

        public IEquipmentCarrier EquipmentCarrier { get; }

        public ITacticalActCarrier TacticalActCarrier { get; }

        public IEvolutionData EvolutionData { get; }

        public IPersonScheme Scheme { get; }

        public ICombatStats CombatStats { get; }

        public IPropStore Inventory { get; }

        public ISurvivalData Survival { get; }

        public EffectCollection Effects { get; }

        public HumanPerson([NotNull] IPersonScheme scheme,
            [NotNull] ITacticalActScheme defaultActScheme,
            [NotNull] IEvolutionData evolutionData,
            [NotNull] ISurvivalRandomSource survivalRandomSource)
        {
            _defaultActScheme = defaultActScheme ?? throw new ArgumentNullException(nameof(defaultActScheme));

            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
            EvolutionData = evolutionData ?? throw new ArgumentNullException(nameof(evolutionData));
            _survivalRandomSource = survivalRandomSource ?? throw new ArgumentNullException(nameof(survivalRandomSource));

            Name = scheme.Sid;

            Effects = new EffectCollection();
            Effects.Added += Effects_CollectionChanged;
            Effects.Removed += Effects_CollectionChanged;
            Effects.Changed += Effects_CollectionChanged;

            EquipmentCarrier = new EquipmentCarrier(Scheme.Slots);
            EquipmentCarrier.EquipmentChanged += EquipmentCarrier_EquipmentChanged;

            TacticalActCarrier = new TacticalActCarrier();

            EvolutionData.PerkLeveledUp += EvolutionData_PerkLeveledUp;


            CombatStats = new CombatStats();
            ClearCalculatedStats();
            CalcCombatStats();

            var perks = GetPerksSafe();
            TacticalActCarrier.Acts = CalcActs(_defaultActScheme, EquipmentCarrier, Effects, perks);

            Survival = new HumanSurvivalData(scheme, survivalRandomSource);
            Survival.StatChanged += Survival_StatCrossKeyValue;
            CalcSurvivalStats();
        }

        public HumanPerson(IPersonScheme scheme,
            [NotNull] ITacticalActScheme defaultScheme,
            [NotNull] IEvolutionData evolutionData,
            [NotNull] ISurvivalRandomSource survivalRandomSource,
            [NotNull] Inventory inventory) :
            this(scheme, defaultScheme, evolutionData, survivalRandomSource)
        {
            Inventory = inventory;
        }

        private void CalcCombatStats()
        {
            var bonusDict = new Dictionary<SkillStatType, float>();

            if (EvolutionData.Perks != null)
            {
                CalcPerkBonuses(bonusDict);
            }

            if (EvolutionData.Stats != null)
            {
                foreach (var bonusItem in bonusDict)
                {
                    var stat = EvolutionData.Stats.SingleOrDefault(x => x.Stat == bonusItem.Key);
                    if (stat != null)
                    {
                        ApplyBonusToStat(bonusItem.Value, stat);
                    }
                }

                foreach (var statItem in EvolutionData.Stats)
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
            var archievedPerks = EvolutionData.GetArchievedPerks();
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
            var equipmentArmors = new List<PersonArmorItem>();
            foreach (var equipment in EquipmentCarrier)
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

            CombatStats.DefenceStats.SetArmors(mergedArmors.ToArray());
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

        private IEnumerable<PersonArmorItem> MergeArmor(IEnumerable<PersonArmorItem> equipmentArmors)
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

        private void EquipmentCarrier_EquipmentChanged(object sender, EventArgs e)
        {
            ClearCalculatedStats();

            CalcCombatStats();

            var perks = GetPerksSafe();
            TacticalActCarrier.Acts = CalcActs(_defaultActScheme, EquipmentCarrier, Effects, perks);

            CalcSurvivalStats();
        }

        private IEnumerable<IPerk> GetPerksSafe()
        {
            var perks = EvolutionData?.GetArchievedPerks();
            if (perks == null)
            {
                perks = new IPerk[0];
            }

            return perks;
        }

        private void CalcSurvivalStats()
        {
            // Расчёт бонусов вынести в отдельный сервис, который покрыть модульными тестами
            // На вход принимает SurvivalData. SurvivalData дожен содержать метод увеличение диапазона характеристики.
            Survival.ResetStats();
            var bonusList = new List<SurvivalStatBonus>();
            FillSurvivalBonusesFromEquipments(ref bonusList);
            FillSurvivalBonusesFromPerks(ref bonusList);
            ApplySurvivalBonuses(bonusList);
        }

        private void FillSurvivalBonusesFromPerks([NotNull, ItemNotNull] ref List<SurvivalStatBonus> bonusList)
        {
            if (EvolutionData == null)
            {
                return;
            }

            var archievedPerks = EvolutionData.GetArchievedPerks();
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
                        if (rule.Type == PersonRuleType.Health)
                        {
                            BonusToHealth(rule.Level, PersonRuleDirection.Positive, ref bonusList);
                        }
                    }
                }
            }
        }

        private void ApplySurvivalBonuses(IEnumerable<SurvivalStatBonus> bonuses)
        {
            foreach (var bonus in bonuses)
            {
                var stat = Survival.Stats.SingleOrDefault(x => x.Type == bonus.SurvivalStatType);
                if (stat != null)
                {
                    var normalizedValueBonus = (int)Math.Round(bonus.ValueBonus, MidpointRounding.AwayFromZero);
                    stat.ChangeStatRange(stat.Range.Min, stat.Range.Max + normalizedValueBonus);
                    var normalizedDropPassBonus = (int)Math.Round(bonus.DownPassBonus, MidpointRounding.AwayFromZero);
                    stat.DownPassRoll = SurvivalStat.DEFAULT_DOWN_PASS_VALUE - normalizedDropPassBonus;
                }
            }
        }

        private void FillSurvivalBonusesFromEquipments([NotNull, ItemNotNull] ref List<SurvivalStatBonus> bonusList)
        {
            for (var i = 0; i < EquipmentCarrier.Count(); i++)
            {
                var equipment = EquipmentCarrier[i];
                if (equipment == null)
                {
                    continue;
                }

                var rules = equipment.Scheme.Equip.Rules;

                if (rules == null)
                {
                    continue;
                }

                foreach (var rule in rules)
                {
                    switch (rule.Type)
                    {
                        case EquipCommonRuleType.Health:
                            BonusToHealth(rule.Level, rule.Direction, ref bonusList);
                            break;

                        case EquipCommonRuleType.HealthIfNoBody:

                            var requirementsCompleted = true;

                            for (var slotIndex = 0; slotIndex < EquipmentCarrier.Count(); slotIndex++)
                            {
                                if ((EquipmentCarrier.Slots[slotIndex].Types & EquipmentSlotTypes.Body) > 0)
                                {
                                    if (EquipmentCarrier[slotIndex] != null)
                                    {
                                        requirementsCompleted = false;
                                        break;
                                    }
                                }
                            }

                            if (requirementsCompleted)
                            {
                                BonusToHealth(rule.Level, rule.Direction, ref bonusList);
                            }

                            break;

                        case EquipCommonRuleType.HungerResistance:
                            BonusToDownPass(SurvivalStatType.Satiety, rule.Level, rule.Direction, ref bonusList);
                            break;
                    }
                }
            }
        }

        private void BonusToDownPass(
            SurvivalStatType statType,
            PersonRuleLevel level,
            PersonRuleDirection direction,
            ref List<SurvivalStatBonus> bonuses)
        {
            var currentBonusValue = 0;
            var directionQuaff = direction == PersonRuleDirection.Negative ? -1 : 1;

            switch (level)
            {
                case PersonRuleLevel.Lesser:
                    currentBonusValue = 1 * directionQuaff;
                    break;

                case PersonRuleLevel.Normal:
                    currentBonusValue = 2 * directionQuaff;
                    break;

                case PersonRuleLevel.Grand:
                    currentBonusValue = 5 * directionQuaff;
                    break;

                case PersonRuleLevel.Absolute:
                    currentBonusValue = 10 * directionQuaff;
                    break;

                case PersonRuleLevel.None:
                default:
                    Debug.Fail("Предположительно, это ошибка.");
                    break;
            }

            var currentBonus = new SurvivalStatBonus(statType)
            {
                DownPassBonus = currentBonusValue
            };

            bonuses.Add(currentBonus);
        }

        /// <summary>
        /// Помещает в список бонус на ХП.
        /// </summary>
        /// <param name="level"> Уровень бонуса. </param>
        /// <param name="direction"> Направление бонуса. </param>
        /// <param name="bonuses"> Аккумулирующий список бонусов.
        /// Отмечен ref, чтобы показать, что изменяется внутри метода. </param>
        private void BonusToHealth(PersonRuleLevel level, PersonRuleDirection direction,
            ref List<SurvivalStatBonus> bonuses)
        {
            const SurvivalStatType hpStatType = SurvivalStatType.Health;
            var hpStat = Survival.Stats.SingleOrDefault(x => x.Type == hpStatType);
            if (hpStat != null)
            {
                var bonus = 0;
                switch (level)
                {
                    case PersonRuleLevel.Lesser:
                        bonus = 1;
                        break;

                    case PersonRuleLevel.Normal:
                        bonus = 3;
                        break;

                    case PersonRuleLevel.Grand:
                        bonus = 5;
                        break;

                    case PersonRuleLevel.Absolute:
                        bonus = 10;
                        break;
                }

                if (direction == PersonRuleDirection.Negative)
                {
                    bonus *= -1;
                }

                var currentBonus = bonuses.SingleOrDefault(x => x.SurvivalStatType == hpStatType);
                if (currentBonus == null)
                {
                    currentBonus = new SurvivalStatBonus(hpStatType);
                    bonuses.Add(currentBonus);
                }
                currentBonus.ValueBonus += bonus;
            }
        }

        private void EvolutionData_PerkLeveledUp(object sender, PerkEventArgs e)
        {
            ClearCalculatedStats();

            CalcCombatStats();

            var perks = GetPerksSafe();
            TacticalActCarrier.Acts = CalcActs(_defaultActScheme, EquipmentCarrier, Effects, perks);

            CalcSurvivalStats();
        }

        private void Survival_StatCrossKeyValue(object sender, SurvivalStatChangedEventArgs e)
        {
            PersonEffectHelper.UpdateSurvivalEffect(Effects, e.Stat, e.Stat.KeySegments, _survivalRandomSource);
        }


        private void Effects_CollectionChanged(object sender, EffectEventArgs e)
        {
            ClearCalculatedStats();

            if (EvolutionData != null)
            {
                CalcCombatStats();
            }

            var perks = GetPerksSafe();
            TacticalActCarrier.Acts = CalcActs(_defaultActScheme, EquipmentCarrier, Effects, perks);

            CalcSurvivalStats();
        }

        private static ITacticalAct[] CalcActs(ITacticalActScheme defaultActScheme,
            IEnumerable<Equipment> equipments,
            EffectCollection effects,
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
            [NotNull] EffectCollection effects,
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

        private static void CalcSurvivalHazardOnTacticalAct(EffectCollection effects,
            ref int toHitModifierValue,
            ref int efficientModifierValue)
        {
            var greaterSurvivalEffect = effects.Items.OfType<SurvivalStatHazardEffect>()
                            .OrderByDescending(x => x.Level).FirstOrDefault();

            if (greaterSurvivalEffect == null)
            {
                return;
            }

            var effecientDebuffRule = greaterSurvivalEffect.Rules
                .FirstOrDefault(x => x.RollType == RollEffectType.Efficient);

            var toHitDebuffRule = greaterSurvivalEffect.Rules
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
            foreach (var stat in EvolutionData.Stats)
            {
                stat.Value = 10;
            }
        }

        public override string ToString()
        {
            return $"{Name}";
        }

    }
}