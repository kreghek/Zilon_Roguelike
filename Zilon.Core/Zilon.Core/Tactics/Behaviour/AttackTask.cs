using System;

namespace Zilon.Core.Tactics.Behaviour
{
    public class AttackTask : IActorTask
    {
        /// <summary>
        /// Возможная дистанция атаки.
        /// </summary>
        private const int ATTACK_DISTANCE = 1;

        private IAttackTarget _target;

        public IActor Actor { get; }

        public bool IsComplete { get; set; }

        public void Execute()
        {
            if (!_target.CanBeDamaged())
            {
                throw new InvalidOperationException("Попытка атаковать цель, которой нельзя нанести урон.");
            }

            var currentCubePos = Actor.Node.CubeCoords;
            var targetCubePos = _target.Node.CubeCoords;

            var distance = currentCubePos.DistanceTo(targetCubePos);

            if (distance != ATTACK_DISTANCE)
            {
                throw new InvalidOperationException("Попытка атаковать цель, находящуюся за пределами атаки.");
            }

            _target.TakeDamage(Actor.Damage);
        }

        public AttackTask(IActor actor, IAttackTarget target)
        {
            _target = target;
            Actor = actor;
        }
    }
}
