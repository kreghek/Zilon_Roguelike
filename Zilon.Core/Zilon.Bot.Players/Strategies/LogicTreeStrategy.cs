using System;
using System.Collections.Generic;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Strategies
{
    public sealed class LogicTreeStrategy : ILogicStrategy
    {
        private readonly Dictionary<ILogicStateTrigger, ILogicStateData> _currentStateTransitionData;
        private readonly LogicStateTree _stateTree;
        private ILogicStateData _currentStateData;

        public LogicTreeStrategy(IActor actor, LogicStateTree stateTree)
        {
            Actor = actor;
            _stateTree = stateTree;
            _currentStateTransitionData = new Dictionary<ILogicStateTrigger, ILogicStateData>();

            CurrentState = _stateTree.StartState;
        }

        public IActor Actor { get; }
        public ILogicState CurrentState { get; private set; }

        public IActorTask GetActorTask()
        {
            // Для текущего состояния проверяем каждый из переходов.
            // Если переход выстреливает, то генерируем задачу актёру.
            //--- это не правильно, НЕПРАВИЛЬНЫЙ БЛОК -----
            // Логика может вернуть нулевую задачу, когда дальнейшее исполнение невозможно.
            // Если задача актёра не нулевая, то считаем логику выстрелившего перехода текущей
            // и дальше работаем с ней.
            // -- конец это неправильно, НЕПРАВИЛЬНЫЙ БЛОК ------
            // Алгоритм делится на два этапа:
            // 1. Определение текущей логики по переходам. То есть из текущей переходим до тех пор,
            // пока у очередной логики все переходы не выстрелят. Эта логика будет зафиксирована, как текущая.
            // 2. Получаем задачу из текущей логики. Если текущая логика возвращает пустую задачу - эначит
            // переводим её в соответствующее состояние и ждём перехода из неё.
            // Второй пункт означает, что каждая логика способна выдавать хотя бы пустую задачу, если она
            // не может выполниться. А так же, что нужно избегать дедлоков - логика не может выполниться, а переходы ждут,
            // пока логика что-либо сделает. Для этого добавить в стратегию счётчики безопасности - если одна и та же логика
            // возвращает пустые задачи и не меняется определённое количество раз (100, например), выбрасывать исключение.
            //TODO Нужно переделать стратегию, логики и триггеры под описание выше. Сейчас работают, как описано в НЕПРАВИЛЬНОМ БЛОКЕ

            var currentStateTransitions = _stateTree.Transitions[CurrentState];
            IActorTask actorTask = null;
            foreach (var transition in currentStateTransitions)
            {
                var trigger = transition.Selector;

                if (!_currentStateTransitionData.TryGetValue(trigger, out var triggerData))
                {
                    triggerData = trigger.CreateData(Actor);
                    _currentStateTransitionData[trigger] = triggerData;
                }

                var isFired = trigger.Test(Actor, CurrentState, triggerData);
                if (isFired)
                {
                    var nextData = transition.NextState.CreateData(Actor);
                    actorTask = transition.NextState.GetTask(Actor, nextData);

                    if (actorTask != null)
                    {
                        CurrentState = transition.NextState;
                        _currentStateData = nextData;
                        ChangeState();
                        break;
                    }
                }
            }

            // Эта ситуация может произойти, если:
            // -- для текущей логики не выстрелило ниодного перехода.
            // -- переходы, которые выстрелили, вернули нулевую задачу.
            // Значит остаёмся с текущей логикой и запрашиваем у неё задачу.
            if (actorTask == null)
            {
                // Пустые данные состояния означают, что состояние ещё не было инициализировано.
                // Это происходит, например, при первом старте стратегии.
                if (_currentStateData == null)
                {
                    _currentStateData = CurrentState.CreateData(Actor);
                }

                actorTask = CurrentState.GetTask(Actor, _currentStateData);
            }

            return actorTask;
        }

        private void ChangeState()
        {
            _currentStateTransitionData.Clear();
        }
    }
}
