using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.World;

namespace Zilon.Core.Tactics.Behaviour
{
    public class AttackTask : OneTurnActorTaskBase
    {
        private readonly ITacticalActUsageService _actService;

        public AttackTask(IActor actor,
            IActorTaskContext context,
            IAttackTarget target,
            ICombatAct tacticalAct,
            ITacticalActUsageService actService) :
            base(actor, context)
        {
            _actService = actService;

            TargetObject = target ?? throw new ArgumentNullException(nameof(target));
            TacticalAct = tacticalAct ?? throw new ArgumentNullException(nameof(tacticalAct));

            TargetNode = target.Node;

            var combatActDuration = tacticalAct.Stats.Duration.GetValueOrDefault(1);
            var durationBonus = GetDurationBonus(actor);
            var durationWithBonus =
                (int)Math.Round(GlobeMetrics.OneIterationLength * combatActDuration * durationBonus);
            Cost = durationWithBonus;
        }

        public override int Cost { get; }

        public ICombatAct TacticalAct { get; }

        public IGraphNode TargetNode { get; }

        public IAttackTarget TargetObject { get; }

        protected override void ExecuteTask()
        {
            if (Actor.Person.GetModuleSafe<ICombatActModule>() is null)
            {
                throw new NotImplementedException("Не неализована возможность атаковать без навыков.");
            }

            if (!TargetObject.CanBeDamaged())
            {
                // Эта ситуация может произойти, когда:
                // 1. Текущий актёр начал выполнять задачу.
                // 2. Цель убили/умерла сама.
                // 3. Наступил момент, когда задача текущего актёра должна выполниться.

                // Эту првоерку нужно проводить выше, когда пользователю сообщаяется возможность
                // выполнить эту задачу до её начала.
                return;
            }

            var stopToUseAct = CheckStopToUseAct();
            if (stopToUseAct)
            {
                //TODO Start acts cooldown, spend resources, etc.
                return;
            }

            var availableSlotAct = GetSecondaryUsedActs();

            var primary = new[] { TacticalAct };
            var secondary = availableSlotAct.Where(x => x != TacticalAct).Skip(1).ToArray();

            var usedActs = new UsedTacticalActs(primary, secondary);

            var actTargetInfo = new ActTargetInfo(TargetObject, TargetNode);

            _actService.UseOn(Actor, actTargetInfo, usedActs, Context.Sector);
        }

        private bool CheckStopToUseAct()
        {
            var stopToUseAct = false;
            if (TargetObject is IActor)
            {
                var targetObjectInSector = ActUsageHelper.SectorHasAttackTarget(Context.Sector, TargetObject);
                if (!targetObjectInSector)
                {
                    // Actors can leave sector after attacker start to attack but before attacker will ready to execute task.
                    stopToUseAct = true;
                }
            }
            else
            {
                var targetObjectInSector = ActUsageHelper.SectorHasAttackTarget(Context.Sector, TargetObject);
                if (!targetObjectInSector)
                {
                    // Static object can't move. So they can't leave sector after attacker start to attack but before attacker will ready to execute task like actors.
                    // This means error.
                    throw new TaskException("Try to attack static object in other sector.");
                }
            }

            return stopToUseAct;
        }

        private static float GetDurationBonus(IActor actor)
        {
            var dexterity =
                (actor.Person.GetModuleSafe<IAttributesModule>()?.GetAttribute(PersonAttributeType.Dexterity)?.Value)
                .GetValueOrDefault(10);
            if (dexterity > 10)
            {
                return 0.75f;
            }

            if (dexterity < 10)
            {
                return 1.25f;
            }

            return 1f;
        }

        private IEnumerable<ICombatAct> GetSecondaryUsedActs()
        {
            var equipmentModule = Actor.Person.GetModuleSafe<IEquipmentModule>();
            var combatActModule = Actor.Person.GetModule<ICombatActModule>();
            var currentActs = combatActModule.GetCurrentCombatActs();

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

                    //TODO Add CanUseAsSecondary to equipment
                    if (slotEquipment.Scheme.Tags?.Contains("shield") == true)
                    {
                        continue;
                    }

                    if ((slots[i].Types & EquipmentSlotTypes.Hand) == 0)
                    {
                        continue;
                    }

                    var equipmentActs = from act in currentActs
                                        where act.Equipment == slotEquipment
                                        //TODO Add CanUseAsSecondary to combat act
                                        //where act.Scheme.Sid != "shield-push"
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
                    // If no equipment was selected then use default act. Each person has only one default act in current moment. -- INCORRECT
                    // THERE ARE MULTYPLE DIFFERENT DEFAULT ACTS.
                    yield return currentActs.First(x => x.Scheme.Sid == "punch");
                }
            }
        }
    }
}