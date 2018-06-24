using System;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class AttackTask : ActorTaskBase
    {
        /// <summary>
        /// Возможная дистанция атаки.
        /// </summary>
        private const int ATTACK_DISTANCE = 1;

        private IAttackTarget _target;

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

            _target.TakeDamage(Actor.Damage);

            IsComplete = true;
        }

        public AttackTask(IActor actor, IAttackTarget target): base(actor)
        {
            if (actor == target)
            {
                throw new ArgumentException("Актур не может атаковать сом себя", nameof(target));
            }

            _target = target;
        }
    }
}
