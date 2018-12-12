using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class AttackTask : OneTurnActorTaskBase
    {
        private readonly ITacticalActUsageService _actService;
        private readonly IMap _map;

        public IAttackTarget Target { get; }

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

            var mainActProcessing = true;
            var availableSlotAct = GetUsedActs();
            foreach (var currentAct in availableSlotAct)
            {
                if (mainActProcessing)
                {
                    mainActProcessing = false;

                    var targetNode = Target.Node;

                    var targetIsOnLine = MapHelper.CheckNodeAvailability(_map, Actor.Node, targetNode);
                    var isInDistance = currentAct.CheckDistance(((HexNode)Actor.Node).CubeCoords, ((HexNode)targetNode).CubeCoords);

                    var canExecute = targetIsOnLine && isInDistance;

                    if (!canExecute)
                    {
                        throw new InvalidOperationException("Задачу на атаку нельзя выполнить сквозь стены.");
                    }

                    Actor.UseAct(Target, currentAct);
                    _actService.UseOn(Actor, Target, currentAct);

                    if (currentAct.Stats.Range.Max > 1)
                    {
                        break;
                    }
                }
                else
                {
                    var targetNode = Target.Node;

                    var targetIsOnLine = MapHelper.CheckNodeAvailability(_map, Actor.Node, targetNode);
                    var isInDistance = currentAct.CheckDistance(((HexNode)Actor.Node).CubeCoords, ((HexNode)targetNode).CubeCoords);

                    var canExecute = targetIsOnLine && isInDistance;

                    if (!canExecute)
                    {
                        throw new InvalidOperationException("Задачу на атаку нельзя выполнить сквозь стены.");
                    }

                    Actor.UseAct(Target, currentAct);
                    _actService.UseOn(Actor, Target, currentAct);
                }
            }
        }

        public AttackTask(IActor actor,
            IAttackTarget target,
            ITacticalActUsageService actService,
            IMap map) :
            base(actor)
        {
            _actService = actService;
            _map = map;

            Target = target;
        }

        private IEnumerable<ITacticalAct> GetUsedActs()
        {
            var slots = Actor.Person.EquipmentCarrier.Slots;
            for (var i = 0; i < slots.Length; i++)
            {
                var slotEquipment = Actor.Person.EquipmentCarrier.Equipments[i];
                if (slotEquipment == null)
                {
                    continue;
                }

                if ((slots[i].Types & EquipmentSlotTypes.Hand) > 0)
                {
                    continue;
                }

                var equipmentActs = from act in Actor.Person.TacticalActCarrier.Acts
                                    where act.Equipment == slotEquipment
                                    select act;

                yield return equipmentActs.First();
            }
        }
    }
}