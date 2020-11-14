using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Auxiliary;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;

namespace Zilon.Core.PersonModules
{
    public sealed class HumanSurvivalModule : SurvivalModuleBase
    {
        private readonly IPersonScheme _personScheme;
        private readonly ISurvivalRandomSource _randomSource;
        private readonly IAttributesModule _attributesModule;
        private readonly IEffectsModule _effectsModule;
        private readonly IEvolutionModule _evolutionModule;
        private readonly IEquipmentModule _equipmentModule;

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        public HumanSurvivalModule([NotNull] IPersonScheme personScheme,
            [NotNull] ISurvivalRandomSource randomSource,
            [NotNull] IAttributesModule attributesModule,
            IEffectsModule effectsModule,
            IEvolutionModule evolutionModule,
            IEquipmentModule equipmentModule) : base(GetStats(personScheme, attributesModule))
        {
            _personScheme = personScheme ?? throw new ArgumentNullException(nameof(personScheme));
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
            _attributesModule = attributesModule ?? throw new ArgumentNullException(nameof(attributesModule));
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
            [NotNull] ISurvivalRandomSource randomSource,
            [NotNull] IAttributesModule attributesModule) : this(
                personScheme,
                randomSource,
                attributesModule,
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

            //if (_evolutionModule != null)
            //{ 
            //    _evolutionModule
            //}
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

        private static IEnumerable<SurvivalStat> GetStats([NotNull] IPersonScheme personScheme, [NotNull] IAttributesModule attributesModule)
        {
            if (personScheme is null)
            {
                throw new ArgumentNullException(nameof(personScheme));
            }

            if (attributesModule is null)
            {
                throw new ArgumentNullException(nameof(attributesModule));
            }

            return GetStatsIterator(personScheme, attributesModule).Where(x => x != null);
        }

        private static IEnumerable<SurvivalStat> GetStatsIterator([NotNull] IPersonScheme personScheme, IAttributesModule attributesModule)
        {
            // Устанавливаем характеристики выживания персонажа
            yield return SetHitPointsStat(personScheme, attributesModule);

            // Выставляем сытость/упоённость
            if (personScheme.SurvivalStats != null)
            {
                yield return CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Satiety,
                    PersonSurvivalStatType.Satiety);

                yield return CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Hydration,
                    PersonSurvivalStatType.Hydration);

                yield return CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Intoxication,
                    PersonSurvivalStatType.Intoxication);

