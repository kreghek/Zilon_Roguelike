using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Core.Client;
using Zilon.Core.CommonServices;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.MapGenerators.StaticObjectFactories;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.ScoreResultGenerating;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.World;

namespace Zilon.Emulation.Common
{
    public abstract class InitializationBase
    {
        public int? DiceSeed { get; set; }

        protected InitializationBase()
        {
        }

        protected InitializationBase(int diceSeed)
        {
            DiceSeed = diceSeed;
        }

        public virtual void RegisterServices(IServiceCollection serviceCollection)
        {
            RegisterSchemeService(serviceCollection);
            RegisterAuxServices(serviceCollection);
            RegisterPlayerServices(serviceCollection);

            RegisterSectorServices(serviceCollection);

            RegisterGlobeInitializationServices(serviceCollection);
        }

        public abstract void ConfigureAux(IServiceProvider serviceFactory);

        private static void RegisterGlobeInitializationServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IGlobeInitializer, GlobeInitializer>();
            serviceCollection.AddSingleton<IBiomeInitializer, BiomeInitializer>();
            serviceCollection.AddSingleton<IBiomeSchemeRoller, BiomeSchemeRoller>();
            serviceCollection.AddSingleton<IGlobeTransitionHandler, GlobeTransitionHandler>();
            serviceCollection.AddSingleton<IPersonInitializer, HumanPersonInitializer>();
            serviceCollection.AddSingleton<IGlobeExpander>(serviceProvider =>
            {
                return (BiomeInitializer)serviceProvider.GetRequiredService<IBiomeInitializer>();
            });
        }

        protected virtual void RegisterMonsterGeneratorRandomSource(IServiceCollection serviceRegistry)
        {
            serviceRegistry.AddSingleton<IMonsterGenerator, MonsterGenerator>(serviceProvider =>
            {
                var schemeService = serviceProvider.GetRequiredService<ISchemeService>();
                var monsterFactory = serviceProvider.GetRequiredService<IMonsterPersonFactory>();
                var randomSource = serviceProvider.GetRequiredService<IMonsterGeneratorRandomSource>();
                var actorTaskSource = serviceProvider.GetRequiredService<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();

                var generator = new MonsterGenerator(schemeService, monsterFactory, randomSource, actorTaskSource);
                return generator;
            });
            serviceRegistry.AddSingleton<IMonsterPersonFactory, MonsterPersonFactory>();
            serviceRegistry.AddSingleton<IMonsterGeneratorRandomSource, MonsterGeneratorRandomSource>();
        }

        protected virtual void RegisterChestGeneratorRandomSource(IServiceCollection serviceRegistry)
        {
            serviceRegistry.AddScoped<IStaticObstaclesGenerator, StaticObstaclesGenerator>();
            serviceRegistry.AddSingleton<IInteriorObjectRandomSource, InteriorObjectRandomSource>();
            serviceRegistry.AddScoped<IChestGenerator, ChestGenerator>();
            serviceRegistry.AddSingleton<IChestGeneratorRandomSource, ChestGeneratorRandomSource>();
            serviceRegistry.AddSingleton<IStaticObjectFactoryCollector>(diFactory =>
            {
                var factories = diFactory.GetServices<IStaticObjectFactory>().ToArray();
                return new StaticObjectFactoryCollector(factories);
            });
            serviceRegistry.AddSingleton<IStaticObjectFactory, StoneDepositFactory>();
            serviceRegistry.AddSingleton<IStaticObjectFactory, OreDepositFactory>();
            serviceRegistry.AddSingleton<IStaticObjectFactory, TrashHeapFactory>();
            serviceRegistry.AddSingleton<IStaticObjectFactory, CherryBrushFactory>();
            serviceRegistry.AddSingleton<IStaticObjectFactory, PitFactory>();
            serviceRegistry.AddSingleton<IStaticObjectFactory, PuddleFactory>();
            serviceRegistry.AddSingleton<IStaticObjectsGeneratorRandomSource, StaticObjectsGeneratorRandomSource>();
        }

        private void RegisterSectorServices(IServiceCollection serviceRegistry)
        {
            RegisterClientServices(serviceRegistry);
            RegisterScopedSectorService(serviceRegistry);
            RegisterBot(serviceRegistry);
        }

        private static void RegisterSchemeService(IServiceCollection container)
        {
            container.AddSingleton<ISchemeLocator>(factory =>
            {
                //TODO Организовать отдельный общий метод/класс/фабрику для конструирования локатора схем.
                // Подобные конструкции распределены по всему проекту: в тестах, бенчах, окружении ботов.
                // Следует их объединить в одном месте.
                var schemePath = Environment.GetEnvironmentVariable("ZILON_LIV_SCHEME_CATALOG");

                var schemeLocator = new FileSchemeLocator(schemePath);

                return schemeLocator;
            });

            container.AddSingleton<ISchemeService, SchemeService>();

            container.AddSingleton<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>();
        }

