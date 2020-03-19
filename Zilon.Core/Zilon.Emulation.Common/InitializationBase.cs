using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Core.Client;
using Zilon.Core.CommonServices;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.ScoreResultGenerating;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;

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
        }

        public abstract void ConfigureAux(IServiceProvider serviceFactory);

        protected virtual void RegisterMonsterGeneratorRandomSource(IServiceCollection serviceRegistry)
        {
            serviceRegistry.AddScoped<IMonsterGenerator, MonsterGenerator>();
            serviceRegistry.AddSingleton<IMonsterGeneratorRandomSource, MonsterGeneratorRandomSource>();
        }

        protected virtual void RegisterChestGeneratorRandomSource(IServiceCollection serviceRegistry)
        {
            serviceRegistry.AddScoped<IChestGenerator, ChestGenerator>();
            serviceRegistry.AddSingleton<IChestGeneratorRandomSource, ChestGeneratorRandomSource>();
        }

        private void RegisterSectorServices(IServiceCollection serviceRegistry)
        {
            RegisterClientServices(serviceRegistry);
            RegisterGameLoop(serviceRegistry);
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
            RegisterMonsterGeneratorRandomSource(container);
            RegisterChestGeneratorRandomSource(container);
            container.AddScoped<ICitizenGenerator, CitizenGenerator>();
            container.AddScoped<ISectorFactory, SectorFactory>();
            container.AddScoped<ISectorManager, InfiniteSectorManager>();
            container.AddScoped<IActorManager, ActorManager>();
            container.AddScoped<IPropContainerManager, PropContainerManager>();
            container.AddScoped<ITacticalActUsageService>(serviceProvider =>
            {
                var randomSource = serviceProvider.GetRequiredService<ITacticalActUsageRandomSource>();
                var perkResolver = serviceProvider.GetRequiredService<IPerkResolver>();
                var sectorManager = serviceProvider.GetRequiredService<ISectorManager>();

                var tacticalActUsageService = new TacticalActUsageService(randomSource, perkResolver, sectorManager);

                ConfigurateTacticalActUsageService(serviceProvider, tacticalActUsageService);

                return tacticalActUsageService;
            });
            container.AddScoped<MonsterBotActorTaskSource>();
        }

        private static void ConfigurateTacticalActUsageService(IServiceProvider serviceProvider, TacticalActUsageService tacticalActUsageService)
        {
            // Указание необязательных зависимостей
            tacticalActUsageService.EquipmentDurableService = serviceProvider.GetService<IEquipmentDurableService>();
            tacticalActUsageService.PlayerEventLogService = serviceProvider.GetService<IPlayerEventLogService>();
        }

        private static void RegisterGameLoop(IServiceCollection serviceRegistry)
        {
            serviceRegistry.AddScoped<IGameLoop, GameLoop>();
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
            container.AddSingleton<IHumanPersonFactory, RandomHumanPersonFactory>();
            container.AddSingleton<IPersonPerkInitializator, PersonPerkInitializator>();

            container.AddSingleton<IMapFactorySelector, SwitchMapFactorySelector>();
            container.AddSingleton<RoomMapFactory>();
            container.AddSingleton<IRoomGenerator, RoomGenerator>();
            container.AddSingleton(CreateRoomGeneratorRandomSource);
            container.AddSingleton<CellularAutomatonMapFactory>();
            container.AddSingleton<IInteriorObjectRandomSource, InteriorObjectRandomSource>();
            container.AddSingleton<ICitizenGeneratorRandomSource, CitizenGeneratorRandomSource>();

            container.AddSingleton<IUserTimeProvider, UserTimeProvider>();
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
            serviceCollection.AddSingleton<HumanPlayer>();
            serviceCollection.AddSingleton<IBotPlayer, BotPlayer>();
        }

        protected abstract void RegisterBot(IServiceCollection serviceCollection);
    }
}
