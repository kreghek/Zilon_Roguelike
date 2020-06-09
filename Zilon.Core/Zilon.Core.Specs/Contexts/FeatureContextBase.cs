using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.CommonServices;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.PersonGeneration;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Specs.Mocks;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.World;

namespace Zilon.Core.Specs.Contexts
{
    public abstract class FeatureContextBase
    {
        private ITacticalActUsageRandomSource _specificActUsageRandomSource;

        public IServiceProvider ServiceProvider { get; }

        public List<IActorInteractionEvent> RaisedActorInteractionEvents { get; }

        public IGlobe Globe { get; private set; }

        protected FeatureContextBase()
        {
            RaisedActorInteractionEvents = new List<IActorInteractionEvent>();

            var serviceCollection = RegisterServices();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            ServiceProvider = serviceProvider;

            ConfigureServices(serviceProvider);
        }

        private void ConfigureServices(ServiceProvider serviceProvider)
        {
            InitClientServices();

            ConfigureEventBus(serviceProvider);

            RegisterManager.ConfigureAuxServices(serviceProvider);
        }

        private void ConfigureEventBus(ServiceProvider serviceProvider)
        {
            var eventMessageBus = serviceProvider.GetRequiredService<IActorInteractionBus>();
            eventMessageBus.NewEvent += EventMessageBus_NewEvent;
        }

        private ServiceCollection RegisterServices()
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

        private void RegisterGlobeInitializationServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IGlobeInitializer, GlobeInitializer>();
            serviceCollection.AddSingleton<IBiomeInitializer, BiomeInitializer>();
            serviceCollection.AddSingleton<IBiomeSchemeRoller, BiomeSchemeRoller>();
            serviceCollection.AddSingleton<IGlobeTransitionHandler, GlobeTransitionHandler>();
            serviceCollection.AddSingleton<IPersonInitializer, HumanPersonInitializer>();
            serviceCollection.AddSingleton<IGlobeExpander>(serviceProvider => {
                return (BiomeInitializer)serviceProvider.GetRequiredService<IBiomeInitializer>();
            });
        }

        private void EventMessageBus_NewEvent(object sender, NewActorInteractionEventArgs e)
        {
            RaisedActorInteractionEvents.Add(e.ActorInteractionEvent);
        }

        public async Task CreateGlobeAsync(int startMapSize)
        {
            var mapFactory = (FuncMapFactory)ServiceProvider.GetRequiredService<IMapFactory>();
            mapFactory.SetFunc(() =>
            {
                ISectorMap map = new SectorGraphMap<HexNode, HexMapNodeDistanceCalculator>();

                MapFiller.FillSquareMap(map, startMapSize);

                var mapRegion = new MapRegion(1, map.Nodes.ToArray())
                {
                    IsStart = true,
                    IsOut = true,
                    ExitNodes = new[] { map.Nodes.Last() }
                };

                map.Regions.Add(mapRegion);

                return Task.FromResult(map);
            });

            var globeInitialzer = ServiceProvider.GetRequiredService<IGlobeInitializer>();
            var globe = await globeInitialzer.CreateGlobeAsync("intro").ConfigureAwait(false);
            Globe = globe;

            var humanPlayer = ServiceProvider.GetRequiredService<IPlayer>();

            var sectorNode = globe.SectorNodes.First();

            humanPlayer.BindSectorNode(sectorNode);
        }

        private class SquareGenerationOptionsSubScheme : ISectorSquareMapFactoryOptionsSubScheme
        {
            public SchemeSectorMapGenerator MapGenerator { get => SchemeSectorMapGenerator.SquarePlane; }
            public int Size { get; set; }
        }

        public ISector GetSector()
        {
            var player = ServiceProvider.GetRequiredService<IPlayer>();
            return player.SectorNode.Sector;
        }

        public void AddWall(int x1, int y1, int x2, int y2)
        {
            var player = ServiceProvider.GetRequiredService<IPlayer>();

            var map = player.SectorNode.Sector.Map;

            map.RemoveEdge(x1, y1, x2, y2);
        }

        public IActor GetActiveActor()
        {
            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();
            var actor = playerState.ActiveActor.Actor;
            return actor;
        }

        public Equipment CreateEquipment(string propSid)
        {
            var schemeService = ServiceProvider.GetRequiredService<ISchemeService>();
            var propFactory = ServiceProvider.GetRequiredService<IPropFactory>();

            var propScheme = schemeService.GetScheme<IPropScheme>(propSid);

            var equipment = propFactory.CreateEquipment(propScheme);
            return equipment;
        }

        public void AddHumanActor(string personSid, OffsetCoords startCoords)
        {
            var humanPlayer = ServiceProvider.GetRequiredService<IPlayer>();

            var sector = humanPlayer.SectorNode.Sector;

            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();
            var humanTaskSource = ServiceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            var actorManager = sector.ActorManager;
            var perkResolver = ServiceProvider.GetRequiredService<IPerkResolver>();

            // Подготовка актёров
            var humanStartNode = sector.Map.Nodes.SelectByHexCoords(startCoords.X, startCoords.Y);
            var humanActor = CreateHumanActor(personSid, humanStartNode, perkResolver);

            humanTaskSource.SwitchActiveActor(humanActor);

            actorManager.Add(humanActor);

            var humanActroViewModelMock = new Mock<IActorViewModel>();
            humanActroViewModelMock.SetupGet(x => x.Actor).Returns(humanActor);
            var humanActroViewModel = humanActroViewModelMock.Object;
            playerState.ActiveActor = humanActroViewModel;
        }