        private void RegisterScopedSectorService(IServiceCollection container)
        {
            //TODO сделать генераторы независимыми от сектора.
            // Такое время жизни, потому что в зависимостях есть менеджеры.
            container.AddScoped<ISectorGenerator, SectorGenerator>();
            container.AddSingleton<IBiomeInitializer, BiomeInitializer>();
            container.AddSingleton<IBiomeSchemeRoller, BiomeSchemeRoller>();
            container.AddSingleton<IResourceMaterializationMap, ResourceMaterializationMap>();
            RegisterMonsterGeneratorRandomSource(container);
            RegisterChestGeneratorRandomSource(container);
            container.AddScoped<SectorFactory>();  // TOOD Костфль, чтобы не заполнять конструктор сервиса руками. 
            container.AddScoped<ISectorFactory, SectorFactory>(serviceProvider =>
            {
                var sectorFactory = serviceProvider.GetRequiredService<SectorFactory>();
                var scoreManager = serviceProvider.GetService<IScoreManager>();
                sectorFactory.ScoreManager = scoreManager;
                return sectorFactory;
            });
            RegisterActUsageServices(container);
            container.AddScoped<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();
            container.AddScoped<IActorTaskSourceCollector, ActorTaskSourceCollector>(serviceProvider =>
            {
                var monsterTaskSource = serviceProvider.GetRequiredService<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();
                return new ActorTaskSourceCollector(monsterTaskSource);
            });
        }

        private static void RegisterActUsageServices(IServiceCollection container)
        {
            container.AddScoped<IActUsageHandlerSelector>(serviceProvider =>
            {
                var handlers = serviceProvider.GetServices<IActUsageHandler>();
                var handlersArray = handlers.ToArray();
                var handlerSelector = new ActUsageHandlerSelector(handlersArray);
                return handlerSelector;
            });
            container.AddScoped<IActUsageHandler>(serviceProvider =>
            {
                var perkResolver = serviceProvider.GetRequiredService<IPerkResolver>();
                var randomSource = serviceProvider.GetRequiredService<ITacticalActUsageRandomSource>();
                var handler = new ActorActUsageHandler(perkResolver, randomSource);
                ConfigurateActorActUsageHandler(serviceProvider, handler);
                return handler;
            });
            container.AddScoped<IActUsageHandler, StaticObjectActUsageHandler>();
            container.AddScoped<ITacticalActUsageService>(serviceProvider =>
            {
                var randomSource = serviceProvider.GetRequiredService<ITacticalActUsageRandomSource>();
                var actHandlerSelector = serviceProvider.GetRequiredService<IActUsageHandlerSelector>();

                var tacticalActUsageService = new TacticalActUsageService(randomSource, actHandlerSelector);

                ConfigurateTacticalActUsageService(serviceProvider, tacticalActUsageService);

                return tacticalActUsageService;
            });
        }

        private static void ConfigurateTacticalActUsageService(IServiceProvider serviceProvider, TacticalActUsageService tacticalActUsageService)
        {
            // Указание необязательных зависимостей
            tacticalActUsageService.EquipmentDurableService = serviceProvider.GetService<IEquipmentDurableService>();
        }

