using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    public sealed class HumanSurvivalModule : SurvivalModuleBase
    {
        private readonly IPersonScheme _personScheme;
        private readonly ISurvivalRandomSource _randomSource;
        private readonly IEffectsModule _effectsModule;
        private readonly IEvolutionModule _evolutionModule;
        private readonly IEquipmentModule _equipmentModule;

        public HumanSurvivalModule([NotNull] IPersonScheme personScheme,
            [NotNull] ISurvivalRandomSource randomSource,
            IEffectsModule effectsModule,
            IEvolutionModule evolutionModule,
            IEquipmentModule equipmentModule) : base(GetStats(personScheme))
        {
            _personScheme = personScheme ?? throw new ArgumentNullException(nameof(personScheme));
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
            _effectsModule = effectsModule;
            _evolutionModule = evolutionModule;
            _equipmentModule = equipmentModule;

            RegisterModuleEventHandlers();

            foreach (var stat in Stats)
            {
                stat.Changed += Stat_Changed;
            }

            CalcSurvivalStats();
        }

        public HumanSurvivalModule([NotNull] IPersonScheme personScheme,
            [NotNull] ISurvivalRandomSource randomSource) : this(
                personScheme,
                randomSource,
                effectsModule: null,
                evolutionModule: null,
                equipmentModule: null)
        {
        }

        public HumanSurvivalModule([NotNull] IEnumerable<SurvivalStat> personStats,
            [NotNull] ISurvivalRandomSource randomSource) : base(
                personStats)
        {
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
        }

        private void RegisterModuleEventHandlers()
        {
            if (_equipmentModule != null)
            {
                _equipmentModule.EquipmentChanged += EquipmentModule_EquipmentChanged;
            }

            if (_effectsModule != null)
            {
                _effectsModule.Added += Effects_CollectionChanged;
                _effectsModule.Removed += Effects_CollectionChanged;
                _effectsModule.Changed += Effects_CollectionChanged;
            }
        }

        private void Effects_CollectionChanged(object sender, EffectEventArgs e)
        {
            CalcSurvivalStats();
        }

        private void EquipmentModule_EquipmentChanged(object sender, EquipmentChangedEventArgs e)
        {
            CalcSurvivalStats();
        }

        private void CalcSurvivalStats()
        {
            // Расчёт бонусов вынести в отдельный сервис, который покрыть модульными тестами
            // На вход принимает SurvivalData. SurvivalData дожен содержать метод увеличение диапазона характеристики.
            ResetStats();

            var bonusList = new List<SurvivalStatBonus>();
            FillSurvivalBonusesFromEquipments(ref bonusList);
            FillSurvivalBonusesFromPerks(ref bonusList);
            FillSurvivalBonusesFromEffects(ref bonusList);

            ApplySurvivalBonuses(bonusList);
        }

        private static IEnumerable<SurvivalStat> GetStats([NotNull] IPersonScheme personScheme)
        {
            if (personScheme is null)
            {
                throw new ArgumentNullException(nameof(personScheme));
            }
            // Устанавливаем характеристики выживания персонажа
            var statList = new List<SurvivalStat>();
            SetHitPointsStat(personScheme, statList);

            // Выставляем сытость/упоённость
            if (personScheme.SurvivalStats != null)
            {
                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Satiety,
                    PersonSurvivalStatType.Satiety,
                    statList);

                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Hydration,
                    PersonSurvivalStatType.Hydration,
                    statList);

                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Intoxication,
                    PersonSurvivalStatType.Intoxication,
                    statList);

                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Wound,
                    PersonSurvivalStatType.Wound,
                    statList);
            }

            CreateUselessStat(SurvivalStatType.Breath, statList);
            CreateUselessStat(SurvivalStatType.Energy, statList);

            return statList.ToArray();
        }

        private static void CreateUselessStat(SurvivalStatType statType, List<SurvivalStat> statList)
        {
            var stat = new SurvivalStat(100, 0, 100)
            {
                Type = statType,
                Rate = 1,
                DownPassRoll = 0
            };

            statList.Add(stat);
        }

        private void Stat_Changed(object sender, EventArgs e)
        {
            DoStatChanged((SurvivalStat)sender);
        }

        private static void CreateStatFromScheme(IPersonSurvivalStatSubScheme[] survivalStats,
            SurvivalStatType statType,
            PersonSurvivalStatType schemeStatType,
            List<SurvivalStat> statList)
        {
            var stat = CreateStat(statType, schemeStatType, survivalStats);
            if (stat != null)
            {
                statList.Add(stat);
            }
        }

        private static void SetHitPointsStat(IPersonScheme personScheme, IList<SurvivalStat> statList)
        {
            var hpStat = new HpSurvivalStat(personScheme.Hp, 0, personScheme.Hp)
            {
                Type = SurvivalStatType.Health
            };

            statList.Add(hpStat);
        }

        /// <summary>Обновление состояния данных о выживании.</summary>
        public override void Update()
        {
            foreach (var stat in Stats)
            {
                if (stat.Rate == 0)
                {
                    continue;
                }

                var roll = _randomSource.RollSurvival(stat);
                var statDownRoll = GetStatDownRoll(stat);

                if (roll < statDownRoll)
                {
                    ChangeStatInner(stat, -stat.Rate);
                }
            }
        }

        private static SurvivalStat CreateStat(
            SurvivalStatType type,
            PersonSurvivalStatType schemeStatType,
            IPersonSurvivalStatSubScheme[] survivalStats)
        {
            var statScheme = survivalStats.SingleOrDefault(x => x.Type == schemeStatType);
            if (statScheme is null)
            {
                return null;
            }

            var keySegmentList = new List<SurvivalStatKeySegment>();
            if (statScheme.KeyPoints != null)
            {
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Max, PersonSurvivalStatKeypointLevel.Max, statScheme.KeyPoints, keySegmentList);
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Strong, PersonSurvivalStatKeypointLevel.Strong, statScheme.KeyPoints, keySegmentList);
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Lesser, PersonSurvivalStatKeypointLevel.Lesser, statScheme.KeyPoints, keySegmentList);
            }

            var stat = new SurvivalStat(statScheme.StartValue, statScheme.MinValue, statScheme.MaxValue)
            {
                Type = type,
                Rate = 1,
                KeySegments = keySegmentList.ToArray(),
                DownPassRoll = statScheme.DownPassRoll.GetValueOrDefault(SurvivalStat.DEFAULT_DOWN_PASS_VALUE)
            };

            return stat;
        }

        private static void AddKeyPointFromScheme(
            SurvivalStatHazardLevel segmentLevel,
            PersonSurvivalStatKeypointLevel schemeSegmentLevel,
            IPersonSurvivalStatKeySegmentSubScheme[] keyPoints,
            List<SurvivalStatKeySegment> keyPointList)
        {
            var schemeKeySegment = GetKeyPointSchemeValue(schemeSegmentLevel, keyPoints);
            if (schemeKeySegment == null)
            {
                return;
            }

            var keySegment = new SurvivalStatKeySegment(schemeKeySegment.Start, schemeKeySegment.End, segmentLevel);
            keyPointList.Add(keySegment);
        }

        private void DoStatChanged(SurvivalStat stat)
        {
            var args = new SurvivalStatChangedEventArgs(stat);
            InvokeStatChangedEvent(this, args);
        }

        private static int GetStatDownRoll(SurvivalStat stat)
        {
            return stat.DownPassRoll;
        }

        /// <summary>Сброс всех характеристик к первоначальному состоянию.</summary>
        public override void ResetStats()
        {
            Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health)?.ChangeStatRange(0, _personScheme.Hp);

            foreach (var stat in Stats)
            {
                stat.DownPassRoll = SurvivalStat.DEFAULT_DOWN_PASS_VALUE;
            }
        }

        private static IPersonSurvivalStatKeySegmentSubScheme GetKeyPointSchemeValue(
            PersonSurvivalStatKeypointLevel level,
            IPersonSurvivalStatKeySegmentSubScheme[] keyPoints)
        {
            return keyPoints.SingleOrDefault(x => x.Level == level);
        }


        private void FillSurvivalBonusesFromEffects([NotNull, ItemNotNull] ref List<SurvivalStatBonus> bonusList)
        {
            if (_effectsModule is null)
            {
                return;
            }

            foreach (var effect in _effectsModule.Items)
            {
                switch (effect)
                {
                    case DiseaseSymptomEffect diseaseSymptomEffect:

                        switch (diseaseSymptomEffect.Symptom.Rule)
                        {
                            case Diseases.DiseaseSymptomType.HealthLimit:
                                BonusToHealth(PersonRuleLevel.Lesser, PersonRuleDirection.Negative, ref bonusList);
                                break;

                            case Diseases.DiseaseSymptomType.HungerSpeed:
                                BonusToDownPass(SurvivalStatType.Satiety, PersonRuleLevel.Lesser, PersonRuleDirection.Negative, ref bonusList);
                                break;

                            case Diseases.DiseaseSymptomType.ThirstSpeed:
                                BonusToDownPass(SurvivalStatType.Hydration, PersonRuleLevel.Lesser, PersonRuleDirection.Negative, ref bonusList);
                                break;

                            case Diseases.DiseaseSymptomType.BreathDownSpeed:
                                BonusToDownPass(SurvivalStatType.Breath, PersonRuleLevel.Lesser, PersonRuleDirection.Negative, ref bonusList);
                                break;

                            case Diseases.DiseaseSymptomType.EnegryDownSpeed:
                                BonusToDownPass(SurvivalStatType.Energy, PersonRuleLevel.Lesser, PersonRuleDirection.Negative, ref bonusList);
                                break;

                            case Diseases.DiseaseSymptomType.Undefined:
                            default:
                                throw new InvalidOperationException($"Неизвестное правило эффекта {diseaseSymptomEffect.Symptom.Rule}");
                        }

                        break;

                    case SurvivalStatHazardEffect _:
                        // Эти эффекты пока не влияют на статы выживания.
                        // Но case- блок должен быть, иначе будет ошибка.
                        break;

                    default:
                        throw new InvalidOperationException($"Неизвестный тип эффекта {effect.GetType()}");
                }
            }
        }

        private void FillSurvivalBonusesFromPerks([NotNull, ItemNotNull] ref List<SurvivalStatBonus> bonusList)
        {
            if (_evolutionModule is null)
            {
                return;
            }

            var archievedPerks = _evolutionModule.GetArchievedPerks();
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
                var stat = Stats.SingleOrDefault(x => x.Type == bonus.SurvivalStatType);
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
            if (_equipmentModule is null)
            {
                return;
            }

            var equipmentModule = _equipmentModule;

            for (var i = 0; i < equipmentModule.Count(); i++)
            {
                var equipment = equipmentModule[i];
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

                            for (var slotIndex = 0; slotIndex < equipmentModule.Count(); slotIndex++)
                            {
                                if ((equipmentModule.Slots[slotIndex].Types & EquipmentSlotTypes.Body) > 0
                                    && equipmentModule[slotIndex] != null)
                                {
                                    requirementsCompleted = false;
                                    break;
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

                        case EquipCommonRuleType.ThristResistance:
                            BonusToDownPass(SurvivalStatType.Hydration, rule.Level, rule.Direction, ref bonusList);
                            break;
                    }
                }
            }
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
            var survivalModule = this;
            const SurvivalStatType hpStatType = SurvivalStatType.Health;
            var hpStat = survivalModule.Stats.SingleOrDefault(x => x.Type == hpStatType);
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

        private static void BonusToDownPass(
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
    }
}
