using System;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    using System.Linq;

    public class AttackTask : ActorTaskBase
    {
        private readonly IAttackTarget _target;
        private readonly IDecisionSource _decisionSource;

        public override void Execute()
        {
            //TODO Сделать проверку, чтобы нельзя было атаковать через стены, аналогично команде на открытие/атаку.
            if (!_target.CanBeDamaged())
            {
                throw new InvalidOperationException("Попытка атаковать цель, которой нельзя нанести урон.");
            }

            var currentCubePos = ((HexNode)Actor.Node).CubeCoords;
            var targetCubePos = ((HexNode)_target.Node).CubeCoords;

            if (Actor.Person.TacticalActCarrier != null)
            {
                var actCarrier = Actor.Person.TacticalActCarrier;
                var act = actCarrier.Acts.FirstOrDefault();
                if (act == null)
                {
                    throw new InvalidOperationException("Не найдено действий.");
                }

                var isInDistance = act.CheckDistance(currentCubePos, targetCubePos);
                if (!isInDistance)
                {
                    throw new InvalidOperationException("Попытка атаковать цель, находящуюся за пределами атаки.");
                }

                var minEfficient = act.MinEfficient;
                var maxEfficient = act.MaxEfficient;
                var rolledEfficient = _decisionSource.SelectEfficient(minEfficient, maxEfficient);
                _target.TakeDamage(rolledEfficient);
            }
            else
            {
                throw new NotImplementedException("Не неализована возможность атаковать без навыков.");
            }

            IsComplete = true;
        }

        public AttackTask(IActor actor, IAttackTarget target, IDecisionSource decisionSource) : base(actor)
        {
            if (actor == target)
            {
                throw new ArgumentException("Актур не может атаковать сам себя", nameof(target));
            }

            _target = target;
            _decisionSource = decisionSource;
        }
    }
}