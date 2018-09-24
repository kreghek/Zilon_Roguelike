using System;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public class TacticalActUsageService : ITacticalActUsageService
    {
        private readonly ITacticalActUsageRandomSource _actUsageRandomSource;
        private readonly IPerkResolver _perkResolver;

        public TacticalActUsageService(ITacticalActUsageRandomSource actUsageRandomSource, IPerkResolver perkResolver)
        {
            _actUsageRandomSource = actUsageRandomSource;
            _perkResolver = perkResolver;
        }

        public void UseOn(IActor actor, IAttackTarget target, ITacticalAct act)
        {
            //TODO реализовать возможность действовать на себя некоторыми скиллами.
            if (actor == target)
            {
                throw new ArgumentException("Актёр не может атаковать сам себя", nameof(target));
            }

            var currentCubePos = ((HexNode)actor.Node).CubeCoords;
            var targetCubePos = ((HexNode)target.Node).CubeCoords;

            var isInDistance = act.CheckDistance(currentCubePos, targetCubePos);
            if (!isInDistance)
            {
                throw new InvalidOperationException("Попытка атаковать цель, находящуюся за пределами атаки.");
            }

            var rolledEfficient = GetActEfficient(act);

            if (target is IActor targetActor)
            {
                UseOnActor(actor, rolledEfficient, targetActor);
            }
            else
            {
                UseOnChest(target, rolledEfficient);
            }
        }

        /// <summary>
        /// Возвращает случайное значение эффективность действия.
        /// </summary>
        /// <param name="act"> Соверщённое действие. </param>
        /// <returns> Возвращает выпавшее значение эффективности. </returns>
        private float GetActEfficient(ITacticalAct act)
        {
            var minEfficient = act.MinEfficient;
            var maxEfficient = act.MaxEfficient;
            var rolledEfficient = _actUsageRandomSource.SelectEfficient(minEfficient, maxEfficient);
            return rolledEfficient;
        }

        /// <summary>
        /// Применяет действие на предмет, на который можно подействовать (сундук/дверь).
        /// </summary>
        /// <param name="target"> Цель использования действия. </param>
        /// <param name="rolledEfficient"> Эффективность действия. </param>
        private static void UseOnChest(IAttackTarget target, float rolledEfficient)
        {
            target.TakeDamage(rolledEfficient);
        }

        /// <summary>
        /// Применяет действие на актёра.
        /// </summary>
        /// <param name="actor"> Актёр, который совершил действие. </param>
        /// <param name="rolledEfficient"> Эффективность действия. </param>
        /// <param name="targetActor"> Цель использования действия. </param>
        private void UseOnActor(IActor actor, float rolledEfficient, IActor targetActor)
        {
            var targetIsDeadLast = targetActor.State.IsDead;

            targetActor.TakeDamage(rolledEfficient);

            if (!targetIsDeadLast && targetActor.State.IsDead)
            {
                CountTargetActorDefeat(actor, targetActor);
            }
        }

        /// <summary>
        /// Расчитывает убийство целевого актёра.
        /// </summary>
        /// <param name="actor"> Актёр, который совершил действие. </param>
        /// <param name="targetActor"> Цель использования действия. </param>
        private void CountTargetActorDefeat(IActor actor, IActor targetActor)
        {
            if (actor.Person is MonsterPerson)
            {
                // Монстры не могут прокачиваться.
                return;
            }

            var evolutionData = actor.Person.EvolutionData;

            var defeatProgress = new DefeatActorJobProgress(targetActor);

            _perkResolver.ApplyProgress(defeatProgress, evolutionData);
        }
    }
}
