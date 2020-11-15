using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Strategies
{
    public sealed class LogicTreeStrategy : ILogicStrategy
    {
        private readonly LogicStateTree _stateTree;

        private readonly LogicTreeStrategyData _strategyData;

        public LogicTreeStrategy(IActor actor, LogicStateTree stateTree)
        {
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
            _stateTree = stateTree ?? throw new ArgumentNullException(nameof(stateTree));

            _strategyData = new LogicTreeStrategyData();

            CurrentState = _stateTree.StartState;
        }

        public bool WriteStateChanges { get; set; }

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

        private bool SelectCurrentState(
            ILogicState currentState,
            ISectorTaskSourceContext context,
            out ILogicState newState)
        {
            var transitionWasPerformed = false;
            newState = null;

            var currentStateTransitions = _stateTree.Transitions[CurrentState];

            foreach (var transition in currentStateTransitions)
            {
                var trigger = transition.Trigger;

                var isFired = trigger.Test(Actor, context, currentState, _strategyData);
                if (isFired)
                {
                    newState = transition.NextState;
                    transitionWasPerformed = true;
                    break;
                }
            }

            return transitionWasPerformed;
        }

        private void UpdateCurrentTriggers(IEnumerable<ILogicStateTrigger> currentLogicTriggers)
        {
            foreach (var trigger in currentLogicTriggers)
            {
                trigger.Update();
            }
        }

        public IActor Actor { get; }

        public ILogicState CurrentState { get; private set; }

        public ILogicStrategyData StrategyData => _strategyData;

        public IActorTask GetActorTask(ISectorTaskSourceContext context)
        {
            // Для текущего состояния проверяем каждый из переходов.
            // Если переход выстреливает, то генерируем задачу актёру.
            // Алгоритм делится на два этапа:
            // 1. Для текущей логики проверяются все триггеры по порядку. Если какой-то триггер выстреливает, то связанная
            // через переход логика фиксируется, как текущая. За одну итерацию может быть только один переход.
            // 2. Получаем задачу из текущей логики. Если текущая логика возвращает пустую задачу - значит
            // переводим её в соответствующее состояние и ждём перехода из неё с помощью триггеров (этап 1).
            // Второй пункт означает, что каждая логика обязана выдавать хотя бы пустую задачу, если она
            // не может выполниться. И выставлять себе статус выполненной. В дереве состояний нужно следить за тем, чтобы 
            // все логики имели триггер на окончание с переходом на какую-либо задачу.

            // После окончания всех проверок триггеров выполняется обновление состояния триггеров. Некоторые триггеры
            // могут иметь счётчики или логику, которая выполняется при каждой итерации (считай, каждый ход).

            var transitionWasPerformed = SelectCurrentState(CurrentState, context, out var newState);

            if (transitionWasPerformed)
            {
                if (WriteStateChanges)
                {
                    Console.WriteLine(newState);
                }

                CurrentState = newState;
                ResetLogicStates(_stateTree);
            }

            var actorTask = CurrentState.GetTask(Actor, context, _strategyData);
            var currentTriggers = _stateTree.Transitions[CurrentState].Select(x => x.Trigger);
            UpdateCurrentTriggers(currentTriggers);

            return actorTask;
        }
    }
}