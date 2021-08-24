﻿using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.OpenStyle;
using Zilon.Core.MapGenerators.RoomStyle;
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
using Zilon.DependencyInjection;

namespace Zilon.Emulation.Common
{
    public abstract class InitializationBase
    {
        protected InitializationBase()
        {
        }

        protected InitializationBase(int diceSeed)
        {
            DiceSeed = diceSeed;
        }

        public int? DiceSeed { get; set; }

        public abstract void ConfigureAux(IServiceProvider serviceFactory);

        public virtual void RegisterServices(IServiceCollection serviceCollection)
        {
            RegisterSchemeService(serviceCollection);
            RegisterAuxServices(serviceCollection);
            RegisterPlayerServices(serviceCollection);

            RegisterSectorServices(serviceCollection);

            RegisterGlobeInitializationServices(serviceCollection);
        }

        protected abstract void RegisterBot(IServiceCollection serviceCollection);

        protected virtual void RegisterChestGeneratorRandomSource(IServiceCollection serviceRegistry)
        {
            serviceRegistry.AddScoped<StaticObjectGenerator>();
            serviceRegistry.AddScoped<IStaticObjectsGenerator, StaticObjectGenerator>(serviceProvider =>
            {
                var service = serviceProvider.GetRequiredService<StaticObjectGenerator>();
                service.MonsterIdentifierGenerator = serviceProvider.GetService<IMonsterIdentifierGenerator>();
                return service;
            });
            serviceRegistry.AddSingleton<IInteriorObjectRandomSource, InteriorObjectRandomSource>();
            serviceRegistry.AddScoped<IChestGenerator, ChestGenerator>();
            serviceRegistry.AddSingleton<IChestGeneratorRandomSource, ChestGeneratorRandomSource>();

            serviceRegistry.RegisterStaticObjectFactoringFromCore();

            serviceRegistry.AddSingleton<IStaticObjectsGeneratorRandomSource, StaticObjectsGeneratorRandomSource>();
        }

        protected virtual void RegisterMonsterGeneratorRandomSource(IServiceCollection serviceRegistry)
        {
            serviceRegistry.AddSingleton<IMonsterGenerator, MonsterGenerator>(serviceProvider =>
            {
                var schemeService = serviceProvider.GetRequiredService<ISchemeService>();
                var monsterFactory = serviceProvider.GetRequiredService<IMonsterPersonFactory>();
                var randomSource = serviceProvider.GetRequiredService<IMonsterGeneratorRandomSource>();
                var actorTaskSource =
                    serviceProvider.GetRequiredService<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();

                var generator = new MonsterGenerator(schemeService, monsterFactory, randomSource, actorTaskSource);
                return generator;
            });
            serviceRegistry.AddSingleton<IMonsterPersonFactory, MonsterPersonFactory>(serviceProvider =>
            {
                var identifierGenerator = serviceProvider.GetService<IMonsterIdentifierGenerator>();
                return new MonsterPersonFactory
                {
                    MonsterIdentifierGenerator = identifierGenerator
                };
            });
            serviceRegistry.AddSingleton<IMonsterGeneratorRandomSource, MonsterGeneratorRandomSource>();
        }

        protected virtual void RegisterPersonFactory<TPersonfactory>(IServiceCollection container)
            where TPersonfactory : class, IPersonFactory
        {
            container.AddSingleton<TPersonfactory>(); //TODO Костяль, чтобы не прописывать всё в конструктор
            container.AddSingleton<IPersonFactory, TPersonfactory>(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<TPersonfactory>();
                factory.PlayerEventLogService = serviceProvider.GetService<IPlayerEventLogService>();
                return factory;
            });
        }

        protected virtual void RegisterSchemeService(IServiceCollection container)
        {
            container.AddSingleton<ISchemeLocator>(factory =>
            {
                var schemeLocator = FileSchemeLocator.CreateFromEnvVariable();

                return schemeLocator;
            });

            container.AddSingleton<ISchemeService, SchemeService>();

            container.AddSingleton<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>();
        }

