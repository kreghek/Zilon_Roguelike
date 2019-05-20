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
                    //new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
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

        public static LogicStateTree JoeHumanBot
        {
            get
            {

                var tree = new LogicStateTree();

                var roamingLogic = Factory.CreateLogic<RoamingLogicState>();
                var roamingIdleLogic = Factory.CreateLogic<IdleLogicState>();
                var fightLogic = Factory.CreateLogic<DefeatTargetLogicState>();
                var fightIdleLogic = Factory.CreateLogic<IdleLogicState>();
                var healSelfLogic = Factory.CreateLogic<HealSelfLogicState>();
                var eatProviantLogic = Factory.CreateLogic<EatProviantLogicState>();
                var lootLogic = Factory.CreateLogic<LootLogicState>();


                tree.StartState = roamingLogic;

                tree.Transitions.Add(roamingLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LootDetectedTrigger>(), lootLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), roamingIdleLogic)
                });

                tree.Transitions.Add(fightLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    //new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    // После победы над текущим противником отдыхаем
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), fightIdleLogic)
                });

                tree.Transitions.Add(roamingIdleLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)
                });

                tree.Transitions.Add(fightIdleLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)
                });

                tree.Transitions.Add(healSelfLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), roamingIdleLogic)
                });

                tree.Transitions.Add(eatProviantLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), roamingLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic)
                });

                tree.Transitions.Add(lootLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), roamingLogic)
                });

                return tree;
            }
        }

        public static LogicStateTree DuncanHumanBot
        {
            get
            {

                var tree = new LogicStateTree();

                var roamingLogic = Factory.CreateLogic<RoamingLogicState>();
                var fightLogic = Factory.CreateLogic<DefeatTargetLogicState>();
                var healSelfLogic = Factory.CreateLogic<HealSelfLogicState>();
                var eatProviantLogic = Factory.CreateLogic<EatProviantLogicState>();
                var lootLogic = Factory.CreateLogic<LootLogicState>();


                tree.StartState = roamingLogic;

                tree.Transitions.Add(roamingLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LootDetectedTrigger>(), lootLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                });

                tree.Transitions.Add(fightLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    //new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                });

                tree.Transitions.Add(healSelfLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                });

                tree.Transitions.Add(eatProviantLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), roamingLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic)
                });

                tree.Transitions.Add(lootLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), roamingLogic)
                });

                return tree;
            }
        }

        public static LogicStateTree DefaultHumanBot => JoeHumanBot;
    }
}
