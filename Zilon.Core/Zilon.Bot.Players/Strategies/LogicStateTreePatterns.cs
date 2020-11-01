using Zilon.Bot.Players.Logics;
using Zilon.Bot.Players.Triggers;

namespace Zilon.Bot.Players.Strategies
{
    public class LogicStateTreePatterns
    {
        private readonly ILogicStateFactory _factory;

        public LogicStateTreePatterns(ILogicStateFactory factory)
        {
            _factory = factory;
        }

        public LogicStateTree Monster
        {
            get
            {
                LogicStateTree tree = new LogicStateTree();

                ILogicState roamingLogic = _factory.CreateLogic<RoamingLogicState>();
                ILogicState roamingIdleLogic = _factory.CreateLogic<IdleLogicState>();
                ILogicState fightLogic = _factory.CreateLogic<DefeatTargetLogicState>();
                ILogicState fightIdleLogic = _factory.CreateLogic<IdleLogicState>();

                tree.StartState = roamingLogic;

                tree.Transitions.Add(roamingLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), roamingIdleLogic)
                    });

                tree.Transitions.Add(fightLogic, new[]
                {
                    //new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    // После победы над текущим противником отдыхаем
                    new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), fightIdleLogic)
                });

                tree.Transitions.Add(roamingIdleLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)
                    });

                tree.Transitions.Add(fightIdleLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)
                    });

                Debug.Assert(ValidateTree(tree),
                    "Все используемые логики должны быть добавлены в Transitions, как ключи.");

                return tree;
            }
        }

        public LogicStateTree Citizen
        {
            get
            {
                LogicStateTree tree = new LogicStateTree();

                ILogicState roamingLogic = _factory.CreateLogic<RoamingLogicState>();
                ILogicState roamingIdleLogic = _factory.CreateLogic<IdleLogicState>();

                tree.StartState = roamingLogic;

                tree.Transitions.Add(roamingLogic,
                    new[] {new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), roamingIdleLogic)});

                tree.Transitions.Add(roamingIdleLogic,
                    new[] {new LogicTransition(_factory.CreateTrigger<CounterOverTrigger>(), roamingLogic)});

                Debug.Assert(ValidateTree(tree),
                    "Все используемые логики должны быть добавлены в Transitions, как ключи.");

                return tree;
            }
        }

        public LogicStateTree JoeHumanBot
        {
            get
            {
                LogicStateTree tree = new LogicStateTree();

                ILogicState exploreLogic = _factory.CreateLogic<ExploreLogicState>();
                ILogicState exploreIdleLogic = _factory.CreateLogic<IdleLogicState>();
                ILogicState fightLogic = _factory.CreateLogic<DefeatTargetLogicState>();
                ILogicState fightIdleLogic = _factory.CreateLogic<IdleLogicState>();
                ILogicState healSelfLogic = _factory.CreateLogic<HealSelfLogicState>();
                ILogicState eatProviantLogic = _factory.CreateLogic<EatProviantLogicState>();
                ILogicState lootLogic = _factory.CreateLogic<LootLogicState>();
                ILogicState exitLogic = _factory.CreateLogic<ExitLogicState>();

                tree.StartState = exploreLogic;

                tree.Transitions.Add(exploreLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<LootDetectedTrigger>(), lootLogic),
                        new LogicTransition(_factory.CreateTrigger<HungryAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<ThirstAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<ExploredTrigger>(), exitLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreIdleLogic)
                    });

                tree.Transitions.Add(fightLogic, new[]
                {
                    new LogicTransition(_factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                    //new LogicTransition(Factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                    // После победы над текущим противником отдыхаем
                    new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), fightIdleLogic)
                });

                tree.Transitions.Add(exploreIdleLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<HungryAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<ThirstAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<CounterOverTrigger>(), exploreLogic)
                    });

                tree.Transitions.Add(fightIdleLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<HungryAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<ThirstAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<CounterOverTrigger>(), exploreLogic)
                    });

                tree.Transitions.Add(healSelfLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<HungryAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<ThirstAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreIdleLogic)
                    });

                tree.Transitions.Add(eatProviantLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreLogic),
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic)
                    });

                tree.Transitions.Add(lootLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                    });

                tree.Transitions.Add(exitLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                        new LogicTransition(_factory.CreateTrigger<HungryAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<ThirstAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                    });

                Debug.Assert(ValidateTree(tree),
                    "Все используемые логики должны быть добавлены в Transitions, как ключи.");

                return tree;
            }
        }

        public LogicStateTree DuncanHumanBot
        {
            get
            {
                LogicStateTree tree = new LogicStateTree();

                ILogicState exploreLogic = _factory.CreateLogic<ExploreLogicState>();
                ILogicState fightLogic = _factory.CreateLogic<DefeatTargetLogicState>();
                ILogicState healSelfLogic = _factory.CreateLogic<HealSelfLogicState>();
                ILogicState eatProviantLogic = _factory.CreateLogic<EatProviantLogicState>();
                ILogicState lootLogic = _factory.CreateLogic<LootLogicState>();
                ILogicState equipLogic = _factory.CreateLogic<EquipBetterPropLogicState>();
                ILogicState exitLogic = _factory.CreateLogic<ExitLogicState>();


                tree.StartState = exploreLogic;

                tree.Transitions.Add(exploreLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<LootDetectedTrigger>(), lootLogic),
                        new LogicTransition(_factory.CreateTrigger<HungryAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<ThirstAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<HasBetterEquipmentTrigger>(), equipLogic),
                        new LogicTransition(_factory.CreateTrigger<ExploredTrigger>(), exitLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                    });

                tree.Transitions.Add(fightLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                    });

                tree.Transitions.Add(healSelfLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<HungryAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<ThirstAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                    });

                tree.Transitions.Add(eatProviantLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreLogic),
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic)
                    });

                tree.Transitions.Add(lootLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<HungryAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<ThirstAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                    });

                tree.Transitions.Add(equipLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<IntruderDetectedTrigger>(), fightLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                    });

                tree.Transitions.Add(exitLogic,
                    new[]
                    {
                        new LogicTransition(_factory.CreateTrigger<LowHpAndHasResourceTrigger>(), healSelfLogic),
                        new LogicTransition(_factory.CreateTrigger<HungryAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<ThirstAndHasResourceTrigger>(),
                            eatProviantLogic),
                        new LogicTransition(_factory.CreateTrigger<HasBetterEquipmentTrigger>(), equipLogic),
                        new LogicTransition(_factory.CreateTrigger<LogicOverTrigger>(), exploreLogic)
                    });

                Debug.Assert(ValidateTree(tree),
                    "Все используемые логики должны быть добавлены в Transitions, как ключи.");

                return tree;
            }
        }

        public LogicStateTree DefaultHumanBot => JoeHumanBot;

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
    }
}