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
        public int Id { get; set; }

        public string Name { get; }

        public float Hp => Scheme.Hp;

        public IEquipmentCarrier EquipmentCarrier { get; }

        public ITacticalActCarrier TacticalActCarrier { get; }

        public IEvolutionData EvolutionData { get; }

        public PersonScheme Scheme { get; }

        public ICombatStats CombatStats { get; }

        public IPropStore Inventory { get; }

        public ISurvivalData Survival { get; }

        public EffectCollection Effects { get; }

        public HumanPerson(PersonScheme scheme, IEvolutionData evolutionData)
        {
            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));

            Effects = new EffectCollection();
            Effects.Added += Effects_CollectionChanged;
            Effects.Removed += Effects_CollectionChanged;
            Effects.Changed += Effects_CollectionChanged;

            var slotCount = Scheme.SlotCount;
            EquipmentCarrier = new EquipmentCarrier(slotCount);
            EquipmentCarrier.EquipmentChanged += EquipmentCarrier_EquipmentChanged;

            TacticalActCarrier = new TacticalActCarrier();


            EvolutionData = evolutionData;

            if (EvolutionData != null)
            {
                EvolutionData.PerkLeveledUp += EvolutionData_PerkLeveledUp;
            }

            CombatStats = new CombatStats();
            ClearCombatStats((CombatStats)CombatStats);

            if (EvolutionData != null)
            {
                CalcCombatStats(CombatStats, EvolutionData);
            }

            Survival = new SurvivalData();
            Survival.StatCrossKeyValue += Survival_StatCrossKeyValue;
        }

        public HumanPerson(PersonScheme scheme, IEvolutionData evolutionData, Inventory inventory):
            this(scheme, evolutionData)
        {
            Inventory = inventory;
        }

        private void CalcCombatStats(ICombatStats combatStats, IEvolutionData evolutionData)
        {
            var bonusDict = new Dictionary<CombatStatType, float>();

            var archievedPerks = evolutionData.Perks.Where(x => x.CurrentLevel != null).ToArray();
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
                                AddStatToDict(bonusDict, CombatStatType.Melee, PersonRuleLevel.Lesser, PersonRuleDirection.Positive);
                                break;

                            case PersonRuleType.Ballistic:
                                AddStatToDict(bonusDict, CombatStatType.Ballistic, PersonRuleLevel.Lesser, PersonRuleDirection.Positive);
                                break;
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

            foreach (var bonusItem in bonusDict)
            {
                var stat = CombatStats.Stats.SingleOrDefault(x => x.Stat == bonusItem.Key);
                if (stat != null)
                {
                    stat.Value += stat.Value * bonusItem.Value;

                    if (stat.Value <= 1)
                    {
                        stat.Value = 1;
                    }
                }
            }

            foreach (var statItem in CombatStats.Stats)
            {
                statItem.Value = (float)Math.Round(statItem.Value, 1);
            }
        }

        private static void AddStatToDict(Dictionary<CombatStatType, float> bonusDict,
            CombatStatType targetStatType,
            PersonRuleLevel level,
            PersonRuleDirection direction)
        {
            bonusDict.TryGetValue(targetStatType, out float value);

            var q = 0f;
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
            TacticalActCarrier.Acts = CalcActs(EquipmentCarrier.Equipments, CombatStats);
        }

        private void EvolutionData_PerkLeveledUp(object sender, PerkEventArgs e)
        {
            ClearCombatStats((CombatStats)CombatStats);

            if (EvolutionData != null)
            {
                CalcCombatStats(CombatStats, EvolutionData);
            }
        }

        private void Survival_StatCrossKeyValue(object sender, SurvivalStatChangedEventArgs e)
        {
            PersonEffectHelper.UpdateSurvivalEffect(Effects, e.Stat, e.KeyPoint);
        }


        private void Effects_CollectionChanged(object sender, EffectEventArgs e)
        {
            ClearCombatStats((CombatStats)CombatStats);

            if (EvolutionData != null)
            {
                CalcCombatStats(CombatStats, EvolutionData);
            }

            TacticalActCarrier.Acts = CalcActs(EquipmentCarrier.Equipments, CombatStats);
        }

        private static ITacticalAct[] CalcActs(IEnumerable<Equipment> equipments, ICombatStats combatStats)
        {
            if (equipments == null)
            {
                throw new ArgumentNullException(nameof(equipments));
            }

            var actList = new List<ITacticalAct>();

            foreach (var equipment in equipments)
            {
                if (equipment == null)
                {
                    continue;
                }

                foreach (var actScheme in equipment.Acts)
                {
                    var equipmentPower = CalcEquipmentEfficient(equipment);
                    var act = new TacticalAct(equipmentPower, actScheme, combatStats);

                    actList.Add(act);
                }
            }

            return actList.ToArray();
        }

        private static float CalcEquipmentEfficient(Equipment equipment)
        {
            return equipment.Power * equipment.Scheme.Equip.Power;
        }

        private void ClearCombatStats(CombatStats combatStats)
        {
            //TODO Статы рассчитывать на основании схемы персонажа, перков, экипировки
            combatStats.Stats = new[]{
                new CombatStatItem {Stat = CombatStatType.Melee, Value = 10 },
                new CombatStatItem {Stat = CombatStatType.Ballistic, Value = 10 }
            };
        }

        public override string ToString()
        {
            return $"{Name}";
        }

    }
}
