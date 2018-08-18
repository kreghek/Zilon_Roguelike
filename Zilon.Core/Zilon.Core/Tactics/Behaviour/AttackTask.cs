using System;

namespace Zilon.Core.Tactics.Behaviour
{
    using System.Linq;

    public class AttackTask : OneTurnActorTaskBase
    {
        private readonly ITacticalActUsageService _actService;

        public IAttackTarget Target { get; }

        protected override void ExecuteTask()
        {
            //TODO Сделать проверку, чтобы нельзя было атаковать через стены, аналогично команде на открытие/атаку.
            if (!Target.CanBeDamaged())
            {
                throw new InvalidOperationException("Попытка атаковать цель, которой нельзя нанести урон.");
            }

            if (Actor.Person.TacticalActCarrier != null)
            {
                var actCarrier = Actor.Person.TacticalActCarrier;
                var act = actCarrier.Acts.FirstOrDefault();
                if (act == null)
                {
                    throw new InvalidOperationException("Не найдено действий.");
                }

                Actor.UseAct(Target, act);
                _actService.UseOn(Actor, Target, act);
            }
            else
            {
                throw new NotImplementedException("Не неализована возможность атаковать без навыков.");
            }
        }

        public AttackTask(IActor actor,
            IAttackTarget target,
            ITacticalActUsageService actService) :
            base(actor)
        {
            Target = target;
            _actService = actService;
        }
    }
}