        public void AddMonsterActor(string monsterSid, int monsterId, OffsetCoords startCoords)
        {
            var humanPlayer = ServiceProvider.GetRequiredService<IPlayer>();

            var sector = humanPlayer.SectorNode.Sector;

            var schemeService = ServiceProvider.GetRequiredService<ISchemeService>();
            var actorManager = sector.ActorManager;

            var monsterScheme = schemeService.GetScheme<IMonsterScheme>(monsterSid);
            var monsterStartNode = sector.Map.Nodes.SelectByHexCoords(startCoords.X, startCoords.Y);

            var monster = CreateMonsterActor(monsterScheme, monsterStartNode);
            monster.Person.Id = monsterId;

            actorManager.Add(monster);
        }

        public IPropContainer AddChest(int id, OffsetCoords nodeCoords)
        {
            var humanPlayer = ServiceProvider.GetRequiredService<IPlayer>();

            var sector = humanPlayer.SectorNode.Sector;

            var node = sector.Map.Nodes.SelectByHexCoords(nodeCoords.X, nodeCoords.Y);
            var chest = new FixedPropChest(Array.Empty<IProp>());
            var staticObject = new StaticObject(node, chest.Purpose, id);
            staticObject.AddModule<IPropContainer>(chest);

            sector.StaticObjectManager.Add(staticObject);

            return chest;
        }

        public void AddResourceToActor(string resourceSid, int count, IActor actor)
        {
            var schemeService = ServiceProvider.GetRequiredService<ISchemeService>();

            var resourceScheme = schemeService.GetScheme<IPropScheme>(resourceSid);

            AddResourceToActor(resourceScheme, count, actor);
        }

        public static void AddResourceToActor(IPropScheme resourceScheme, int count, IActor actor)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            var resource = new Resource(resourceScheme, count);

            actor.Person.GetModule<IInventoryModule>().Add(resource);
        }

        public IActor GetMonsterById(int id)
        {
            var humanPlayer = ServiceProvider.GetRequiredService<IPlayer>();

            var sector = humanPlayer.SectorNode.Sector;

            var actorManager = sector.ActorManager;

            var monster = actorManager.Items
                .SingleOrDefault(x => x.Person is MonsterPerson && x.Person.Id == id);

            return monster;
        }

        public void SpecifyTacticalActUsageRandomSource(ITacticalActUsageRandomSource actUsageRandomSource)
        {
            _specificActUsageRandomSource = actUsageRandomSource;
        }

        private IActor CreateHumanActor([NotNull] string personSchemeSid,
            [NotNull] IGraphNode startNode,
            [NotNull] IPerkResolver perkResolver)
        {
            var personFactory = ServiceProvider.GetRequiredService<IPersonFactory>();
            var humanTaskSource = ServiceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();

            var person = personFactory.Create(personSchemeSid, Fractions.MainPersonFraction);

            var actor = new Actor(person, humanTaskSource, startNode, perkResolver);

            return actor;
        }

        private IActor CreateMonsterActor([NotNull] IMonsterScheme monsterScheme,
            [NotNull] IGraphNode startNode)
        {
            var monsterFactory = ServiceProvider.GetRequiredService<IMonsterPersonFactory>();

            var taskSource = ServiceProvider.GetRequiredService<IActorTaskSource<ISectorTaskSourceContext>>();

            var monsterPerson = monsterFactory.Create(monsterScheme);

            var actor = new Actor(monsterPerson, taskSource, startNode);

            return actor;
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

        private ITacticalActUsageRandomSource CreateActUsageRandomSource(IDice dice)
        {
            if (_specificActUsageRandomSource != null)
            {
                return _specificActUsageRandomSource;
            }

            var actUsageRandomSourceMock = new Mock<TacticalActUsageRandomSource>(dice).As<ITacticalActUsageRandomSource>();
            actUsageRandomSourceMock.Setup(x => x.RollEfficient(It.IsAny<Roll>()))
                .Returns<Roll>(roll => roll.Dice / 2 * roll.Count);  // Всегда берётся среднее значение среди всех бросков
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

            survivalRandomSourceMock.Setup(x => x.RollSurvival(It.IsAny<SurvivalStat>())).Returns(6);
            survivalRandomSourceMock.Setup(x => x.RollMaxHazardDamage()).Returns(6);

            return survivalRandomSource;
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

        private static void RegisterPlayerServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPlayer, HumanPlayer>();
            serviceCollection.AddSingleton<IActorTaskSource<ISectorTaskSourceContext>, HumanBotActorTaskSource<ISectorTaskSourceContext>>();
            serviceCollection.AddSingleton<IHumanActorTaskSource<ISectorTaskSourceContext>, HumanActorTaskSource<ISectorTaskSourceContext>>();
            serviceCollection.AddSingleton<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();
            serviceCollection.AddSingleton<IActorTaskSourceCollector>(serviceProvider =>
            {
                var humanTaskSource = serviceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
                var monsterTaskSource = serviceProvider.GetRequiredService<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();
                return new ActorTaskSourceCollector(humanTaskSource, monsterTaskSource);
            });
            RegisterManager.RegisterBot(serviceCollection);
        }

        private void InitClientServices()
        {
            var humanTaskSource = ServiceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();

            playerState.TaskSource = humanTaskSource;
        }

        public class VisualEventInfo
        {
            public VisualEventInfo(IActor actor, EventArgs eventArgs, string eventName)
            {
                Actor = actor ?? throw new ArgumentNullException(nameof(actor));
                EventArgs = eventArgs ?? throw new ArgumentNullException(nameof(eventArgs));
                EventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
            }

            public IActor Actor { get; }

            public EventArgs EventArgs { get; }

            public string EventName { get; }
        }
    }
}
