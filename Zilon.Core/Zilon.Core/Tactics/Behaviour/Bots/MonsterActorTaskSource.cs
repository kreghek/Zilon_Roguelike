using System.Collections.Generic;

using Zilon.Core.Players;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public class MonsterActorTaskSource : IActorTaskSource
    {
        private readonly Dictionary<IActor, IBotLogic> _logicDict;
        private readonly IPlayer _player;
        private readonly IDecisionSource _decisionSource;
        private readonly ITacticalActUsageService _actService;
        private readonly ISectorManager _sectorManager;
        private readonly IActorManager _actorManager;

        public MonsterActorTaskSource(IBotPlayer player,
            IDecisionSource decisionSource,
            ITacticalActUsageService actService,
            ISectorManager sectorManager,
            IActorManager actorManager): this((IPlayer)player, decisionSource, actService, sectorManager, actorManager)
        {

        }

        protected MonsterActorTaskSource(IPlayer player,
            IDecisionSource decisionSource,
            ITacticalActUsageService actService,
            ISectorManager sectorManager,
            IActorManager actorManager)
        {
            _logicDict = new Dictionary<IActor, IBotLogic>();
            _player = player;
            _decisionSource = decisionSource;
            _actService = actService;
            _sectorManager = sectorManager;
            _actorManager = actorManager;
        }

        public IActorTask[] GetActorTasks(IActor actor)
        {
            // TODO Лучше сразу отдавать на обработку актёров текущего игрока.
            if (actor.Owner != _player)
            {
                return new IActorTask[0];
            }

            var actorTasks = new List<IActorTask>();
            if (actor.Person.Survival.IsDead)
            {
                _logicDict.Remove(actor);
                //TODO Избавиться от этой зависимости.
                // Лучше в секторе подписаться на смерть актёра и удалять там из списка маршрутов.
                _sectorManager.CurrentSector.PatrolRoutes.Remove(actor);
            }
            else
            {
                if (!_logicDict.TryGetValue(actor, out var logic))
                {
                    if (_sectorManager.CurrentSector.PatrolRoutes.TryGetValue(actor, out var partolRoute))
                    {

                        var patrolLogic = new PatrolLogic(actor,
                            partolRoute,
                            _sectorManager.CurrentSector.Map,
                            _actorManager,
                            _decisionSource,
                            _actService);

                        _logicDict[actor] = patrolLogic;
                        logic = patrolLogic;
                    }
                    else
                    {
                        var idleLogic = new RoamingLogic(actor,
                            _sectorManager.CurrentSector.Map,
                            _actorManager,
                            _decisionSource,
                            _actService);

                        _logicDict[actor] = idleLogic;
                        logic = idleLogic;
                    }
                }

                var currentTask = logic.GetCurrentTask();
                actorTasks.Add(currentTask);
            }

            return actorTasks.ToArray();
        }
    }
}
