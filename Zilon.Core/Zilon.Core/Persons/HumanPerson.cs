using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Persons.Auxiliary;
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

        public int Hp { get; }

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
            Hp = scheme.Hp;

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

            TacticalActCarrier.Acts = CalcActs(EquipmentCarrier.Equipments);

            Survival = SurvivalData.CreateHumanPersonSurvival(scheme, survivalRandomSource);
            Survival.StatCrossKeyValue += Survival_StatCrossKeyValue;
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
                var archievedPerks = EvolutionData.Perks.Where(x => x.CurrentLevel != null).ToArray();
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
                            var ruleType = rule.Type;
                            switch (ruleType)
                            {
                                case PersonRuleType.Melee:
                                    AddStatToDict(bonusDict, SkillStatType.Melee, PersonRuleLevel.Lesser, PersonRuleDirection.Positive);
                                    break;

                                case PersonRuleType.Ballistic:
                                    AddStatToDict(bonusDict, SkillStatType.Ballistic, PersonRuleLevel.Lesser, PersonRuleDirection.Positive);
                                    break;
                            }
                        }
                    }
                }
            }

            if (EvolutionData.Stats != null)
            {
                foreach (var bonusItem in bonusDict)
                {
                    var stat = EvolutionData.Stats.SingleOrDefault(x => x.Stat == bonusItem.Key);
                    if (stat != null)
                    {
                        stat.Value += stat.Value * bonusItem.Value;

                        if (stat.Value <= 1)
                        {
                            stat.Value = 1;
                        }
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
        /// Пересчёт показателей брони персонажа.
        /// </summary>
        private void RecalculatePersonArmor()
        {
            var equipmentArmors = new List<PersonArmorItem>();
            foreach (var equipment in EquipmentCarrier.Equipments)
            {
                if (equipment == null)
                {
                    continue;
                }

                var equipStats = equipment.Scheme.Equip;

                if (equipStats.Armors != null)
                {
                    foreach (var propArmor in equipStats.Armors)
                    {
                        var personArmorItem = new PersonArmorItem(propArmor.Impact,
                            propArmor.AbsorbtionLevel,
                            propArmor.ArmorRank);

                        equipmentArmors.Add(personArmorItem);
                    }
                }
            }

            var mergedArmors = MergeArmor(equipmentArmors);

            CombatStats.DefenceStats.SetArmors(mergedArmors.ToArray());
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

                        if (armorLevel != null)
                        {
                            var levelDiff = GetLevelDiff(armor.AbsorbtionLevel, armorLevel.Value);
                            if (levelDiff > 0)
                            {
                                rankRaw += armor.ArmorRank * 0.33f * levelDiff;
                            }
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

            TacticalActCarrier.Acts = CalcActs(EquipmentCarrier.Equipments);
        }

        private void EvolutionData_PerkLeveledUp(object sender, PerkEventArgs e)
        {
            ClearCalculatedStats();

            CalcCombatStats();
        }

        private void Survival_StatCrossKeyValue(object sender, SurvivalStatChangedEventArgs e)
        {
            PersonEffectHelper.UpdateSurvivalEffect(Effects, e.Stat, e.KeyPoints, _survivalRandomSource);
        }


        private void Effects_CollectionChanged(object sender, EffectEventArgs e)
        {
            ClearCalculatedStats();

            if (EvolutionData != null)
            {
                CalcCombatStats();
            }

            TacticalActCarrier.Acts = CalcActs(EquipmentCarrier.Equipments);
        }

        private ITacticalAct[] CalcActs(IEnumerable<Equipment> equipments)
        {
            if (equipments == null)
            {
                throw new ArgumentNullException(nameof(equipments));
            }

            var actList = new List<ITacticalAct>();

            var defaultAct = CreateTacticalAct(_defaultActScheme, Effects);
            actList.Insert(0, defaultAct);

            foreach (var equipment in equipments)
            {
                if (equipment == null)
                {
                    continue;
                }

                foreach (var actScheme in equipment.Acts)
                {
                    var act = CreateTacticalAct(actScheme, Effects);

                    actList.Insert(0, act);
                }
            }

            return actList.ToArray();
        }

        private ITacticalAct CreateTacticalAct(ITacticalActScheme scheme, EffectCollection effects)
        {
            var greaterSurvivalEffect = effects.Items.OfType<SurvivalStatHazardEffect>()
                .OrderByDescending(x => x.Level).FirstOrDefault();

            if (greaterSurvivalEffect == null)
            {
                return new TacticalAct(scheme, scheme.Stats.Efficient, new Roll(6, 1));
            }
            else
            {
                var effecientBuffRule = greaterSurvivalEffect.Rules
                    .FirstOrDefault(x => x.RollType == RollEffectType.Efficient);

                var toHitBuffRule = greaterSurvivalEffect.Rules
                    .FirstOrDefault(x => x.RollType == RollEffectType.ToHit);

                var efficientRoll = scheme.Stats.Efficient;
                if (effecientBuffRule != null)
                {
                    var modifiers = new RollModifiers(-1);
                    efficientRoll = new Roll(efficientRoll.Dice, efficientRoll.Count, modifiers);
                }

                var toHitRoll = new Roll(6, 1);
                if (toHitBuffRule != null)
                {
                    var modifiers = new RollModifiers(-1);
                    toHitRoll = new Roll(6, 1, modifiers);
                }

                return new TacticalAct(scheme, efficientRoll, toHitRoll);
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