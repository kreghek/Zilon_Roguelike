using System;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.CommonServices;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Specs.Mocks;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.World;

namespace Zilon.Core.Specs.Contexts
{
    public class RegisterServices
    {
        private ITacticalActUsageRandomSource _specificActUsageRandomSource;

        public IServiceProvider Register()
        {
            var serviceCollection = RegisterServices1();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

        public void SpecifyTacticalActUsageRandomSource(ITacticalActUsageRandomSource actUsageRandomSource)
        {
            _specificActUsageRandomSource = actUsageRandomSource;
        }

        private static void ConfigurateActorActUsageHandler(
            IServiceProvider serviceProvider,
            ActorActUsageHandler handler)
        {
            // Указание необязательных зависимостей
            handler.EquipmentDurableService = serviceProvider.GetService<IEquipmentDurableService>();

            handler.ActorInteractionBus = serviceProvider.GetService<IActorInteractionBus>();

            handler.PlayerEventLogService = serviceProvider.GetService<IPlayerEventLogService>();

            handler.ScoreManager = serviceProvider.GetService<IScoreManager>();
        }

        private static void ConfigurateTacticalActUsageService(
            IServiceProvider serviceProvider,
            TacticalActUsageService tacticalActUsageService)
        {
            // Указание необязательных зависимостей
            tacticalActUsageService.EquipmentDurableService = serviceProvider.GetService<IEquipmentDurableService>();
        }

        private ITacticalActUsageRandomSource CreateActUsageRandomSource(IDice dice)
        {
            if (_specificActUsageRandomSource != null)
            {
                return _specificActUsageRandomSource;
            }

            var actUsageRandomSourceMock =
                new Mock<TacticalActUsageRandomSource>(dice).As<ITacticalActUsageRandomSource>();
            actUsageRandomSourceMock.Setup(x => x.RollEfficient(It.IsAny<Roll>()))
                .Returns<Roll
                >(roll => (roll.Dice / 2) * roll.Count); // Всегда берётся среднее значение среди всех бросков
            actUsageRandomSourceMock.Setup(x => x.RollToHit(It.IsAny<Roll>()))
                .Returns(4);
            actUsageRandomSourceMock.Setup(x => x.RollArmorSave())
                .Returns(4);
            var actUsageRandomSource = actUsageRandomSourceMock.Object;

            return actUsageRandomSource;
        }

        private static ISurvivalRandomSource CreateSurvivalRandomSource()
        {
            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            survivalRandomSourceMock.Setup(x => x.RollSurvival(It.IsAny<SurvivalStat>())).Returns(1);
            survivalRandomSourceMock.Setup(x => x.RollMaxHazardDamage()).Returns(6);

            return survivalRandomSource;
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
        private void RegisterAuxServices(IServiceCollection serviceCollection)
        {
            var dice = new LinearDice(123);
            serviceCollection.AddSingleton<IDice>(factory => dice);

            var decisionSourceMock = new Mock<DecisionSource>(dice).As<IDecisionSource>();
            decisionSourceMock.CallBase = true;
            var decisionSource = decisionSourceMock.Object;

            serviceCollection.AddSingleton(factory => decisionSource);
            serviceCollection.AddSingleton(factory => CreateActUsageRandomSource(dice));

            serviceCollection.AddSingleton<IPerkResolver, PerkResolver>();
            RegisterActUsageServices(serviceCollection);

            serviceCollection.AddSingleton<IPropFactory, PropFactory>();
            serviceCollection.AddSingleton<IDropResolver, DropResolver>();
            serviceCollection.AddSingleton<IDropResolverRandomSource, DropResolverRandomSource>();
            serviceCollection.AddSingleton(factory => CreateSurvivalRandomSource());

            serviceCollection.AddSingleton<IEquipmentDurableService, EquipmentDurableService>();
            serviceCollection.AddSingleton<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>();

            serviceCollection.AddSingleton<IUserTimeProvider, UserTimeProvider>();
        }

        private static void RegisterClientServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISectorUiState, SectorUiState>();
            serviceCollection.AddSingleton<IInventoryState, InventoryState>();
        }

        private static void RegisterCommands(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<MoveCommand>();
            serviceCollection.AddSingleton<NextTurnCommand>();
            serviceCollection.AddSingleton<UseSelfCommand>();
            serviceCollection.AddSingleton<AttackCommand>();

            serviceCollection.AddTransient<PropTransferCommand>();
            serviceCollection.AddTransient<EquipCommand>();
        }

        private static void RegisterGlobeInitializationServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IGlobeInitializer, GlobeInitializer>();
            serviceCollection.AddSingleton<IBiomeInitializer, BiomeInitializer>();
            serviceCollection.AddSingleton<IBiomeSchemeRoller, BiomeSchemeRoller>();
            serviceCollection.AddSingleton<IGlobeTransitionHandler, GlobeTransitionHandler>();
            serviceCollection.AddSingleton<IPersonInitializer, EmptyPersonInitializer>();
            serviceCollection.AddSingleton<IGlobeExpander>(serviceProvider =>
            {
                return (BiomeInitializer)serviceProvider.GetRequiredService<IBiomeInitializer>();
            });
        }

        private static void RegisterPlayerServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPlayer, HumanPlayer>();
            serviceCollection
                .AddSingleton<IActorTaskSource<ISectorTaskSourceContext>,
                    HumanBotActorTaskSource<ISectorTaskSourceContext>>();
            serviceCollection
                .AddSingleton<IHumanActorTaskSource<ISectorTaskSourceContext>,
                    HumanActorTaskSource<ISectorTaskSourceContext>>();
            serviceCollection.AddSingleton<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();
            serviceCollection.AddSingleton<IActorTaskSourceCollector>(serviceProvider =>
            {
                var humanTaskSource =
                    serviceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
                var monsterTaskSource =
                    serviceProvider.GetRequiredService<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();
                return new ActorTaskSourceCollector(humanTaskSource, monsterTaskSource);
            });
            RegisterManager.RegisterBot(serviceCollection);
        }

        private static void RegisterSchemeService(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISchemeLocator>(factory =>
            {
                var schemeLocator = FileSchemeLocator.CreateFromEnvVariable();

                return schemeLocator;
            });

            serviceCollection.AddSingleton<ISchemeService, SchemeService>();

            serviceCollection.AddSingleton<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>();
        }

        private static void RegisterSectorService(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMapFactory, FuncMapFactory>();
            serviceCollection.AddSingleton<ISectorGenerator, TestEmptySectorGenerator>();
            serviceCollection.AddSingleton<IRoomGenerator, RoomGenerator>();
            serviceCollection.AddSingleton<IScoreManager, ScoreManager>();
            serviceCollection.AddSingleton<IActorInteractionBus, ActorInteractionBus>();
            serviceCollection.AddSingleton<IPersonFactory, TestEmptyPersonFactory>();
            serviceCollection.AddSingleton<IMonsterPersonFactory, MonsterPersonFactory>();
        }

        private IServiceCollection RegisterServices1()
        {
            var serviceCollection = new ServiceCollection();
            RegisterGlobeInitializationServices(serviceCollection);
            RegisterSchemeService(serviceCollection);
            RegisterSectorService(serviceCollection);
            RegisterAuxServices(serviceCollection);
            RegisterPlayerServices(serviceCollection);
            RegisterClientServices(serviceCollection);
            RegisterCommands(serviceCollection);
            return serviceCollection;
        }
    }
}