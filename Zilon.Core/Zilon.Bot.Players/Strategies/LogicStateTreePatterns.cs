using System;
using System.Diagnostics;
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

                Debug.Assert(ValidateTree(tree), "Все используемые логики должны быть добавлены в Transitions, как ключи.");

                return tree;
            }
        }

        public static LogicStateTree Citizen
        {
            get
            {

                var tree = new LogicStateTree();

                var roamingLogic = Factory.CreateLogic<RoamingLogicState>();
                var roamingIdleLogic = Factory.CreateLogic<IdleLogicState>();

                tree.StartState = roamingLogic;

                tree.Transitions.Add(roamingLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), roamingIdleLogic)
                });


                tree.Transitions.Add(roamingIdleLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)
                });

                Debug.Assert(ValidateTree(tree), "Все используемые логики должны быть добавлены в Transitions, как ключи.");

                return tree;
            }
        }

        public static LogicStateTree JoeHumanBot
        {
            get
            {

                var tree = new LogicStateTree();

                var exploreLogic = Factory.CreateLogic<ExploreLogicState>();
                var exploreIdleLogic = Factory.CreateLogic<IdleLogicState>();
                var fightLogic = Factory.CreateLogic<DefeatTargetLogicState>();
                var fightIdleLogic = Factory.CreateLogic<IdleLogicState>();
                var healSelfLogic = Factory.CreateLogic<HealSelfLogicState>();
                var eatProviantLogic = Factory.CreateLogic<EatProviantLogicState>();
                var lootLogic = Factory.CreateLogic<LootLogicState>();
                var exitLogic = Factory.CreateLogic<ExitLogicState>();


                tree.StartState = exploreLogic;

                tree.Transitions.Add(exploreLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LootDetectedTrigger>(), lootLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ExploredTrigger>(), exitLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreIdleLogic)
                });

                tree.Transitions.Add(fightLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    //new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    // После победы над текущим противником отдыхаем
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), fightIdleLogic)
                });

                tree.Transitions.Add(exploreIdleLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<CounterOverTrigger>(), exploreLogic)
                });

                tree.Transitions.Add(fightIdleLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<CounterOverTrigger>(), exploreLogic)
                });

                tree.Transitions.Add(healSelfLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreIdleLogic)
                });

                tree.Transitions.Add(eatProviantLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic)
                });

                tree.Transitions.Add(lootLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                });

                tree.Transitions.Add(exitLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                });

                Debug.Assert(ValidateTree(tree), "Все используемые логики должны быть добавлены в Transitions, как ключи.");

                return tree;
            }
        }

        public static LogicStateTree DuncanHumanBot
        {
            get
            {

                var tree = new LogicStateTree();

                var exploreLogic = Factory.CreateLogic<ExploreLogicState>();
                var fightLogic = Factory.CreateLogic<DefeatTargetLogicState>();
                var healSelfLogic = Factory.CreateLogic<HealSelfLogicState>();
                var eatProviantLogic = Factory.CreateLogic<EatProviantLogicState>();
                var lootLogic = Factory.CreateLogic<LootLogicState>();
                var equipLogic = Factory.CreateLogic<EquipBetterPropLogicState>();
                var exitLogic = Factory.CreateLogic<ExitLogicState>();


                tree.StartState = exploreLogic;

                tree.Transitions.Add(exploreLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LootDetectedTrigger>(), lootLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<HasBetterEquipmentTrigger>(), equipLogic),
                    new LogicTransition(Factory.CreateTrigger<ExploredTrigger>(), exitLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                });

                tree.Transitions.Add(fightLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                });

                tree.Transitions.Add(healSelfLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                });

                tree.Transitions.Add(eatProviantLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreLogic),
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic)
                });

                tree.Transitions.Add(lootLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                });

                tree.Transitions.Add(equipLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                });

                tree.Transitions.Add(exitLogic, new LogicTransition[] {
                    new LogicTransition(Factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    new LogicTransition(Factory.CreateTrigger<HungryAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<ThirstAndHasResourceTrigger>(), eatProviantLogic),
                    new LogicTransition(Factory.CreateTrigger<HasBetterEquipmentTrigger>(), equipLogic),
                    new LogicTransition(Factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                });

                Debug.Assert(ValidateTree(tree), "Все используемые логики должны быть добавлены в Transitions, как ключи.");

                return tree;
            }
        }

        private static bool ValidateTree(LogicStateTree tree)
        {
            foreach (var item in tree.Transitions)
            {
                foreach (var transition in item.Value)
                {
                    if (!tree.Transitions.ContainsKey(item.Key))
                    {
                        // TODO Выбрасывать более конкретный тип исключения
                        throw new Exception($"{item} {transition} ссылается на несуществующую логику");
                    }
                }
            }

            return true;
        }

        public static LogicStateTree DefaultHumanBot => JoeHumanBot;
    }
}
