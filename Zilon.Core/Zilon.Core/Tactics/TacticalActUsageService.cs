using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public sealed class TacticalActUsageService : ITacticalActUsageService
    {
        private readonly ITacticalActUsageRandomSource _actUsageRandomSource;
        private readonly IPerkResolver _perkResolver;
        private readonly ISectorManager _sectorManager;

        public TacticalActUsageService(ITacticalActUsageRandomSource actUsageRandomSource,
            IPerkResolver perkResolver,
            ISectorManager sectorManager)
        {
            _actUsageRandomSource = actUsageRandomSource ?? throw new ArgumentNullException(nameof(actUsageRandomSource));
            _perkResolver = perkResolver ?? throw new ArgumentNullException(nameof(perkResolver));
            _sectorManager = sectorManager ?? throw new ArgumentNullException(nameof(sectorManager));
        }

        public void UseOn(IActor actor, IAttackTarget target, UsedTacticalActs usedActs)
        {
            foreach (var act in usedActs.Primary)
            {
                if (!act.Stats.Targets.HasFlag(TacticalActTargets.Self) && actor == target)
                {
                    throw new ArgumentException("Актёр не может атаковать сам себя", nameof(target));
                }

                UseAct(actor, target, act);
            }

            // Использование дополнительных действий.
            // Используются с некоторой вероятностью.
            foreach (var act in usedActs.Secondary)
            {
                var useSuccessRoll = GetUseSuccessRoll();
                var useFactRoll = GetUseFactRoll();

                if (useFactRoll < useSuccessRoll)
                {
                    continue;
                }

                UseAct(actor, target, act);
            }
        }

        private void UseAct(IActor actor, IAttackTarget target, ITacticalAct act)
        {
            var currentCubePos = ((HexNode)actor.Node).CubeCoords;
            var targetCubePos = ((HexNode)target.Node).CubeCoords;

            var isInDistance = act.CheckDistance(currentCubePos, targetCubePos);
            if (!isInDistance)
            {
                throw new InvalidOperationException("Попытка атаковать цель, находящуюся за пределами атаки.");
            }

            var targetNode = target.Node;

            var targetIsOnLine = MapHelper.CheckNodeAvailability(_sectorManager.CurrentSector.Map,
                actor.Node,
                targetNode);

            if (!targetIsOnLine)
            {
                throw new InvalidOperationException("Задачу на атаку нельзя выполнить сквозь стены.");
            }


            actor.UseAct(target, act);

            var tacticalActRoll = GetActEfficient(act);

            // Изъятие патронов
            if (act.Constrains?.PropResourceType != null)
            {
                RemovePropResource(actor, act);
            }

            if (target is IActor targetActor)
            {
                UseOnActor(actor, targetActor, tacticalActRoll);
            }
            else
            {
                UseOnChest(target, tacticalActRoll);
            }
        }

        private static void RemovePropResource(IActor actor, ITacticalAct act)
        {
            var propResources = from prop in actor.Person.Inventory.CalcActualItems()
                                where prop is Resource
                                where prop.Scheme.Bullet?.Caliber == act.Constrains.PropResourceType
                                select prop;

            if (propResources.FirstOrDefault() is Resource propResource)
            {
                if (propResource.Count >= act.Constrains.PropResourceCount)
                {
                    var usedResource = new Resource(propResource.Scheme, act.Constrains.PropResourceCount.Value);
                    actor.Person.Inventory.Remove(usedResource);
                }
                else
                {
                    throw new InvalidOperationException($"Не хватает ресурса {propResource} для использования действия {act}.");
                }
            }
            else
            {
                throw new InvalidOperationException($"Не хватает ресурса {act.Constrains?.PropResourceType} для использования действия {act}.");
            }
        }

        private int GetUseFactRoll()
        {
            var roll = _actUsageRandomSource.RollUseSecondaryAct();
            return roll;
        }

        private int GetUseSuccessRoll()
        {
            // В будущем успех использования вторичных дейсвий будет зависить от действия, экипировки, перков.
            return 5;
        }

        /// <summary>
        /// Возвращает случайное значение эффективность действия.
        /// </summary>
        /// <param name="act"> Соверщённое действие. </param>
        /// <returns> Возвращает выпавшее значение эффективности. </returns>
        private TacticalActRoll GetActEfficient(ITacticalAct act)
        {
            var rolledEfficient = _actUsageRandomSource.RollEfficient(act.Efficient);

            var roll = new TacticalActRoll(act, rolledEfficient);

            return roll;
        }

        /// <summary>
        /// Применяет действие на предмет, на который можно подействовать (сундук/дверь).
        /// </summary>
        /// <param name="target"> Цель использования действия. </param>
        /// <param name="tacticalActRoll"> Эффективность действия. </param>
        private static void UseOnChest(IAttackTarget target, TacticalActRoll tacticalActRoll)
        {
            target.TakeDamage(tacticalActRoll.Efficient);
        }

        /// <summary>
        /// Применяет действие на актёра.
        /// </summary>
        /// <param name="actor"> Актёр, который совершил действие. </param>
        /// <param name="targetActor"> Цель использования действия. </param>
        /// <param name="tacticalActRoll"> Эффективность действия. </param>
        private void UseOnActor(IActor actor, IActor targetActor, TacticalActRoll tacticalActRoll)
        {
            var targetIsDeadLast = targetActor.Person.Survival.IsDead;

            var offenceType = tacticalActRoll.TacticalAct.Stats.Offense.Type;
            var usedDefences = GetCurrentDefences(targetActor, offenceType);

            var prefferedDefenceItem = HitHelper.CalcPreferredDefense(usedDefences);
            var successToHitRoll = HitHelper.CalcSuccessToHit(prefferedDefenceItem);
            var factToHitRoll = _actUsageRandomSource.RollToHit();

            if (factToHitRoll >= successToHitRoll)
            {
                int actEfficient = CalcEfficient(targetActor, tacticalActRoll);

                if (actEfficient <= 0)
                {
                    return;
                }

                targetActor.TakeDamage(actEfficient);

                if (!targetIsDeadLast && targetActor.Person.Survival.IsDead)
                {
                    CountTargetActorDefeat(actor, targetActor);
                }
            }
            else
            {
                if (prefferedDefenceItem != null)
                {
                    targetActor.ProcessDefence(prefferedDefenceItem,
                        successToHitRoll,
                        factToHitRoll);
                }
            }
        }

        /// <summary>
        /// Расчитывает эффективность умения с учётом поглощения броней.
        /// </summary>
        /// <param name="targetActor"> Целевой актёр. </param>
        /// <param name="tacticalActRoll"> Результат броска исходной эфективности действия. </param>
        /// <returns> Возвращает числовое значение эффективности действия. </returns>
        private int CalcEfficient(IActor targetActor, TacticalActRoll tacticalActRoll)
        {
            var actApRank = GetActApRank(tacticalActRoll.TacticalAct);
            var armorRank = GetArmorRank(targetActor, tacticalActRoll.TacticalAct);

            var actEfficientArmorBlocked = tacticalActRoll.Efficient;
            var rankDiff = actApRank - armorRank;

            if (armorRank != null && rankDiff < 10)
            {
                var factArmorSaveRoll = RollArmorSave();
                var successArmorSaveRoll = GetSuccessArmorSave(targetActor, tacticalActRoll.TacticalAct);
                if (factArmorSaveRoll >= successArmorSaveRoll)
                {
                    var armorAbsorbtion = GetArmorAbsorbtion(targetActor, tacticalActRoll.TacticalAct);
                    actEfficientArmorBlocked = AbsorbActEfficient(actEfficientArmorBlocked, armorAbsorbtion);
                }

                targetActor.ProcessArmor(armorRank.Value, successArmorSaveRoll, factArmorSaveRoll);
            }

            return actEfficientArmorBlocked;
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

            return targetActor.Person.CombatStats.DefenceStats.Defences
                            .Where(x => x.Type == defenceType || x.Type == DefenceType.DivineDefence);
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
        /// Возвращает показатель поглощения брони цели.
        /// Это величина, на которую будет снижен урон.
        /// </summary>
        /// <param name="targetActor"> Целевой актёр, для которого проверяется поглощение урона. </param>
        /// <param name="usedTacticalAct"> Действие, которое будет использовано для нанесения урона. </param>
        /// <returns> Возвращает показатель поглощения брони цели. </returns>
        private static int GetArmorAbsorbtion(IActor targetActor, ITacticalAct usedTacticalAct)
        {
            var actorArmors = targetActor.Person.CombatStats.DefenceStats.Armors;
            var actImpact = usedTacticalAct.Stats.Offense.Impact;
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
            var actorArmors = targetActor.Person.CombatStats.DefenceStats.Armors;
            var actImpact = usedTacticalAct.Stats.Offense.Impact;
            var preferredArmor = actorArmors.FirstOrDefault(x => x.Impact == actImpact);

            if (preferredArmor == null)
            {
                throw new InvalidOperationException($"Не найдена защита {actImpact}.");
            }

            var apRankDiff = usedTacticalAct.Stats.Offense.ApRank - preferredArmor.ArmorRank;

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
        /// Возвращает результат спас-броска на броню.
        /// </summary>
        /// <returns></returns>
        private int RollArmorSave()
        {
            var factRoll = _actUsageRandomSource.RollArmorSave();
            return factRoll;
        }

        /// <summary>
        /// Возвращает ранг брони цели.
        /// </summary>
        /// <param name="targetActor"> Актёр, для которого выбирается ранг брони. </param>
        /// <param name="usedTacticalAct"> Действие, от которого требуется броня. </param>
        /// <returns> Возвращает числовое значение ранга брони указанного типа. </returns>
        private static int? GetArmorRank(IActor targetActor, ITacticalAct usedTacticalAct)
        {
            var actorArmors = targetActor.Person.CombatStats.DefenceStats.Armors;
            var actImpact = usedTacticalAct.Stats.Offense.Impact;
            var preferredArmor = actorArmors.FirstOrDefault(x => x.Impact == actImpact);

            return preferredArmor?.ArmorRank;
        }

        /// <summary>
        /// Возвращает ранг пробития действия.
        /// </summary>
        /// <param name="tacticalAct"></param>
        /// <returns></returns>
        private int GetActApRank(ITacticalAct tacticalAct)
        {
            return tacticalAct.Stats.Offense.ApRank;
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

            var evolutionData = actor.Person.EvolutionData;

            var defeatProgress = new DefeatActorJobProgress(targetActor);

            _perkResolver.ApplyProgress(defeatProgress, evolutionData);
        }
    }
}
