﻿using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Базовая реализация набора навыков для боя.
    /// </summary>
    public sealed class CombatStatsModule : ICombatStatsModule
    {
        private readonly IEquipmentModule _equipmentModule;
        private readonly IEvolutionModule _evolutionModule;

        public CombatStatsModule(IEvolutionModule evolutionModule, IEquipmentModule equipmentModule)
        {
            DefenceStats = new PersonDefenceStats(Array.Empty<PersonDefenceItem>(), Array.Empty<PersonArmorItem>());
            IsActive = true;

            _evolutionModule = evolutionModule ?? throw new ArgumentNullException(nameof(evolutionModule));
            _equipmentModule = equipmentModule ?? throw new ArgumentNullException(nameof(equipmentModule));

            _equipmentModule.EquipmentChanged += EquipmentModule_EquipmentChanged;
        }

        private static void AddStatToDict(Dictionary<SkillStatType, float> bonusDict,
            SkillStatType targetStatType,
            PersonRuleLevel level,
            PersonRuleDirection direction)
        {
            bonusDict.TryGetValue(targetStatType, out float value);
            var q = level switch
            {
                PersonRuleLevel.Lesser => 0.1f,
                PersonRuleLevel.Normal => 0.3f,
                PersonRuleLevel.Grand => 0.5f,
                PersonRuleLevel.None => throw new NotSupportedException(),
                PersonRuleLevel.Absolute => throw new NotSupportedException(),
                _ => throw new NotSupportedException($"Неизветный уровень угрозы выживания {level}.")
            };
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

        private void CalcCombatStats()
        {
            var bonusDict = new Dictionary<SkillStatType, float>();

            if (_evolutionModule.Perks != null)
            {
                CalcPerkBonuses(bonusDict);
            }

            if (_evolutionModule.Stats != null)
            {
                foreach (var bonusItem in bonusDict)
                {
                    var stat = _evolutionModule.Stats.SingleOrDefault(x => x.Stat == bonusItem.Key);
                    if (stat != null)
                    {
                        ApplyBonusToStat(bonusItem.Value, stat);
                    }
                }

                foreach (var statItem in _evolutionModule.Stats)
                {
                    statItem.Value = (float)Math.Round(statItem.Value, 1);
                }
            }

            RecalculatePersonArmor();
        }

        /// <summary>
        /// Расчёт бонусов, которые дают перки.
        /// </summary>
        /// <param name="bonusDict"> Текущее состояние бонусов. </param>
        private void CalcPerkBonuses(Dictionary<SkillStatType, float> bonusDict)
        {
            var archievedPerks = _evolutionModule.GetArchievedPerks();
            foreach (var archievedPerk in archievedPerks)
            {
                PerkLevel? currentLevel = archievedPerk.CurrentLevel;

                if (archievedPerk.Scheme.IsBuildIn)
                {
                    if (archievedPerk.Scheme.Levels is null)
                    {
                        continue;
                    }

                    currentLevel = new PerkLevel(1, 1);
                }

                var levels = archievedPerk.Scheme.Levels;
                if (levels is null)
                {
                    continue;
                }

                if (currentLevel is null)
                {
                    continue;
                }

                var currentLevelScheme = levels[currentLevel.Primary - 1];

                if (currentLevelScheme is null)
                {
                    continue;
                }

                if (currentLevelScheme.Rules == null)
                {
                    continue;
                }

                for (var i = 1; i <= currentLevel.Sub; i++)
                {
                    foreach (var rule in currentLevelScheme.Rules)
                    {
                        if (rule is null)
                        {
                            continue;
                        }

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

                case PersonRuleType.Health:
                case PersonRuleType.Damage:
                case PersonRuleType.ToHit:
                case PersonRuleType.HealthIfNoBody:
                case PersonRuleType.HungerResistance:
                case PersonRuleType.ThristResistance:
                    // This perk rule is not impact to combat stats.
                    break;

                case PersonRuleType.Undefined:
                    throw new InvalidOperationException("Undefined rule");

                default:
                    throw new InvalidOperationException($"Rule {rule.Type} unknown");
            }
        }

        private void EquipmentModule_EquipmentChanged(object sender, EquipmentChangedEventArgs e)
        {
            CalcCombatStats();
        }

        private static int GetArmorModifierByLevel(PersonRuleLevel level)
        {
            return level switch
            {
                PersonRuleLevel.None => 0,
                PersonRuleLevel.Lesser => 1,
                PersonRuleLevel.Normal => 2,
                PersonRuleLevel.Grand => 3,
                PersonRuleLevel.Absolute => 5,
                _ => throw new ArgumentException($"Неизвестное значение уровня {level}.", nameof(level))
            };
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

        private static int GetLevelDiff(PersonRuleLevel level, PersonRuleLevel baseLevel)
        {
            var a = GetArmorModifierByLevel(level);
            var b = GetArmorModifierByLevel(baseLevel);
            return a - b;
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
                foreach (var armor in orderedArmors)
                {
                    // Because all armors are ordered by desc.
                    // First the armor with minimun level will be processed.
                    if (armorLevel is null)
                    {
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

                if (rankRaw != null && armorLevel != null)
                {
                    var totalRankRaw = Math.Round(rankRaw.Value);

                    yield return new PersonArmorItem(armorGroup.Key,
                        armorLevel.Value,
                        (int)totalRankRaw);
                }
            }
        }

        /// <summary>
        /// Пересчёт показателей брони персонажа.
        /// </summary>
        private void RecalculatePersonArmor()
        {
            var equipmentModule = _equipmentModule;

            var equipmentArmors = new List<PersonArmorItem>();
            foreach (var equipment in equipmentModule)
            {
                if (equipment == null)
                {
                    continue;
                }

                var equipStats = equipment.Scheme.Equip;

                var armors = equipStats?.Armors;
                if (armors != null)
                {
                    var notNullArmors = armors.Where(x => x != null).Select(x => x!).ToArray();
                    var currentEquipmentArmors = GetEquipmentArmors(notNullArmors);
                    equipmentArmors.AddRange(currentEquipmentArmors);
                }
            }

            var mergedArmors = MergeArmor(equipmentArmors);

            DefenceStats.SetArmors(mergedArmors.ToArray());
        }

        /// <summary>
        /// Навыки обороны против наступательных действий.
        /// </summary>
        public IPersonDefenceStats DefenceStats { get; set; }

        /// <inheritdoc />
        public string Key => nameof(ICombatStatsModule);

        /// <inheritdoc />
        public bool IsActive { get; set; }
    }
}