        private static void ConfigurateActorActUsageHandler(IServiceProvider serviceProvider,
            ActorActUsageHandler handler)
        {
            // Указание необязательных зависимостей
            handler.EquipmentDurableService = serviceProvider.GetService<IEquipmentDurableService>();

            handler.ActorInteractionBus = serviceProvider.GetService<IActorInteractionBus>();

            handler.PlayerEventLogService = serviceProvider.GetService<IPlayerEventLogService>();

            handler.ScoreManager = serviceProvider.GetService<IScoreManager>();
        }

        private static void ConfigurateTacticalActUsageService(IServiceProvider serviceProvider,
            TacticalActUsageService tacticalActUsageService)
        {
            // Указание необязательных зависимостей
            tacticalActUsageService.EquipmentDurableService = serviceProvider.GetService<IEquipmentDurableService>();
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

        private static IRoomGeneratorRandomSource CreateRoomGeneratorRandomSource(IServiceProvider factory)
        {
            var localLinearDice = factory.GetRequiredService<LinearDice>();
            var localRoomSizeDice = factory.GetRequiredService<ExpDice>();
            var randomSource = new RoomGeneratorRandomSource(localLinearDice, localRoomSizeDice);
            return randomSource;
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
            RegisterPersonFactory<TemplateBasedPersonFactory>(container);
            container.AddSingleton<IPersonPerkInitializator, PersonPerkInitializator>();

            container.AddSingleton<IMapFactorySelector, SwitchMapFactorySelector>();
            container.AddSingleton<RoomMapFactory>();
            container.AddSingleton<IRoomGenerator, RoomGenerator>();
            container.AddSingleton(CreateRoomGeneratorRandomSource);
            container.AddSingleton<CellularAutomatonMapFactory>();
            container.AddSingleton<OpenMapFactory>();
            container.AddSingleton<IInteriorObjectRandomSource, InteriorObjectRandomSource>();

            container.AddSingleton<IUserTimeProvider, UserTimeProvider>();

            container.AddSingleton<IDiseaseGenerator, DiseaseGenerator>();
        }

        private static void RegisterClientServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISectorUiState, SectorUiState>();
            serviceCollection.AddScoped<IInventoryState, InventoryState>();
            serviceCollection.AddScoped<IAnimationBlockerService, AnimationBlockerService>();

            serviceCollection.AddScoped<ICommandLoopContext, CommandLoopContext>();
            serviceCollection.AddScoped<ICommandLoopUpdater, CommandLoopUpdater>();
            serviceCollection.AddScoped<ICommandPool, QueueCommandPool>();
        }

        private static void RegisterGlobeInitializationServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IGlobeInitializer, GlobeInitializer>();
            serviceCollection.AddSingleton<IBiomeInitializer, BiomeInitializer>();
            serviceCollection.AddSingleton<IBiomeSchemeRoller, BiomeSchemeRoller>();
            serviceCollection.AddSingleton<IGlobeTransitionHandler, GlobeTransitionHandler>();
            serviceCollection.AddSingleton<IPersonInitializer, HumanPersonInitializer>();
            serviceCollection.AddSingleton<ITransitionPool, TransitionPool>();
            serviceCollection.AddSingleton<IGlobeExpander>(serviceProvider =>
            {
                return (BiomeInitializer)serviceProvider.GetRequiredService<IBiomeInitializer>();
            });

            serviceCollection.AddSingleton<IGlobeLoopUpdater, GlobeLoopUpdater>();
            serviceCollection.AddSingleton<IGlobeLoopContext, GlobeLoopContext>();
        }

        private static void RegisterPlayerServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IScoreManager, ScoreManager>();
            serviceCollection.AddSingleton<IPlayerEventLogService, PlayerEventLogService>();
            serviceCollection.AddSingleton<IDeathReasonService, DeathReasonService>();
            serviceCollection.AddSingleton<IPlayer, HumanPlayer>();
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
            container.AddScoped<SectorFactory>(); // TOOD Костфль, чтобы не заполнять конструктор сервиса руками. 
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
                var monsterTaskSource =
                    serviceProvider.GetRequiredService<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();
                return new ActorTaskSourceCollector(monsterTaskSource);
            });
        }

        private void RegisterSectorServices(IServiceCollection serviceRegistry)
        {
            RegisterClientServices(serviceRegistry);
            RegisterScopedSectorService(serviceRegistry);
            RegisterBot(serviceRegistry);
        }
    }
}