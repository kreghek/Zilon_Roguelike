using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Strategies
{
    public sealed class LogicTreeStrategy : ILogicStrategy
    {
        private readonly LogicStateTree _stateTree;

        private LogicTreeStrategyData _strategyData;

        public LogicTreeStrategy(IActor actor, LogicStateTree stateTree)
        {
            Actor = actor;
            _stateTree = stateTree;

            CurrentState = _stateTree.StartState;
        }

        public IActor Actor { get; }
        public ILogicState CurrentState { get; private set; }
        public ILogicStrategyData StrategyData => _strategyData;

        public IActorTask GetActorTask()
        {
            // Для текущего состояния проверяем каждый из переходов.
            // Если переход выстреливает, то генерируем задачу актёру.
            // Алгоритм делится на два этапа:
            // 1. Определение текущей логики по переходам. То есть из текущей переходим до тех пор,
            // пока у очередной логики все переходы не выстрелят. Эта логика будет зафиксирована, как текущая.
            // 2. Получаем задачу из текущей логики. Если текущая логика возвращает пустую задачу - значит
            // переводим её в соответствующее состояние и ждём перехода из неё.
            // Второй пункт означает, что каждая логика обязана выдавать хотя бы пустую задачу, если она
            // не может выполниться. И выставлять себе статус выполненной. В дереве состояний нужно следить за тем, чтобы 
            // все логики имели триггер на окончание с переходом на какую-либо задачу.

            var newCurrentState = SelectCurrentState();
            if (newCurrentState != CurrentState)
            {
                CurrentState = newCurrentState;
                ResetLogicStates(_stateTree);
            }

            var actorTask = CurrentState.GetTask(Actor, _strategyData);

            var currentTriggers = _stateTree.Transitions[CurrentState].Select(x => x.Trigger);
            UpdateCurrentTriggers(currentTriggers);

            return actorTask;
        }

        private ILogicState SelectCurrentState()
        {
            // Историю переходов генерируем только для исключительных ситуаций,
            // когда выбор логики зацикливается.
            var stateHistory = new List<LogicStateTrack>
            {
                new LogicStateTrack(CurrentState, firedTrigger: null)
            };

            var currentState = CurrentState;
            var safityCounter = 100;
            bool anyTriggerFired;
            do
            {
                var currentStateTransitions = _stateTree.Transitions[currentState];

                anyTriggerFired = false;
                foreach (var transition in currentStateTransitions)
                {
                    var trigger = transition.Trigger;

                    var isFired = trigger.Test(Actor, currentState, _strategyData);
                    if (isFired)
                    {
                        currentState = transition.NextState;
                        stateHistory.Add(new LogicStateTrack(currentState, firedTrigger: trigger));
                        anyTriggerFired = true;
                        break;
                    }
                }

                safityCounter--;
            } while (anyTriggerFired && safityCounter > 0);

            if (safityCounter <= 0)
            {
                throw new BotStrategyException(stateHistory);
            }

            return currentState;
        }

        private void ResetLogicStates(LogicStateTree logicStateTree)
        {
            foreach (var transition in logicStateTree.Transitions)
            {
                transition.Key.Reset();

                foreach (var trigger in transition.Value)
                {
                    trigger.Trigger.Reset();
                }
            }
        }

        private void UpdateCurrentTriggers(IEnumerable<ILogicStateTrigger> currentLogicTriggers)
        {
            foreach (var trigger in currentLogicTriggers)
            {
                trigger.Update();
            }
        }
    }
}
