using System;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class AttackTask : OneTurnActorTaskBase
    {
        private readonly ITacticalActUsageService _actService;
        private readonly IMap _map;

        public IAttackTarget Target { get; }

        protected override void ExecuteTask()
        {
            if (!Target.CanBeDamaged())
            {
                throw new InvalidOperationException("Попытка атаковать цель, которой нельзя нанести урон.");
            }

            if (Actor.Person.TacticalActCarrier == null)
            {
                throw new NotImplementedException("Не неализована возможность атаковать без навыков.");
            }

            var actCarrier = Actor.Person.TacticalActCarrier;
            var act = actCarrier.Acts.FirstOrDefault();
            if (act == null)
            {
                throw new InvalidOperationException("Не найдено действий.");
            }

            var targetNode = Target.Node;

            var targetIsOnLine = MapHelper.CheckNodeAvailability(_map, Actor.Node, targetNode);
            var isInDistance = act.CheckDistance(((HexNode)Actor.Node).CubeCoords, ((HexNode)targetNode).CubeCoords);

            var canExecute = targetIsOnLine && isInDistance;

            if (!canExecute)
            {
                throw new InvalidOperationException("Задачу на атаку нельзя выполнить сквозь стены.");
            }


            Actor.UseAct(Target, act);
            _actService.UseOn(Actor, Target, act);
        }

        public AttackTask(IActor actor,
            IAttackTarget target,
            ITacticalActUsageService actService,
            IMap map) :
            base(actor)
        {
            _actService = actService;
            _map = map;

            Target = target;
        }
    }
}