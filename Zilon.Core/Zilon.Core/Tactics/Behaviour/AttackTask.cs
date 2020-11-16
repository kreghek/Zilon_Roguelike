using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    public class AttackTask : OneTurnActorTaskBase
    {
        private readonly ITacticalActUsageService _actService;

        public AttackTask(
            IActor actor,
            IActorTaskContext context,
            IAttackTarget target,
            ITacticalAct tacticalAct,
            ITacticalActUsageService actService) :
            base(actor, context)
        {
            _actService = actService;

            Target = target;
            TacticalAct = tacticalAct;
        }

        public ITacticalAct TacticalAct { get; }

        public IAttackTarget Target { get; }

        protected override void ExecuteTask()
        {
            if (Actor.Person.GetModuleSafe<ICombatActModule>() is null)
            {
                throw new NotImplementedException("Не неализована возможность атаковать без навыков.");
            }

            if (!Target.CanBeDamaged())
            {
                // Эта ситуация может произойти, когда:
                // 1. Текущий актёр начал выполнять задачу.
                // 2. Цель убили/умерла сама.
                // 3. Наступил момент, когда задача текущего актёра должна выполниться.

                // Эту првоерку нужно проводить выше, когда пользователю сообщаяется возможность
                // выполнить эту задачу до её начала.
                return;
            }

            var availableSlotAct = GetUsedActs();
            var usedActs = new UsedTacticalActs(new[]
            {
                TacticalAct
            }, availableSlotAct.Skip(1));
            _actService.UseOn(Actor, Target, usedActs, Context.Sector);
        }

        private IEnumerable<ITacticalAct> GetUsedActs()
        {
            if (Actor.Person.GetModuleSafe<IEquipmentModule>() == null)
            {
                yield return Actor.Person.GetModule<ICombatActModule>()
                                  .CalcCombatActs()
                                  .First();
            }
            else
            {
                var usedEquipmentActs = false;
                var slots = Actor.Person.GetModule<IEquipmentModule>()
                                 .Slots;
                for (var i = 0; i < slots.Length; i++)
                {
                    var slotEquipment = Actor.Person.GetModule<IEquipmentModule>()[i];
                    if (slotEquipment == null)
                    {
                        continue;
                    }

                    if ((slots[i]
                        .Types & EquipmentSlotTypes.Hand) == 0)
                    {
                        continue;
                    }

                    var equipmentActs = from act in Actor.Person.GetModule<ICombatActModule>()
                                                         .CalcCombatActs()
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
                    yield return Actor.Person.GetModule<ICombatActModule>()
                                      .CalcCombatActs()
                                      .First();
                }
            }
        }
    }
}