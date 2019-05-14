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
            // Логика может вернуть нулевую задачу, когда дальнейшее исполнение невозможно.
            // Если задача актёра не нулевая, то считаем логику выстрелившего перехода текущей
            // и дальше работаем с ней.

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
                actorTask = CurrentState.GetTask(Actor, _currentStateData);
            }

            return actorTask;
        }
    }
}
