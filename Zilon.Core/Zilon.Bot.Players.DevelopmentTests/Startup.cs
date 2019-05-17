using System.Configuration;

using LightInject;

using Zilon.Bot.Players.LightInject;
using Zilon.Bot.Players.LightInject.DependencyInjection;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Bot.Players.DevelopmentTests
{
    class Startup
    {
        public void RegisterGlobalServices(IServiceRegistry serviceRegistry)
        {
            RegisterSchemeService(serviceRegistry);
            RegisterAuxServices(serviceRegistry);
            RegisterPlayerServices(serviceRegistry);

            RegisterSectorServices(serviceRegistry);
        }

        private void RegisterSectorServices(IServiceRegistry serviceRegistry)
        {
            RegisterClientServices(serviceRegistry);
            RegisterGameLoop(serviceRegistry);
            RegisterSectorService(serviceRegistry);
            RegisterBot(serviceRegistry);
        }

        public void ConfigureAux(IServiceFactory serviceFactory)
        {
            LogicStateTreePatterns.Factory = serviceFactory.GetInstance<ILogicStateFactory>();
        }

        private void RegisterSchemeService(IServiceRegistry container)
        {
            container.Register<ISchemeLocator>(factory =>
            {
                var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

                var schemeLocator = new FileSchemeLocator(schemePath);

                return schemeLocator;
            }, new PerContainerLifetime());

            container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());

            container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
        }

        private void RegisterSectorService(IServiceRegistry container)
        {
            container.Register<IMapFactory, RoomMapFactory>(new PerScopeLifetime());
            container.Register<ISectorGenerator, SectorGenerator>(new PerScopeLifetime());
            container.Register<ISectorFactory, SectorFactory>(new PerScopeLifetime());
            container.Register<ISectorManager, InfiniteSectorManager>(new PerScopeLifetime());
            container.Register<IActorManager, ActorManager>(new PerScopeLifetime());
            container.Register<IPropContainerManager, PropContainerManager>(new PerScopeLifetime());
            container.Register<ITraderManager, TraderManager>(new PerScopeLifetime());
            container.Register<IRoomGenerator, RoomGenerator>(new PerScopeLifetime());
            container.Register<IRoomGeneratorRandomSource, RoomGeneratorRandomSource>(new PerScopeLifetime());
            container.Register<IScoreManager, ScoreManager>(new PerScopeLifetime());
            container.Register<IMonsterGenerator, MonsterGenerator>(new PerScopeLifetime());
            container.Register<IMonsterGeneratorRandomSource, MonsterGeneratorRandomSource>(new PerScopeLifetime());
            container.Register<IChestGenerator, ChestGenerator>(new PerScopeLifetime());
            container.Register<IChestGeneratorRandomSource, ChestGeneratorRandomSource>(new PerScopeLifetime());

            container.Register<IActorTaskSource, MonsterActorTaskSource>("monster", new PerScopeLifetime());
        }

        private void RegisterGameLoop(IServiceRegistry container)
        {
            container.Register<IGameLoop, GameLoop>(new PerScopeLifetime());
        }

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices(IServiceRegistry container)
        {
            var dice = new Dice();
            container.Register<IDice>(factory => dice, new PerContainerLifetime());

            container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
            container.Register<ITacticalActUsageRandomSource, TacticalActUsageRandomSource>(new PerContainerLifetime());

            container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());
            container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
            container.Register<IDropResolver, DropResolver>(new PerContainerLifetime());
            container.Register<IDropResolverRandomSource, DropResolverRandomSource>(new PerContainerLifetime());
            container.Register<ISurvivalRandomSource, SurvivalRandomSource>(new PerContainerLifetime());

            container.Register<IEquipmentDurableService, EquipmentDurableService>(new PerContainerLifetime());
            container.Register<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>(new PerContainerLifetime());
        }

        private void RegisterClientServices(IServiceRegistry container)
        {
            container.Register<ISectorUiState, SectorUiState>(new PerScopeLifetime());
            container.Register<IInventoryState, InventoryState>(new PerScopeLifetime());
        }

        private void RegisterPlayerServices(IServiceRegistry container)
        {
            container.Register<IScoreManager, ScoreManager>(new PerContainerLifetime());
            container.Register<HumanPlayer>(new PerContainerLifetime());
            container.Register<IBotPlayer, BotPlayer>(new PerContainerLifetime());
        }

        private void RegisterBot(IServiceRegistry container)
        {
            container.RegisterLogicState();
            container.Register<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory),
                new PerScopeLifetime());
            container.Register<IActorTaskSource, BotActorTaskSource>("bot", new PerScopeLifetime());
        }
    }
}
