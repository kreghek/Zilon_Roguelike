using System;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public class TacticalActUsageService : ITacticalActUsageService
    {
        private readonly IActUsageRandomSource _actUsageRandomSource;
        private readonly IPerkResolver _perkResolver;

        public TacticalActUsageService(IActUsageRandomSource actUsageRandomSource, IPerkResolver perkResolver)
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

            var minEfficient = act.MinEfficient;
            var maxEfficient = act.MaxEfficient;
            var rolledEfficient = _actUsageRandomSource.SelectEfficient(minEfficient, maxEfficient);

            if (target is IActor targetActor)
            {
                var targetIsDeadLast = targetActor.State.IsDead;

                targetActor.TakeDamage(rolledEfficient);

                if (!targetIsDeadLast && targetActor.State.IsDead)
                {
                    if (!(actor.Person is MonsterPerson))
                    {
                        var evolutionData = actor.Person.EvolutionData;

                        var defeatProgress = new DefeatActorJobProgress(targetActor);

                        _perkResolver.ApplyProgress(defeatProgress, evolutionData);
                    }
                }
            }
            else
            {
                target.TakeDamage(rolledEfficient);
            }
        }
    }
}
