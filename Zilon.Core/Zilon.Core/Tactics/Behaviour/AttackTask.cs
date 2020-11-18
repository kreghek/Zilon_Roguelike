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

        public AttackTask(IActor actor,
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

            var availableSlotAct = GetSecondaryUsedActs();
            var usedActs = new UsedTacticalActs(new[] { TacticalAct }, availableSlotAct.Where(x => x != TacticalAct));
            _actService.UseOn(Actor, Target, usedActs, Context.Sector);
        }

        private IEnumerable<ITacticalAct> GetSecondaryUsedActs()
        {
            var equipmentModule = Actor.Person.GetModuleSafe<IEquipmentModule>();
            var combatActModule = Actor.Person.GetModule<ICombatActModule>();
            var currentActs = combatActModule.CalcCombatActs();

            if (equipmentModule == null)
            {
                // If person has not equipment, the has only one default action in current moment. Use it.
                yield return currentActs.Single();
            }
            else
            {
                var usedEquipmentActs = false;
                var slots = equipmentModule.Slots;
                for (var i = 0; i < slots.Length; i++)
                {
                    var slotEquipment = equipmentModule[i];
                    if (slotEquipment == null)
                    {
                        continue;
                    }

                    if ((slots[i].Types & EquipmentSlotTypes.Hand) == 0)
                    {
                        continue;
                    }

                    var equipmentActs = from act in currentActs
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
                    // If no equipment was selected then use default act. Each person has only one default act in current moment.
                    yield return currentActs.Single();
                }
            }
        }
    }
}