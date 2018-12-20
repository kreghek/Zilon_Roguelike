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

            var availableSlotAct = GetUsedActs();
            var usedActs = new UsedTacticalActs(availableSlotAct.Take(1), availableSlotAct.Skip(1));
            _actService.UseOn(Actor, Target, usedActs);
        }

        public AttackTask(IActor actor,
            IAttackTarget target,
            ITacticalActUsageService actService) :
            base(actor)
        {
            _actService = actService;

            Target = target;
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
    }
}