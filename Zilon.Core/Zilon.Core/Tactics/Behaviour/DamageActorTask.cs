using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class DamageActorTask : OneTurnActorTaskBase
    {
        private readonly ISectorMap _map;

        public IAttackTarget Target { get; }

        public DamageActorTask(IActor actor,
            IAttackTarget target,
            ISectorMap map) :
            base(actor)
        {
            Target = target;
            _map = map;
        }

        protected override void ExecuteTask()
        {
            if (!Target.CanBeDamaged())
            {
                throw new InvalidOperationException("Попытка атаковать цель, которой нельзя нанести урон.");
            }

            if (Actor.Person.TacticalActCarrier == null)
            {
                throw new NotImplementedException("Не неализована возможность атаковать без навыков.");
            }

            var availableSlotActs = GetUsedActs();
            foreach (var availableAct in availableSlotActs)
            {
                if (availableAct.Stats.Effect != TacticalActEffectType.Damage)
                {
                    throw new InvalidOperationException($"Нельзя атаковать действием {availableAct}.");
                }
            }

            var targetNode = Target.Node;

            

            var targetIsOnLine = _map.TargetIsOnLine(Actor.Node, targetNode);
            if (!targetIsOnLine)
            {
                throw new InvalidOperationException("Задачу на атаку нельзя выполнить сквозь стены.");
            }

            var usedActs = new UsedTacticalActs(availableSlotActs.Take(1), availableSlotActs.Skip(1));

            foreach (var act in usedActs.Primary)
            {
                if (!act.Stats.Targets.HasFlag(TacticalActTargets.Self) && Actor == Target)
                {
                    throw new InvalidOperationException($"Актёр не может использовать действие {act} сам себя.");
                }

                UseAct(Actor, Target, act);
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

                UseAct(Actor, Target, act);
            }
        }

        private IEnumerable<ITacticalAct> GetUsedActs()
        {
            if (Actor.Person.EquipmentCarrier == null)
            {
                yield return Actor.Person.TacticalActCarrier.Acts.First();
            }
            else
            {
                var usedEquipmentActs = false;
                var slots = Actor.Person.EquipmentCarrier.Slots;
                for (var i = 0; i < slots.Length; i++)
                {
                    var slotEquipment = Actor.Person.EquipmentCarrier[i];
                    if (slotEquipment == null)
                    {
                        continue;
                    }

                    if ((slots[i].Types & EquipmentSlotTypes.Hand) == 0)
                    {
                        continue;
                    }

                    var equipmentActs = from act in Actor.Person.TacticalActCarrier.Acts
                                        where act.Equipment == slotEquipment
                                        select act;

                    var usedAct = equipmentActs.FirstOrDefault();

                    if (usedAct != null)
                    {
                        usedEquipmentActs = true;

                        yield return usedAct;
                    }
                }

                if (!usedEquipmentActs)
                {
                    yield return Actor.Person.TacticalActCarrier.Acts.First();
                }
            }
        }

        /// <summary>
        /// Наносит урон актёру.
        /// </summary>
        /// <param name="actor"> Актёр, который совершил действие. </param>
        /// <param name="targetActor"> Цель использования действия. </param>
        private void DamageActor(IActor actor, IActor targetActor)
        {
            var targetIsDeadLast = targetActor.Person.Survival.IsDead;

            var offenceType = tacticalActRoll.TacticalAct.Stats.Offence.Type;
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

                if (EquipmentDurableService != null && targetActor.Person.EquipmentCarrier != null)
                {
                    var damagedEquipment = GetDamagedEquipment(targetActor);

                    // может быть null, если нет брони вообще
                    if (damagedEquipment != null)
                    {
                        EquipmentDurableService.UpdateByUse(damagedEquipment, targetActor.Person);
                    }
                }

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

        private void UseAct(IActor actor, IAttackTarget target, ITacticalAct act)
        {
            bool isInDistance;
            isInDistance = CheckInDistance(actor, target, act);

            if (!isInDistance)
            {
                throw new InvalidOperationException("Попытка атаковать цель, находящуюся за пределами атаки.");
            }


            actor.UseAct(target, act);

            var tacticalActRoll = GetActEfficient(act);

            // Изъятие патронов
            if (act.Constrains?.PropResourceType != null)
            {
                RemovePropResource(actor, act);
            }


            UseOnActor(actor, targetActor, tacticalActRoll);

            if (act.Equipment != null)
            {
                EquipmentDurableService?.UpdateByUse(act.Equipment, actor.Person);
            }
        }

        private static bool CheckInDistance(IActor actor, IAttackTarget target, ITacticalAct act)
        {
            var useOnSelf = actor == target;
            var actCanBeUsedOnSelf = act.Stats.Targets.HasFlag(TacticalActTargets.Self);

            if (useOnSelf && actCanBeUsedOnSelf)
            {
                return true;
            }

            var currentHexNode = (HexNode)actor.Node;
            var targetHexNode = (HexNode)target.Node;

            var currentCubePos = currentHexNode.CubeCoords;
            var targetCubePos = targetHexNode.CubeCoords;

            var isInDistance = act.CheckDistance(currentCubePos, targetCubePos);

            return isInDistance;
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
                    HealActor(actor, targetActor, tacticalActRoll);
                    break;

                default:
                    throw new ArgumentException(string.Format("Не определённый эффект {0} действия {1}.",
                        tacticalActRoll.TacticalAct.Stats.Effect,
                        tacticalActRoll.TacticalAct));
            }
        }

        private int GetUseSuccessRoll()
        {
            // В будущем успех использования вторичных дейсвий будет зависить от действия, экипировки, перков.
            return 5;
        }

        private int GetUseFactRoll()
        {
            var roll = _actUsageRandomSource.RollUseSecondaryAct();
            return roll;
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
    }
}