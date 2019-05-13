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
        private readonly Dictionary<object, ILogicState> _logicStates;

        private readonly Dictionary<ILogicState, IEnumerable<LogicTransition>> _transitions;
        private readonly ILogicState _startState;

        public BasicStrategy(IActor actor)
        {
            _logicStates = new Dictionary<object, ILogicState>();

            Actor = actor;

            InitializeStateTree(ref _startState);

            CurrentState = StartState;
        }

        public IActor Actor { get; }
        public ILogicState StartState => _startState;
        public ILogicState CurrentState { get; private set; }

        public IActorTask GetActorTask()
        {
            throw new NotImplementedException();
        }

        private void InitializeStateTree(ref ILogicState start)
        {
            var idleLogic = new IdleLogicState();
            var attackLogic = new DefeatTargetLogicState();
            var roamingLogic = new RoamingLogicState();

            start = roamingLogic;

            _transitions.Add(roamingLogic, new LogicTransition[] {
                new LogicTransition(new IntruderDetectedTrigger(), attackLogic),
                new LogicTransition(new LogicOverTrigger(), idleLogic)
            });

            _transitions.Add(attackLogic, new LogicTransition[] {
                new LogicTransition(new CounterOverTrigger(), attackLogic),
                new LogicTransition(new LogicOverTrigger(), idleLogic)
            });

            _transitions.Add(idleLogic, new LogicTransition[] {
                new LogicTransition(new IntruderDetectedTrigger(), attackLogic),
                new LogicTransition(new CounterOverTrigger(), roamingLogic)
            });
        }
    }
}
