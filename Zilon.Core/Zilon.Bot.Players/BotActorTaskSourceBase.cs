using Zilon.Bot.Sdk;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public abstract class BotActorTaskSourceBase<TContext> : ISectorActorTaskSource<TContext>
        where TContext : class, ISectorTaskSourceContext
    {
        //TODO Есть риск утечки.
        // Актёры могут быть удалены, но информация о них будет храниться здесь, предотвращая чистку.
        // Рассмотреть варианты:
        // 1. Периодическая чистка внутри самого источника команд. Тогда нужна зависимость от менеджера актёров.
        // 2. В интерфейс добавить метод для обработки удаления актёра.
        private readonly Dictionary<IActor, ILogicStrategy> _actorStrategies;

        protected BotActorTaskSourceBase()
        {
            _actorStrategies = new Dictionary<IActor, ILogicStrategy>();
        }

        protected abstract ILogicStrategy GetLogicStrategy(IActor actor);

        public void CancelTask(IActorTask cencelledActorTask)
        {
            // Этот метод был введен для HumanActorTaskSource.
            // В этой реализации источника команд не используется.
        }

        public abstract void Configure(IBotSettings botSettings);

        public Task<IActorTask> GetActorTaskAsync(IActor actor, TContext context)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
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

            if (actor.CanExecuteTasks)
            {
                if (!_actorStrategies.TryGetValue(actor, out var logicStrategy))
                {
                    // Создаём стратегию для текущего актёра.
                    // Добавляем созданную стратегию в словарь стратегий.
                    logicStrategy = GetLogicStrategy(actor);
                    _actorStrategies[actor] = logicStrategy;
                }

                var actorTask = logicStrategy.GetActorTask(context);

                if (actorTask == null)
                {
                    var taskContext = new ActorTaskContext(context.Sector);
                    return Task.FromResult<IActorTask>(new IdleTask(actor, taskContext, 1));
                }

                return Task.FromResult(actorTask);
            }

            _actorStrategies.Remove(actor);

            // Сюда попадаем в случае смерти персонажа.
            // Когда мы пытаемся выполнить какую-то задачу, а персонаж при это был/стал мертв.
            throw new InvalidOperationException("Произведена попытка получить задачу для мертвого персонажа.");
        }

        public void ProcessTaskComplete(IActorTask actorTask)
        {
            // Этот метод был введен для HumanActorTaskSource.
            // В этой реализации источника команд не используется.
        }

        public void ProcessTaskExecuted(IActorTask actorTask)
        {
            // Этот метод был введен для HumanActorTaskSource.
            // Нужен для того, чтобы сразу начать выполнение следующего действия, не дожидаясь окончания текущего.
            // Например, при использовании парного оружия.
            // Механика пока не реализована.
        }
    }
}