using System;
using System.Collections.Generic;

using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class MonsterActorTaskSource : IActorTaskSource
    {
        private readonly Dictionary<IActor, IBotLogic> _logicDict;
        private readonly IPlayer _player;
        private readonly Dictionary<IActor, IPatrolRoute> _patrolRoutes;
        private readonly IDecisionSource _decisionSource;

        public MonsterActorTaskSource(IPlayer player,
            Dictionary<IActor, IPatrolRoute> patrolRoutes,
            IDecisionSource decisionSource)
        {
            _logicDict = new Dictionary<IActor, IBotLogic>();
            _player = player;
            _patrolRoutes = patrolRoutes;
            _decisionSource = decisionSource;
        }

        public IActorTask[] GetActorTasks(IMap map, IActorManager actorList)
        {
            var actorTasks = new List<IActorTask>();
            foreach (var actor in actorList.Actors)
            {
                if (actor.Person.Player != _player)
                {
                    continue;
                }

                if (actor.IsDead)
                {
                    _logicDict.Remove(actor);
                    _patrolRoutes.Remove(actor);
                }
                else
                {
                    if (!_logicDict.TryGetValue(actor, out IBotLogic logic))
                    {
                        if (_patrolRoutes.TryGetValue(actor, out IPatrolRoute partolRoute))
                        {

                            var patrolLogic = new PatrolLogic(actor, partolRoute, map, actorList, _decisionSource);
                            _logicDict[actor] = patrolLogic;
                            logic = patrolLogic;
                        }
                        else
                        {
                            throw new ArgumentException($"Для актёра {actor} не задан маршрут.",
                                nameof(partolRoute));
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
