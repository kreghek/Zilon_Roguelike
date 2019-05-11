using System.Collections.Generic;

using Zilon.Bot.Sdk;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Bot.Players
{
    public class HumanBotActorTaskSource : ISectorActorTaskSource
    {
        private readonly HumanPlayer _player;
        private readonly IDecisionSource _decisionSource;
        private readonly ITacticalActUsageService _actService;
        private readonly ISectorManager _sectorManager;
        private readonly IActorManager _actorManager;

        private readonly Dictionary<IActor, List<ILogicStateSelector>> _logicDict;

        public HumanBotActorTaskSource(HumanPlayer player,
                                       IDecisionSource decisionSource,
                                       ITacticalActUsageService actService,
                                       ISectorManager sectorManager,
                                       IActorManager actorManager)
        {
            _player = player;
            _decisionSource = decisionSource;
            _actService = actService;
            _sectorManager = sectorManager;
            _actorManager = actorManager;

            _logicDict = new Dictionary<IActor, List<ILogicStateSelector>>();
        }

        public IActorTask[] GetActorTasks(IActor actor)
        {
            // TODO Лучше сразу отдавать на обработку актёров текущего игрока.
            if (actor.Owner != _player)
            {
                return new IActorTask[0];
            }

            // Алгоритм состоит из двух компонент:
            // -- логика.
            // ---- возвращает задачу актёра.
            // -- селектор логики.
            // ---- проверка условия генерации логики.
            // ---- генерация логики (создание новой или выбор существующей).
            // Логика и селекторы содержат состояние, завязанное на актёра.
            // Все селекторы организованы в список. Проверка начинается с первого по списку селектора.
            // Сначала селектор выбирает логику для текущего актёра.
            // Если условия генерации выстреливают, вызывается метод генерации логики.
            // В ином случае проверяются условия следующего селектора.

            if (actor.Person.Survival.IsDead)
            {
                _logicDict.Remove(actor);
                _sectorManager.CurrentSector.PatrolRoutes.Remove(actor);
            }
            else
            {
                if (_logicDict.TryGetValue(actor, out var logicSelectors))
                {
                    var logicStateSelector = GetReadyLogicSelector(logicSelectors);

                    var logicState = logicStateSelector?.GenerateLogic();

                    var actorTask = logicState?.GetCurrentTask();

                    if (actorTask == null)
                    {
                        return new IActorTask[0];
                    }

                    return new IActorTask[] { actorTask };
                }
            }

            return new IActorTask[0];
        }

        private ILogicStateSelector GetReadyLogicSelector(IEnumerable<ILogicStateSelector> logicStateSelectors)
        {
            // Перебираем все селекторы логики, пока не будут выполнены условия генерации.
            // На выходе возвращаем селектор, готовый к генерации логики.
            foreach (var logicSelector in logicStateSelectors)
            {
                if (logicSelector.CheckConditions())
                {
                    return logicSelector;
                }
            }

            return null;
        }
    }
}