                yield return CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Wound,
                    PersonSurvivalStatType.Wound);
            }

            yield return CreateUselessStat(SurvivalStatType.Breath);
            yield return CreateUselessStat(SurvivalStatType.Energy);
        }

        private static SurvivalStat CreateUselessStat(SurvivalStatType statType)
        {
            var stat = new SurvivalStat(100, 0, 100)
            {
                Type = statType,
                Rate = 1,
                DownPassRoll = 0
            };

            return stat;
        }

        private void Stat_Changed(object sender, EventArgs e)
        {
            var stat = (SurvivalStat)sender;

            if (stat.KeySegments is null)
            {
                return;
            }

            if (_effectsModule != null)
            {
                PersonEffectHelper.UpdateSurvivalEffect(
                    _effectsModule,
                    stat,
                    stat.KeySegments,
                    _randomSource,
                    PlayerEventLogService);
            }

            DoStatChanged((SurvivalStat)sender);
        }

        private static SurvivalStat CreateStatFromScheme(IPersonSurvivalStatSubScheme[] survivalStats,
            SurvivalStatType statType,
            PersonSurvivalStatType schemeStatType)
        {
            var stat = CreateStat(statType, schemeStatType, survivalStats);
            return stat ?? null;
        }

        private static SurvivalStat SetHitPointsStat(IPersonScheme personScheme, IAttributesModule attributesModule)
        {
            var constitutionHpBonus = GetConstitutionHpBonus(attributesModule);

            // В схеме храним базовые ХП.
            // Конституция добавяет или снижает ХП.
            var totalHp = personScheme.Hp + constitutionHpBonus;
            var hpStat = new HpSurvivalStat(totalHp, 0, totalHp)
            {
                Type = SurvivalStatType.Health
            };

            return hpStat;
        }

        private static int GetConstitutionHpBonus(IAttributesModule attributesModule)
        {
            var constitutionAttribute = attributesModule.GetAttribute(PersonAttributeType.Constitution);
            int constitutionHpBonus;
            if (constitutionAttribute is null)
            {
                constitutionHpBonus = 0;
            }
            else
            {
                const int CONSTITUTION_HP_INFLUENCE = 2;
                const int BASE_CONSTITUTION = 10;

                if (constitutionAttribute.Value > 10)
                {
                    constitutionHpBonus = ((int)constitutionAttribute.Value - BASE_CONSTITUTION) * CONSTITUTION_HP_INFLUENCE;
                }
                else
                {
                    constitutionHpBonus = -(BASE_CONSTITUTION - (int)constitutionAttribute.Value) * CONSTITUTION_HP_INFLUENCE;
                }
            }

            return constitutionHpBonus;
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
            var constitutionBonus = GetConstitutionHpBonus(_attributesModule);
            var totalHp = _personScheme.Hp + constitutionBonus;
            Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health)?.ChangeStatRange(0, totalHp);

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
                PerkLevel currentLevel;
                PerkLevelSubScheme currentLevelScheme;

                if (archievedPerk.Scheme.IsBuildIn)
                {
                    currentLevel = new PerkLevel(0, 0);
                }
                else
                {
                    currentLevel = archievedPerk.CurrentLevel;
                }

                if (archievedPerk.Scheme.IsBuildIn && archievedPerk.Scheme.Levels is null)
                {
                    continue;
                }

                currentLevelScheme = archievedPerk.Scheme.Levels[currentLevel.Primary];

                if (currentLevelScheme.Rules == null)
                {
                    continue;
                }

                for (var i = 0; i <= currentLevel.Sub; i++)
                {
                    foreach (var rule in currentLevelScheme.Rules)
                    {
                        bonusList = ProcessRule(bonusList, rule);
                    }
                }
            }
        }

        private List<SurvivalStatBonus> ProcessRule(List<SurvivalStatBonus> bonusList, PerkRuleSubScheme rule)
        {
            switch (rule.Type)
            {
                case PersonRuleType.Health:
                    BonusToHealth(rule.Level, PersonRuleDirection.Positive, ref bonusList);
                    break;

                case PersonRuleType.HealthIfNoBody:

                    var equipmentModule = _equipmentModule;

                    var requirementsCompleted = true;

                    if (equipmentModule != null)
                    {
                        // it is logically. If person can not have equipment, he has no body armor.

                        for (var slotIndex = 0; slotIndex < equipmentModule.Count(); slotIndex++)
                        {
                            if ((equipmentModule.Slots[slotIndex].Types & EquipmentSlotTypes.Body) > 0
                                && equipmentModule[slotIndex] != null)
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

                case PersonRuleType.HungerResistance:
                    BonusToDownPass(SurvivalStatType.Satiety, rule.Level, rule.Direction, ref bonusList);
                    break;

                case PersonRuleType.ThristResistance:
                    BonusToDownPass(SurvivalStatType.Hydration, rule.Level, rule.Direction, ref bonusList);
                    break;
            }

            return bonusList;
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
                    bonusList = ProcessRuleAndChangeBonusList(bonusList, equipmentModule, rule);
                }
            }
        }

        private List<SurvivalStatBonus> ProcessRuleAndChangeBonusList(List<SurvivalStatBonus> bonusList, IEquipmentModule equipmentModule, PersonRule rule)
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

                default:
                    throw new InvalidOperationException($"Правило {rule.Type} не обрабатывается.");
            }

            return bonusList;
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
            const SurvivalStatType HP_STAT_TYPE = SurvivalStatType.Health;
            var hpStat = Stats.SingleOrDefault(x => x.Type == HP_STAT_TYPE);
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

                    case PersonRuleLevel.None:
                        throw new InvalidOperationException("Неопределённое правило.");

                    default:
                        throw new InvalidOperationException($"Правило {level} не обрабатывается.");
                }

                if (direction == PersonRuleDirection.Negative)
                {
                    bonus *= -1;
                }

                var currentBonus = bonuses.SingleOrDefault(x => x.SurvivalStatType == HP_STAT_TYPE);
                if (currentBonus == null)
                {
                    currentBonus = new SurvivalStatBonus(HP_STAT_TYPE);
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
                    Debug.Fail("Предположительно, это ошибка.");
                    break;
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