using Zilon.Bot.Players.Logics;
using Zilon.Bot.Players.Triggers;

namespace Zilon.Bot.Players.Strategies
{
    public sealed class LogicStateTreePatterns
    {
        public static ILogicStateFactory Factory;

        public static LogicStateTree Monster
        {
            get
            {

                var tree = new LogicStateTree();

                var roamingLogic = Factory.CreateLogic<RoamingLogicState>();
                var roamingIdleLogic = Factory.CreateLogic<IdleLogicState>();
                var fightLogic = Factory.CreateLogic<DefeatTargetLogicState>();
                var fightIdleLogic = Factory.CreateLogic<IdleLogicState>();


                tree.StartState = roamingLogic;

                tree.Transitions.Add(roamingLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), roamingIdleLogic)
                });

                tree.Transitions.Add(fightLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    // После победы над текущим противником отдыхаем
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), fightIdleLogic)
                });

                tree.Transitions.Add(roamingIdleLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)
                });

                tree.Transitions.Add(fightIdleLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)
                });

                return tree;
            }
        }

        public static LogicStateTree HumanBot
        {
            get
            {

                var tree = new LogicStateTree();

                var roamingLogic = Factory.CreateLogic<RoamingLogicState>();
                var roamingIdleLogic = Factory.CreateLogic<IdleLogicState>();
                var fightLogic = Factory.CreateLogic<DefeatTargetLogicState>();
                var fightIdleLogic = Factory.CreateLogic<IdleLogicState>();
                var healSelfLogic = Factory.CreateLogic<HealSelfLogicState>();


                tree.StartState = roamingLogic;

                tree.Transitions.Add(roamingLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), roamingIdleLogic),
                    new LogicTransition(Factory.CreateTrigger<LowHpTrigger>(), healSelfLogic)
                });

                tree.Transitions.Add(fightLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    // После победы над текущим противником отдыхаем
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), fightIdleLogic),
                    new LogicTransition(Factory.CreateTrigger<LowHpTrigger>(), healSelfLogic)
                });

                tree.Transitions.Add(roamingIdleLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<CounterOverTrigger>(), roamingLogic),
                    new LogicTransition(Factory.CreateTrigger<LowHpTrigger>(), healSelfLogic)
                });

                tree.Transitions.Add(fightIdleLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<CounterOverTrigger>(), roamingLogic),
                    new LogicTransition(Factory.CreateTrigger<LowHpTrigger>(), healSelfLogic)
                });

                return tree;
            }
        }
    }
}
