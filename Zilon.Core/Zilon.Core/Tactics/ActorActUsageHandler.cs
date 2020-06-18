using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Diseases;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics.ActorInteractionEvents;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Обработчик действий, нацеленных на актёра.
    /// </summary>
    public sealed class ActorActUsageHandler : IActUsageHandler
    {
        private readonly IPerkResolver _perkResolver;
        private readonly ITacticalActUsageRandomSource _actUsageRandomSource;

        /// <summary>
        /// Шина событий возаимодействия актёров.
        /// </summary>
        public IActorInteractionBus ActorInteractionBus { get; set; }

        /// <summary>
        /// Сервис для работы с достижениями персонажа.
        /// </summary>
        public IScoreManager ScoreManager { get; set; }

        /// <summary>
        /// Сервис для логирования событий, связанных с персонажем игрока.
        /// </summary>
        public IPlayerEventLogService PlayerEventLogService { get; set; }

        /// <summary>Сервис для работы с прочностью экипировки.</summary>
        public IEquipmentDurableService EquipmentDurableService { get; set; }

        public ActorActUsageHandler(IPerkResolver perkResolver, ITacticalActUsageRandomSource actUsageRandomSource)
        {
            _perkResolver = perkResolver ?? throw new ArgumentNullException(nameof(perkResolver));
            _actUsageRandomSource = actUsageRandomSource ?? throw new ArgumentNullException(nameof(actUsageRandomSource));
        }

        /// <inheritdoc/>
        public Type TargetType { get => typeof(IActor); }

        /// <inheritdoc/>
        public void ProcessActUsage(IActor actor, IAttackTarget target, TacticalActRoll tacticalActRoll)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (tacticalActRoll is null)
            {
                throw new ArgumentNullException(nameof(tacticalActRoll));
            }

            UseOnActor(actor, target as IActor, tacticalActRoll);
        }

        /// <summary>
        /// Применяет действие на актёра.
        /// </summary>
        /// <param name="actor"> Актёр, который совершил действие. </param>
        /// <param name="targetActor"> Цель использования действия. </param>
        /// <param name="tacticalActRoll"> Эффективность действия. </param>
        private void UseOnActor(IActor actor, IActor targetActor, TacticalActRoll tacticalActRoll)
        {
            switch (tacticalActRoll.TacticalAct.Stats.Effect)
            {
                case TacticalActEffectType.Damage:
                    DamageActor(actor, targetActor, tacticalActRoll);
                    break;

                case TacticalActEffectType.Heal:
                    HealActor(targetActor, tacticalActRoll);
                    break;

                default:
                    var effect = tacticalActRoll.TacticalAct.Stats.Effect;
                    var tacticalAct = tacticalActRoll.TacticalAct;
                    throw new ArgumentException($"Не определённый эффект {effect} действия {tacticalAct}.");
            }
        }

        /// <summary>
        /// Производит попытку нанесения урона целевову актёру с учётом обороны и брони.
        /// </summary>
        /// <param name="actor"> Актёр, который совершил действие. </param>
        /// <param name="targetActor"> Цель использования действия. </param>
        /// <param name="tacticalActRoll"> Эффективность действия. </param>
        private void DamageActor(IActor actor, IActor targetActor, TacticalActRoll tacticalActRoll)
        {
            var targetIsDeadLast = targetActor.Person.CheckIsDead();

            var offenceType = tacticalActRoll.TacticalAct.Stats.Offence.Type;
            var usedDefences = GetCurrentDefences(targetActor, offenceType);

            var prefferedDefenceItem = HitHelper.CalcPreferredDefense(usedDefences);
            var successToHitRoll = HitHelper.CalcSuccessToHit(prefferedDefenceItem);
            var factToHitRoll = _actUsageRandomSource.RollToHit(tacticalActRoll.TacticalAct.ToHit);

            if (factToHitRoll >= successToHitRoll)
            {
                var damageEfficientCalcResult = CalcEfficient(targetActor, tacticalActRoll);
                var actEfficient = damageEfficientCalcResult.ResultEfficient;

                ProcessSuccessfulAttackEvent(
                    actor,
                    targetActor,
                    damageEfficientCalcResult,
                    successToHitRoll,
                    factToHitRoll);

                if (actEfficient <= 0)
                {
                    return;
                }

                targetActor.TakeDamage(actEfficient);

                CountTargetActorAttack(actor, targetActor, tacticalActRoll.TacticalAct);

                ProcessDiseaseInfection(actor, targetActor);

                LogDamagePlayerEvent(actor, targetActor, tacticalActRoll.TacticalAct);

                ReduceTargetEquipmentDurability(targetActor);

                if (!targetIsDeadLast && targetActor.Person.CheckIsDead())
                {
                    CountTargetActorDefeat(actor, targetActor);
                }
            }
            else
            {
                if (prefferedDefenceItem != null)
                {
                    // Это промах, потому что целевой актёр увернулся.
                    ProcessAttackDodgeEvent(actor,
                        targetActor,
                        prefferedDefenceItem,
                        successToHitRoll,
                        factToHitRoll);
                }
                else
                {
                    // Это промах чистой воды.
                    ProcessPureMissEvent(actor,
                        targetActor,
                        successToHitRoll,
                        factToHitRoll);
                }
            }
        }

        private void ReduceTargetEquipmentDurability(IActor targetActor)
        {
            if (EquipmentDurableService is null || targetActor.Person.GetModuleSafe<IEquipmentModule>() is null)
            {
                return;
            }

            var damagedEquipment = GetDamagedEquipment(targetActor);

            // может быть null, если нет брони вообще
            if (damagedEquipment is null)
            {
                return;
            }

            EquipmentDurableService.UpdateByUse(damagedEquipment, targetActor.Person);
        }

        /// <summary>
        /// Лечит актёра.
        /// </summary>
        /// <param name="targetActor"> Цель использования действия. </param>
        /// <param name="tacticalActRoll"> Эффективность действия. </param>
        private static void HealActor(IActor targetActor, TacticalActRoll tacticalActRoll)
        {
            targetActor.Person.GetModuleSafe<ISurvivalModule>()?.RestoreStat(SurvivalStatType.Health, tacticalActRoll.Efficient);
        }

        /// <summary>
        /// Расчитывает эффективность умения с учётом поглощения броней.
        /// </summary>
        /// <param name="targetActor"> Целевой актёр. </param>
        /// <param name="tacticalActRoll"> Результат броска исходной эфективности действия. </param>
        /// <returns> Возвращает числовое значение эффективности действия. </returns>
        private DamageEfficientCalc CalcEfficient(IActor targetActor, TacticalActRoll tacticalActRoll)
        {
            var damageEfficientCalcResult = new DamageEfficientCalc();

            var actApRank = GetActApRank(tacticalActRoll.TacticalAct);
            damageEfficientCalcResult.ActApRank = actApRank;
            var armorRank = GetArmorRank(targetActor, tacticalActRoll.TacticalAct);
            damageEfficientCalcResult.ArmorRank = armorRank;

            var actEfficientArmorBlocked = tacticalActRoll.Efficient;
            var rankDiff = actApRank - armorRank;

            if (armorRank != null && rankDiff < 10)
            {
                var factArmorSaveRoll = RollArmorSave();
                damageEfficientCalcResult.FactArmorSaveRoll = factArmorSaveRoll;
                var successArmorSaveRoll = GetSuccessArmorSave(targetActor, tacticalActRoll.TacticalAct);
                damageEfficientCalcResult.SuccessArmorSaveRoll = successArmorSaveRoll;
                if (factArmorSaveRoll >= successArmorSaveRoll)
                {
                    damageEfficientCalcResult.TargetSuccessfullUsedArmor = true;
                    var armorAbsorbtion = GetArmorAbsorbtion(targetActor, tacticalActRoll.TacticalAct);
                    damageEfficientCalcResult.ArmorAbsorbtion = armorAbsorbtion;
                    actEfficientArmorBlocked = AbsorbActEfficient(actEfficientArmorBlocked, armorAbsorbtion);
                }
            }

            damageEfficientCalcResult.ActEfficientArmorBlocked = actEfficientArmorBlocked;

            return damageEfficientCalcResult;
        }

        /// <summary>
        /// Возвращает ранг пробития действия.
        /// </summary>
        /// <param name="tacticalAct"></param>
        /// <returns></returns>
        private static int GetActApRank(ITacticalAct tacticalAct)
        {
            return tacticalAct.Stats.Offence.ApRank;
        }

        /// <summary>
        /// Возвращает ранг брони цели.
        /// </summary>
        /// <param name="targetActor"> Актёр, для которого выбирается ранг брони. </param>
        /// <param name="usedTacticalAct"> Действие, от которого требуется броня. </param>
        /// <returns> Возвращает числовое значение ранга брони указанного типа. </returns>
        private static int? GetArmorRank(IActor targetActor, ITacticalAct usedTacticalAct)
        {
            var actorArmors = targetActor.Person.GetModule<ICombatStatsModule>().DefenceStats.Armors;
            var actImpact = usedTacticalAct.Stats.Offence.Impact;
            var preferredArmor = actorArmors.FirstOrDefault(x => x.Impact == actImpact);

            return preferredArmor?.ArmorRank;
        }

        /// <summary>
        /// Возвращает результат спас-броска на броню.
        /// </summary>
        /// <returns></returns>
        private int RollArmorSave()
        {
            var factRoll = _actUsageRandomSource.RollArmorSave();
            return factRoll;
        }

        /// <summary>
        /// Рассчитывает успешный спас-бросок за броню цели.
        /// </summary>
        /// <param name="targetActor"> Целевой актёр, для которого проверяется спас-бросок за броню. </param>
        /// <param name="usedTacticalAct"> Действие, для которого будет проверятся спас-бросок за броню. </param>
        /// <returns> Величина успешного спас-броска за броню. </returns>
        /// <remarks>
        /// При равных рангах броня пробивается на 4+.
        /// За каждые два ранга превосходства действия над бронёй - увеличение на 1.
        /// </remarks>
        private static int GetSuccessArmorSave(IActor targetActor, ITacticalAct usedTacticalAct)
        {
            var actorArmors = targetActor.Person.GetModule<ICombatStatsModule>().DefenceStats.Armors;
            var actImpact = usedTacticalAct.Stats.Offence.Impact;
            var preferredArmor = actorArmors.FirstOrDefault(x => x.Impact == actImpact);

            if (preferredArmor == null)
            {
                throw new InvalidOperationException($"Не найдена защита {actImpact}.");
            }

            var apRankDiff = usedTacticalAct.Stats.Offence.ApRank - preferredArmor.ArmorRank;

            switch (apRankDiff)
            {
                case 1:
                case 0:
                case -1:
                    return 4;

                case 2:
                case 3:
                    return 5;

                case 4:
                case 5:
                case 6:
                    return 6;

                case -2:
                case -3:
                    return 3;

                case -4:
                case -5:
                case -6:
                    return 2;

                default:
                    if (apRankDiff >= 7)
                    {
                        return 7;
                    }
                    else if (apRankDiff <= -7)
                    {
                        return 1;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
            }
        }

        /// <summary>
        /// Возвращает показатель поглощения брони цели.
        /// Это величина, на которую будет снижен урон.
        /// </summary>
        /// <param name="targetActor"> Целевой актёр, для которого проверяется поглощение урона. </param>
        /// <param name="usedTacticalAct"> Действие, которое будет использовано для нанесения урона. </param>
        /// <returns> Возвращает показатель поглощения брони цели. </returns>
        private static int GetArmorAbsorbtion(IActor targetActor, ITacticalAct usedTacticalAct)
        {
            var actorArmors = targetActor.Person.GetModule<ICombatStatsModule>().DefenceStats.Armors;
            var actImpact = usedTacticalAct.Stats.Offence.Impact;
            var preferredArmor = actorArmors.FirstOrDefault(x => x.Impact == actImpact);

            if (preferredArmor == null)
            {
                return 0;
            }

            switch (preferredArmor.AbsorbtionLevel)
            {
                case PersonRuleLevel.None:
                    return 0;

                case PersonRuleLevel.Lesser:
                    return 1;

                case PersonRuleLevel.Normal:
                    return 2;

                case PersonRuleLevel.Grand:
                    return 5;

                case PersonRuleLevel.Absolute:
                    return 10;

                default:
                    throw new InvalidOperationException($"Неизвестный уровень поглощения брони {preferredArmor.AbsorbtionLevel}.");
            }
        }

        /// <summary>
        /// Расчёт эффективности умения с учётом поглащения бронёй.
        /// </summary>
        /// <param name="efficient"> Эффективность умения. </param>
        /// <param name="armorAbsorbtion"> Числовое значение поглощения брони. </param>
        /// <returns> Возвращает поглощённое значение эффективности. Эффективность не может быть меньше нуля при поглощении. </returns>
        private static int AbsorbActEfficient(int efficient, int armorAbsorbtion)
        {
            efficient -= armorAbsorbtion;

            if (efficient < 0)
            {
                efficient = 0;
            }

            return efficient;
        }

        /// <summary>
        /// Извлечение всех оборон актёра, способных противостоять указанному типу урона.
        /// Включая DivineDefence, противодействующий всем типам урона.
        /// </summary>
        /// <param name="targetActor"> Целевой актёр. </param>
        /// <param name="offenceType"> Тип урона. </param>
        /// <returns> Возвращает набор оборон. </returns>
        private static IEnumerable<PersonDefenceItem> GetCurrentDefences(IActor targetActor, OffenseType offenceType)
        {
            var defenceType = HitHelper.GetDefence(offenceType);

            return targetActor.Person.GetModule<ICombatStatsModule>().DefenceStats.Defences
                            .Where(x => x.Type == defenceType || x.Type == DefenceType.DivineDefence);
        }

        private void ProcessSuccessfulAttackEvent(
            IActor actor,
            IActor targetActor,
            DamageEfficientCalc damageEfficientCalcResult,
            int successToHitRoll,
            int factToHitRoll)
        {
            if (ActorInteractionBus == null)
            {
                return;
            }

            var damageEvent = new DamageActorInteractionEvent(actor, targetActor, damageEfficientCalcResult)
            {
                SuccessToHitRoll = successToHitRoll,
                FactToHitRoll = factToHitRoll
            };
            ActorInteractionBus.PushEvent(damageEvent);
        }

        private void CountTargetActorAttack(IActor actor, IActor targetActor, ITacticalAct tacticalAct)
        {
            if (actor.Person is MonsterPerson)
            {
                // Монстры не могут прокачиваться.
                return;
            }

            if (actor.Person == null)
            {
                // Это может происходить в тестах,
                // если в моках не определили персонажа.
                //TODO Поискать решение, как всегда быть уверенным, что персонаж указан в боевых условиях, и может быть null в тестах.
                //TODO Эта же проверка нужна в CountActorDefeat (учёт убиства актёра).
                return;
            }

            var evolutionData = actor.Person.GetModuleSafe<IEvolutionModule>();

            //TODO Такую же проверку добавить в CountActorDefeat (учёт убиства актёра).
            if (evolutionData is null)
            {
                return;
            }

            var progress = new AttackActorJobProgress(targetActor, tacticalAct);

            _perkResolver.ApplyProgress(progress, evolutionData);
        }

        /// <summary>
        /// Обработать инфицирование болезью.
        /// </summary>
        /// <param name="sourceActor"> Актёр-источник заражения. </param>
        /// <param name="targetActor"> Актёр-цель заражения. </param>
        private void ProcessDiseaseInfection(IActor sourceActor, IActor targetActor)
        {
            if (sourceActor.Person?.GetModuleSafe<IDiseaseModule>() is null)
            {
                return;
            }

            if (targetActor.Person?.GetModuleSafe<IDiseaseModule>() is null)
            {
                return;
            }

            var currentDiseases = sourceActor.Person.GetModule<IDiseaseModule>().Diseases;

            foreach (var diseaseProcess in currentDiseases)
            {
                targetActor.Person.GetModule<IDiseaseModule>().Infect(diseaseProcess.Disease);
                CountInfectionInScore(targetActor, diseaseProcess.Disease);
            }
        }

        private void CountInfectionInScore(IActor targetActor, IDisease disease)
        {
            if (targetActor is MonsterPerson)
            {
                // Для монстров не считаем достижения.
                return;
            }

            // Сервис подсчёта очков - необязательная зависимость.
            if (ScoreManager is null)
            {
                return;
            }

            // Каждую болезнь фиксируем только один раз
            if (!ScoreManager.Scores.Diseases.Any(x => x == disease))
            {
                ScoreManager.Scores.Diseases.Add(disease);
            }
        }

        private void LogDamagePlayerEvent(IActor actor, IActor targetActor, ITacticalAct tacticalAct)
        {
            // Сервис логирование - необязательная зависимость.
            // Если он не задан, то не выполняем логирование.
            if (PlayerEventLogService is null)
            {
                return;
            }

            // Логируем только урон по персонажу игрока.
            if (targetActor.Person != PlayerEventLogService.Player.MainPerson)
            {
                return;
            }

            var damageEvent = new PlayerDamagedEvent(tacticalAct, actor);
            PlayerEventLogService.Log(damageEvent);
        }

        private Equipment GetDamagedEquipment(IActor targetActor)
        {
            if (targetActor.Person.GetModuleSafe<IEquipmentModule>() is null)
            {
                throw new ArgumentException("Передан персонаж, который не может носить экипировку.");
            }

            var armorEquipments = new List<Equipment>();
            foreach (var currentEquipment in targetActor.Person.GetModule<IEquipmentModule>())
            {
                if (currentEquipment == null)
                {
                    continue;
                }

                if (currentEquipment.Scheme.Equip?.Armors != null)
                {
                    armorEquipments.Add(currentEquipment);
                }
            }

            var rolledDamagedEquipment = _actUsageRandomSource.RollDamagedEquipment(armorEquipments);

            return rolledDamagedEquipment;
        }

        /// <summary>
        /// Расчитывает убийство целевого актёра.
        /// </summary>
        /// <param name="actor"> Актёр, который совершил действие. </param>
        /// <param name="targetActor"> Цель использования действия. </param>
        private void CountTargetActorDefeat(IActor actor, IActor targetActor)
        {
            if (actor.Person is MonsterPerson)
            {
                // Монстры не могут прокачиваться.
                return;
            }

            var evolutionData = actor.Person.GetModule<IEvolutionModule>();

            var defeatProgress = new DefeatActorJobProgress(targetActor);

            _perkResolver.ApplyProgress(defeatProgress, evolutionData);
        }

        private void ProcessAttackDodgeEvent(
            IActor actor,
            IActor targetActor,
            PersonDefenceItem personDefenceItem,
            int successToHitRoll,
            int factToHitRoll)
        {
            if (ActorInteractionBus == null)
            {
                return;
            }

            var interactEvent = new DodgeActorInteractionEvent(actor, targetActor, personDefenceItem)
            {
                SuccessToHitRoll = successToHitRoll,
                FactToHitRoll = factToHitRoll
            };

            ActorInteractionBus.PushEvent(interactEvent);
        }

        private void ProcessPureMissEvent(IActor actor, IActor targetActor, int successToHitRoll, int factToHitRoll)
        {
            if (ActorInteractionBus == null)
            {
                return;
            }

            var damageEvent = new PureMissActorInteractionEvent(actor, targetActor)
            {
                SuccessToHitRoll = successToHitRoll,
                FactToHitRoll = factToHitRoll
            };

            ActorInteractionBus.PushEvent(damageEvent);
        }
    }
}
