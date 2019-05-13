using System;
using System.Collections.Generic;
using Zilon.Bot.Players.Logics;
using Zilon.Bot.Players.Triggers;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Strategies
{
    public sealed class BasicStrategy : ILogicStrategy
    {
        private readonly Dictionary<ILogicStateTrigger, ILogicStateData> _currentStateTransitionData;
        private readonly Dictionary<ILogicState, IEnumerable<LogicTransition>> _transitions;
        private readonly ILogicState _startState;
        private readonly ILogicStateFactory _factory;
        private ILogicStateData _currentStateData;

        public BasicStrategy(IActor actor, ILogicStateFactory factory)
        {
            Actor = actor;
            _factory = factory;
            _currentStateTransitionData = new Dictionary<ILogicStateTrigger, ILogicStateData>();

            InitializeStateTree(ref _startState);

            CurrentState = StartState;
        }

        public IActor Actor { get; }
        public ILogicState StartState => _startState;
        public ILogicState CurrentState { get; private set; }

        public IActorTask GetActorTask()
        {
            var currentStateTransitions = _transitions[CurrentState];
            foreach (var transition in currentStateTransitions)
            {
                var trigger = transition.Selector;

                if (!_currentStateTransitionData.TryGetValue(trigger, out var triggerData))
                {
                    triggerData = trigger.CreateData(Actor);
                    _currentStateTransitionData[trigger] = triggerData;
                }

                var isFired = trigger.Test(Actor, triggerData);
                if (isFired)
                {
                    CurrentState = transition.NextState;
                    _currentStateData = CurrentState.CreateData(Actor);
                    break;
                }
            }

            var actorTask = CurrentState.GetCurrentTask(Actor, _currentStateData);
            return actorTask;
        }

        private void InitializeStateTree(ref ILogicState start)
        {
            var idleLogic = _factory.CreateLogic<IdleLogicState>();
            var attackLogic = _factory.CreateLogic<DefeatTargetLogicState>();
            var roamingLogic = _factory.CreateLogic<RoamingLogicState>();

            start = roamingLogic;

            _transitions.Add(roamingLogic, new LogicTransition[] {
                new LogicTransition(_factory.CreateTrigger< IntruderDetectedTrigger>(), attackLogic),
                new LogicTransition(_factory.CreateTrigger< LogicOverTrigger>(), idleLogic)
            });

            _transitions.Add(attackLogic, new LogicTransition[] {
                new LogicTransition(_factory.CreateTrigger<CounterOverTrigger>(), attackLogic),
                new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), idleLogic)
            });

            _transitions.Add(idleLogic, new LogicTransition[] {
                new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), attackLogic),
                new LogicTransition(_factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)
            });
        }
    }
}
