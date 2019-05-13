using System;
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

        private readonly Dictionary<IActor, ILogicStrategy> _logicDict;

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

            _logicDict = new Dictionary<IActor, ILogicStrategy>();
        }

        public IActorTask[] GetActorTasks(IActor actor)
        {
            // TODO Лучше сразу отдавать на обработку актёров текущего игрока.
            if (actor.Owner != _player)
            {
                return new IActorTask[0];
            }

            // Основные компоненты:
            // -- Стратегия.
            // ---- Стартовая логика (не равна нулю).
            // ---- Текущая логика (изначально равна стартовой).
            // ---- Текущее состояние для логик (счётчики, ссылки на объекты и т.д.)
            // ---- Возращает задачу актёра.
            // -- Логика.
            // ---- Список селекторов для выбора следующих логик.
            // ---- Признак завершения логики.
            // ---- Возвращает задачу актёра.
            // -- Селектор.
            // ---- Проверка условия перехода.
            // Стратегия является конечным автоматом. Узлы автомата - логики. Условия перехода - селекторы.
            // Стратегия содержит указатель на текущую логику.
            // Логика и селекторы не содержат состояние, завязанное на актёра. Состояние передаёются из стратегии.
            // Все селекторы организованы в список в логике. Каждый селектор указывает на логику, которая будет
            // выбрана текущей в случае выполнения условия. Селекторы упорядочены по приоритету в рамках каждой логики.
            // Услоя перехода грубо делятся на две категории:
            // -- Определённое состояние окружения или актёра.
            // -- Окончание выполнения текущей логики (например, логика преследования или ожидания зависят от счётчика).
            // Если логика закончена, её нужно сменить. Если нет селектора, который указывает на следующую логику,
            // то выполнять переход на стартовую логику.

            if (actor.Person.Survival.IsDead)
            {
                _logicDict.Remove(actor);
                _sectorManager.CurrentSector.PatrolRoutes.Remove(actor);
            }
            else
            {
                if (_logicDict.TryGetValue(actor, out var logicStrategy))
                {
                    //var readySelector = GetReadyLogicSelector(logicStrategy);

                    //var logicState = readySelector?.Selector.GenerateLogic(readySelector?.Result);

                    var actorTask = logicStrategy.GetActorTask();

                    if (actorTask == null)
                    {
                        return new IActorTask[0];
                    }

                    return new IActorTask[] { actorTask };
                }
                else
                {
                    // Создаём стратегию для текущего актёра.
                    // Добавляем созданную стратегию в словарь стратегий.
                }
            }

            return new IActorTask[0];
        }

        private ReadyLogicStateSelector GetReadyLogicSelector(IEnumerable<ILogicStateSelector> logicStateSelectors)
        {
            // Перебираем все селекторы логики, пока не будут выполнены условия генерации.
            // На выходе возвращаем селектор, готовый к генерации логики.
            foreach (var logicSelector in logicStateSelectors)
            {
                var result = logicSelector.CheckConditions();
                if (result != null)
                {
                    return new ReadyLogicStateSelector(logicSelector, result);
                }
            }

            return null;
        }

        private class ReadyLogicStateSelector
        {
            public ReadyLogicStateSelector(ILogicStateSelector selector, ILogicStateSelectorResult result)
            {
                Selector = selector ?? throw new ArgumentNullException(nameof(selector));
                Result = result ?? throw new ArgumentNullException(nameof(result));
            }

            public ILogicStateSelector Selector { get; }

            public ILogicStateSelectorResult Result { get; }
        }
    }
}
