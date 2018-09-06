using System;
using System.Collections.Generic;

using Zilon.Core.Players;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public class MonsterActorTaskSource : IActorTaskSource
    {
        private readonly Dictionary<IActor, IBotLogic> _logicDict;
        private readonly IBotPlayer _player;
        private readonly IDecisionSource _decisionSource;
        private readonly ITacticalActUsageService _actService;
        private readonly ISector _sector;
        private readonly IActorManager _actorManager;

        public MonsterActorTaskSource(IBotPlayer player,
            IDecisionSource decisionSource,
            ITacticalActUsageService actService,
            ISector sector,
            IActorManager actorManager)
        {
            _logicDict = new Dictionary<IActor, IBotLogic>();
            _player = player;
            _decisionSource = decisionSource;
            _actService = actService;
            _sector = sector;
            _actorManager = actorManager;
        }

        public IActorTask[] GetActorTasks(IActor actor)
        {
            if (actor.Owner != _player)
            {
                return new IActorTask[0];
            }

            var actorTasks = new List<IActorTask>();
            if (actor.State.IsDead)
            {
                _logicDict.Remove(actor);
                _sector.PatrolRoutes.Remove(actor);
            }
            else
            {
                if (!_logicDict.TryGetValue(actor, out var logic))
                {
                    if (_sector.PatrolRoutes.TryGetValue(actor, out var partolRoute))
                    {

                        var patrolLogic = new PatrolLogic(actor,
                            partolRoute,
                            _sector.Map,
                            _actorManager,
                            _decisionSource,
                            _actService);

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

            return actorTasks.ToArray();
        }
    }
}
