using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Persons.Auxiliary;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж, находящийся под управлением игрока.
    /// </summary>
    public class HumanPerson : IPerson
    {
        private readonly ITacticalActScheme _defaultActScheme;

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

        public HumanPerson(IPersonScheme scheme, ITacticalActScheme defaultActScheme, IEvolutionData evolutionData)
        {
            _defaultActScheme = defaultActScheme ?? throw new ArgumentNullException(nameof(defaultActScheme));

            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
            EvolutionData = evolutionData ?? throw new ArgumentNullException(nameof(evolutionData));

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

            Survival = new SurvivalData(scheme);
            Survival.StatCrossKeyValue += Survival_StatCrossKeyValue;
        }

        public HumanPerson(IPersonScheme scheme, ITacticalActScheme defaultScheme, IEvolutionData evolutionData, Inventory inventory) :
            this(scheme, defaultScheme, evolutionData)
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

            foreach (var effect in Effects.Items)
            {
                foreach (var rule in effect.Rules)
                {
                    AddStatToDict(bonusDict, rule.StatType, rule.Level, PersonRuleDirection.Negative);
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
            TacticalActCarrier.Acts = CalcActs(EquipmentCarrier.Equipments);
        }

        private void EvolutionData_PerkLeveledUp(object sender, PerkEventArgs e)
        {
            ClearCalculatedStats();

            CalcCombatStats();
        }

        private void Survival_StatCrossKeyValue(object sender, SurvivalStatChangedEventArgs e)
        {
            PersonEffectHelper.UpdateSurvivalEffect(Effects, e.Stat, e.KeyPoint);
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

            var defaultAct = new TacticalAct(_defaultActScheme);
            actList.Insert(0, defaultAct);

            foreach (var equipment in equipments)
            {
                if (equipment == null)
                {
                    continue;
                }

                foreach (var actScheme in equipment.Acts)
                {
                    var act = new TacticalAct(actScheme);

                    actList.Insert(0, act);
                }
            }

            return actList.ToArray();
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