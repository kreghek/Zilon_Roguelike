using System.Collections.Generic;

using Zilon.Bot.Sdk;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public abstract class BotActorTaskSourceBase : ISectorActorTaskSource
    {
        private readonly IPlayer _player;

        private readonly Dictionary<IActor, ILogicStrategy> _actorStrategies;

        public BotActorTaskSourceBase(IPlayer player)
        {
            _player = player;

            _actorStrategies = new Dictionary<IActor, ILogicStrategy>();
        }

        public abstract void Configure(IBotSettings botSettings);

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
            // Логика и селекторы не содержат состояние, завязанное на актёра. Состояние передаётся из стратегии.
            // ПОКА НЕ ПОНЯТНО, КАК ХРАНИТЬ И ПЕРЕДАВАТЬ СОСТОЯНИЕ. СОСТОЯНИЕ МОЖЕТ БЫТЬ СОЗДАНО ИЗ СЕЛЕКТОРА.
            // Все селекторы организованы в список в логике. Каждый селектор указывает на логику, которая будет
            // выбрана текущей в случае выполнения условия. Селекторы упорядочены по приоритету в рамках каждой логики.
            // Условия перехода делятся на две категории:
            // -- Определённое состояние окружения или актёра.
            // -- Окончание выполнения текущей логики (например, логика преследования или ожидания зависят от счётчика).
            // Если логика закончена, её нужно сменить. Если нет селектора, который указывает на следующую логику,
            // то выполнять переход на стартовую логику.

            if (!actor.Person.Survival.IsDead)
            {
                if (!_actorStrategies.TryGetValue(actor, out var logicStrategy))
                {
                    // Создаём стратегию для текущего актёра.
                    // Добавляем созданную стратегию в словарь стратегий.
                    logicStrategy = GetLogicStrategy(actor);
                    _actorStrategies[actor] = logicStrategy;
                }

                var actorTask = logicStrategy.GetActorTask();

                if (actorTask == null)
                {
                    return new IActorTask[0];
                }

                return new IActorTask[] { actorTask };
            }
            else
            {
                _actorStrategies.Remove(actor);
            }

            return new IActorTask[0];
        }

        protected abstract ILogicStrategy GetLogicStrategy(IActor actor);
    }
}
