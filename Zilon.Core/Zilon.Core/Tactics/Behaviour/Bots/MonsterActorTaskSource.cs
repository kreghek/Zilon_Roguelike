using System;
using System.Collections.Generic;

using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public class MonsterActorTaskSource : IActorTaskSource
    {
        private readonly Dictionary<IActor, IBotLogic> _logicDict;
        private readonly IPlayer _player;
        private readonly Dictionary<IActor, IPatrolRoute> _patrolRoutes;
        private readonly IDecisionSource _decisionSource;
        private readonly ITacticalActUsageService _actService;

        public MonsterActorTaskSource(IPlayer player,
            Dictionary<IActor, IPatrolRoute> patrolRoutes,
            IDecisionSource decisionSource,
            ITacticalActUsageService actService)
        {
            _logicDict = new Dictionary<IActor, IBotLogic>();
            _player = player;
            _patrolRoutes = patrolRoutes;
            _decisionSource = decisionSource;
            _actService = actService;
        }

        public IActorTask[] GetActorTasks(IMap map, IActorManager actorManager)
        {
            var actorTasks = new List<IActorTask>();
            foreach (var actor in actorManager.Actors)
            {
                if (actor.Owner != _player)
                {
                    continue;
                }

                if (actor.State.IsDead)
                {
                    _logicDict.Remove(actor);
                    _patrolRoutes.Remove(actor);
                }
                else
                {
                    if (!_logicDict.TryGetValue(actor, out var logic))
                    {
                        if (_patrolRoutes.TryGetValue(actor, out var partolRoute))
                        {

                            var patrolLogic = new PatrolLogic(actor, partolRoute, map, actorManager, _decisionSource, _actService);
                            _logicDict[actor] = patrolLogic;
                            logic = patrolLogic;
                        }
                        else
                        {
                            throw new InvalidOperationException($"Для актёра {actor} не задан маршрут.");
                        }
                    }

                    var currentTask = logic.GetCurrentTask();
                    actorTasks.Add(currentTask);
                }
            }

            return actorTasks.ToArray();
        }
    }
}