        private static void ConfigurateActorActUsageHandler(IServiceProvider serviceProvider, ActorActUsageHandler handler)
        {
            // Указание необязательных зависимостей
            handler.EquipmentDurableService = serviceProvider.GetService<IEquipmentDurableService>();

            handler.ActorInteractionBus = serviceProvider.GetService<IActorInteractionBus>();

            handler.PlayerEventLogService = serviceProvider.GetService<IPlayerEventLogService>();

            handler.ScoreManager = serviceProvider.GetService<IScoreManager>();
        }

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices(IServiceCollection container)
        {
            container.AddSingleton(factory => CreateRandomSeedAndLinearDice());
            container.AddSingleton(factory => CreateRandomSeedAndGaussDice());
            container.AddSingleton(factory => CreateRandomSeedAndExpDice());
            container.AddSingleton<IDice>(factory => factory.GetRequiredService<LinearDice>());

            container.AddSingleton<IDecisionSource, DecisionSource>();
            container.AddSingleton<ITacticalActUsageRandomSource, TacticalActUsageRandomSource>();
            container.AddSingleton<IPerkResolver, PerkResolver>();
            container.AddSingleton<IPropFactory, PropFactory>();
            container.AddSingleton<IDropResolver, DropResolver>();
            container.AddSingleton<IDropResolverRandomSource, DropResolverRandomSource>();
            container.AddSingleton<ISurvivalRandomSource, SurvivalRandomSource>();
            container.AddSingleton<IEquipmentDurableService, EquipmentDurableService>();
            container.AddSingleton<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>();
            RegisterPersonFactory(container);
            container.AddSingleton<IPersonPerkInitializator, PersonPerkInitializator>();

            container.AddSingleton<IMapFactorySelector, SwitchMapFactorySelector>();
            container.AddSingleton<RoomMapFactory>();
            container.AddSingleton<IRoomGenerator, RoomGenerator>();
            container.AddSingleton(CreateRoomGeneratorRandomSource);
            container.AddSingleton<CellularAutomatonMapFactory>();
            container.AddSingleton<IInteriorObjectRandomSource, InteriorObjectRandomSource>();

            container.AddSingleton<IUserTimeProvider, UserTimeProvider>();

            container.AddSingleton<IDiseaseGenerator, DiseaseGenerator>();
        }

        protected virtual void RegisterPersonFactory(IServiceCollection container)
        {
            container.AddSingleton<RandomHumanPersonFactory>(); //TODO Костяль, чтобы не прописывать всё в конструктор
            container.AddSingleton<IPersonFactory, RandomHumanPersonFactory>(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<RandomHumanPersonFactory>();
                factory.PlayerEventLogService = serviceProvider.GetService<IPlayerEventLogService>();
                return factory;
            });
        }

        private static IRoomGeneratorRandomSource CreateRoomGeneratorRandomSource(IServiceProvider factory)
        {
            var localLinearDice = factory.GetRequiredService<LinearDice>();
            var localRoomSizeDice = factory.GetRequiredService<ExpDice>();
            var randomSource = new RoomGeneratorRandomSource(localLinearDice, localRoomSizeDice);
            return randomSource;
        }

        /// <summary>
        /// Создаёт кость и фиксирует зерно рандома.
        /// Если Зерно рандома не задано, то оно выбирается случайно.
        /// </summary>
        /// <returns> Экземпляр кости на основе выбранного или указанного ерна рандома. </returns>
        private LinearDice CreateRandomSeedAndLinearDice()
        {
            LinearDice dice;
            if (DiceSeed == null)
            {
                var diceSeedFact = new Random().Next(int.MaxValue);
                DiceSeed = diceSeedFact;
                dice = new LinearDice(diceSeedFact);
            }
            else
            {
                dice = new LinearDice(DiceSeed.Value);
            }

            return dice;
        }

        /// <summary>
        /// Создаёт кость и фиксирует зерно рандома.
        /// Если Зерно рандома не задано, то оно выбирается случайно.
        /// </summary>
        /// <returns> Экземпляр кости на основе выбранного или указанного ерна рандома. </returns>
        private GaussDice CreateRandomSeedAndGaussDice()
        {
            GaussDice dice;
            if (DiceSeed == null)
            {
                var diceSeedFact = new Random().Next(int.MaxValue);
                DiceSeed = diceSeedFact;
                dice = new GaussDice(diceSeedFact);
            }
            else
            {
                dice = new GaussDice(DiceSeed.Value);
            }

            return dice;
        }

        /// <summary>
        /// Создаёт кость и фиксирует зерно рандома.
        /// Если Зерно рандома не задано, то оно выбирается случайно.
        /// </summary>
        /// <returns> Экземпляр кости на основе выбранного или указанного ерна рандома. </returns>
        private ExpDice CreateRandomSeedAndExpDice()
        {
            ExpDice dice;
            if (DiceSeed == null)
            {
                var diceSeedFact = new Random().Next(int.MaxValue);
                DiceSeed = diceSeedFact;
                dice = new ExpDice(diceSeedFact);
            }
            else
            {
                dice = new ExpDice(DiceSeed.Value);
            }

            return dice;
        }

        private static void RegisterClientServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISectorUiState, SectorUiState>();
            serviceCollection.AddSingleton<IInventoryState, InventoryState>();
        }

        private static void RegisterPlayerServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IScoreManager, ScoreManager>();
            serviceCollection.AddSingleton<IPlayerEventLogService, PlayerEventLogService>();
            serviceCollection.AddSingleton<DeathReasonService>();
            serviceCollection.AddSingleton<IPlayer, HumanPlayer>();
        }

        protected abstract void RegisterBot(IServiceCollection serviceCollection);
    }
}