using System;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    using Zilon.Core.Persons;
    using Zilon.Core.Tactics.Behaviour.Bots;

    public class AttackTask : ActorTaskBase
    {
        /// <summary>
        /// Возможная дистанция атаки.
        /// </summary>
        private const int ATTACK_DISTANCE = 1;

        private readonly IAttackTarget _target;
        private readonly IDecisionSource _decisionSource;

        public override void Execute()
        {
            if (!_target.CanBeDamaged())
            {
                throw new InvalidOperationException("Попытка атаковать цель, которой нельзя нанести урон.");
            }

            var currentCubePos = ((HexNode)Actor.Node).CubeCoords;
            var targetCubePos = ((HexNode)_target.Node).CubeCoords;

            var distance = currentCubePos.DistanceTo(targetCubePos);

            if (distance != ATTACK_DISTANCE)
            {
                throw new InvalidOperationException("Попытка атаковать цель, находящуюся за пределами атаки.");
            }


            if (Actor.Person is ITacticalActCarrier actCarrier)
            {
                var minEfficient = actCarrier.DefaultAct.MinEfficient;
                var maxEfficient = actCarrier.DefaultAct.MaxEfficient;
                var rolledEfficient = _decisionSource.SelectEfficient(minEfficient, maxEfficient);
                _target.TakeDamage(rolledEfficient);
            }

            IsComplete = true;
        }

        public AttackTask(IActor actor, IAttackTarget target, IDecisionSource decisionSource): base(actor)
        {
            if (actor == target)
            {
                throw new ArgumentException("Актур не может атаковать сом себя", nameof(target));
            }

            _target = target;
            _decisionSource = decisionSource;
        }
    }
}
