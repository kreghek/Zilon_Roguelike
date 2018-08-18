using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Components;
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

        public List<IPersonEffect> Effects { get; }

        public HumanPerson(PersonScheme scheme, IEvolutionData evolutionData)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            Scheme = scheme;

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

            Effects = new List<IPersonEffect>();

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
                        var targetValue = 1;

                        var ruleType = rule.Type;
                        switch (ruleType)
                        {
                            case PersonRuleType.Melee:
                                AddStat(combatStats, CombatStatType.Melee, targetValue);
                                break;

                            case PersonRuleType.Ballistic:
                                AddStat(combatStats, CombatStatType.Ballistic, targetValue);
                                break;
                        }
                    }
                }
            }
        }

        private static void AddStat(ICombatStats combatStats, CombatStatType targetStat, int targetValue)
        {
            var statItem = combatStats.Stats.SingleOrDefault(x => x.Stat == targetStat);
            if (statItem != null)
            {
                statItem.Value += targetValue;
            }
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
            switch (e.KeyPoint.Type)
            {
                case SurvivalStatKeyPointType.Lesser:
                    if (e.Stat.Type == SurvivalStatType.Satiety)
                    {
                        if (e.Stat.Value >= e.KeyPoint.Value)
                        {
                            var currentEffect = Effects.SingleOrDefault(x => x.Name == "Слабый голод");
                            if (currentEffect == null)
                            {
                                Effects.Add(new HungerEffect { Name = "Слабый голод" });
                            }
                        }
                    }
                    break;
            }
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
