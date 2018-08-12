using System;

namespace Zilon.Core.Tactics.Behaviour
{
    using System.Linq;

    public class AttackTask : OneTurnActorTaskBase
    {
        private readonly IAttackTarget _target;
        private readonly ITacticalActUsageService _actService;

        protected override void ExecuteTask()
        {
            //TODO Сделать проверку, чтобы нельзя было атаковать через стены, аналогично команде на открытие/атаку.
            if (!_target.CanBeDamaged())
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

                Actor.UseAct(_target, act);
                _actService.UseOn(Actor, _target, act);
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
            _target = target;
            _actService = actService;
        }
    }
}