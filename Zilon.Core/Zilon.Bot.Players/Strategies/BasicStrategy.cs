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
            // Для текущего состояния проверяем каждый из переходов.
            // Если переход выстреливает, то генерируем задачу актёру.
            // Логика может вернуть нулевую задачу, когда дальнейшее исполнение невозможно.
            // Если задача актёра не нулевая, то считаем логику выстрелившего перехода текущей
            // и дальше работаем с ней.

            var currentStateTransitions = _transitions[CurrentState];
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

        private void InitializeStateTree(ref ILogicState start)
        {
            var roamingLogic = _factory.CreateLogic<RoamingLogicState>();
            var roamingIdleLogic = _factory.CreateLogic<IdleLogicState>();
            var fightLogic = _factory.CreateLogic<DefeatTargetLogicState>();
            var fightIdleLogic = _factory.CreateLogic<IdleLogicState>();
            

            start = roamingLogic;

            _transitions.Add(roamingLogic, new LogicTransition[] {
                new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), roamingIdleLogic)
            });

            _transitions.Add(fightLogic, new LogicTransition[] {
                new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                // После победы над текущим противником отдыхаем
                new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), fightIdleLogic)
            });

            _transitions.Add(roamingIdleLogic, new LogicTransition[] {
                new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                new LogicTransition(_factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)
            });

            _transitions.Add(fightIdleLogic, new LogicTransition[] {
                new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                new LogicTransition(_factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)
            });
        }
    }
}
