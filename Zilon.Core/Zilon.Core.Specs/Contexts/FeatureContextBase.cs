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
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Specs.Contexts
{
    public abstract class FeatureContextBase
    {
        private ITacticalActUsageRandomSource _specificActUsageRandomSource;

        public IServiceProvider ServiceProvider { get; }

        public List<IActorInteractionEvent> RaisedActorInteractionEvents { get; }

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

            ConfigureGameLoop(serviceProvider);

            RegisterManager.ConfigureAuxServices(serviceProvider);
        }

        private void ConfigureEventBus(ServiceProvider serviceProvider)
        {
            var eventMessageBus = serviceProvider.GetRequiredService<IActorInteractionBus>();
            eventMessageBus.NewEvent += EventMessageBus_NewEvent;
        }

        private static void ConfigureGameLoop(ServiceProvider serviceProvider)
        {
            var gameLoop = serviceProvider.GetRequiredService<IGameLoop>();
            var humanTaskSource = serviceProvider.GetRequiredService<IHumanActorTaskSource>();
            var monsterTaskSource = serviceProvider.GetRequiredService<MonsterBotActorTaskSource>();
            gameLoop.ActorTaskSources = new IActorTaskSource[] { humanTaskSource, monsterTaskSource };
        }

        private ServiceCollection RegisterServices()
        {
            var serviceCollection = new ServiceCollection();
            RegisterSchemeService(serviceCollection);
            RegisterSectorService(serviceCollection);
            RegisterGameLoop(serviceCollection);
            RegisterAuxServices(serviceCollection);
            RegisterPlayerServices(serviceCollection);
            RegisterClientServices(serviceCollection);
            RegisterCommands(serviceCollection);
            return serviceCollection;
        }

        private void EventMessageBus_NewEvent(object sender, NewActorInteractionEventArgs e)
        {
            RaisedActorInteractionEvents.Add(e.ActorInteractionEvent);
        }

        public async Task CreateSectorAsync(int mapSize)
        {
            var mapFactory = (FuncMapFactory)ServiceProvider.GetRequiredService<IMapFactory>();
            mapFactory.SetFunc(() =>
            {
                ISectorMap map = new SectorGraphMap<HexNode, HexMapNodeDistanceCalculator>();

                MapFiller.FillSquareMap(map, mapSize);

                var mapRegion = new MapRegion(1, map.Nodes.ToArray())
                {
                    IsStart = true,
                    IsOut = true,
                    ExitNodes = new[] { map.Nodes.Last() }
                };

                map.Regions.Add(mapRegion);

                return Task.FromResult(map);
            });

            var sectorManager = ServiceProvider.GetRequiredService<ISectorManager>();
            var humanPlayer = ServiceProvider.GetRequiredService<HumanPlayer>();

            var sectorSubScheme = new TestSectorSubScheme
            {
                RegularMonsterSids = new[] { "rat" },
                MapGeneratorOptions = new SquareGenerationOptionsSubScheme
                { 
                    Size = mapSize
                }
            };

            var sectorNodeMock = new Mock<ISectorNode>();
            sectorNodeMock.SetupGet(x=>x.State).Returns(SectorNodeState.SectorMaterialized);
            sectorNodeMock.SetupGet(x => x.SectorScheme).Returns(sectorSubScheme);
            var sectorNode = sectorNodeMock.Object;

            humanPlayer.BindSectorNode(sectorNode);

            await sectorManager.CreateSectorAsync();
        }

        private class SquareGenerationOptionsSubScheme : ISectorSquareMapFactoryOptionsSubScheme
        {
            public SchemeSectorMapGenerator MapGenerator { get => SchemeSectorMapGenerator.SquarePlane; }
            public int Size { get; set; }
        }

        public ISector GetSector()
        {
            var sectorManager = ServiceProvider.GetRequiredService<ISectorManager>();
            return sectorManager.CurrentSector;
        }

        public void AddWall(int x1, int y1, int x2, int y2)
        {
            var sectorManager = ServiceProvider.GetRequiredService<ISectorManager>();

            var map = sectorManager.CurrentSector.Map;

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
            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();
            var schemeService = ServiceProvider.GetRequiredService<ISchemeService>();
            var sectorManager = ServiceProvider.GetRequiredService<ISectorManager>();
            var humanTaskSource = ServiceProvider.GetRequiredService<IHumanActorTaskSource>();
            var actorManager = sectorManager.CurrentSector.ActorManager;
            var humanPlayer = ServiceProvider.GetRequiredService<HumanPlayer>();
            var perkResolver = ServiceProvider.GetRequiredService<IPerkResolver>();

            var personScheme = schemeService.GetScheme<IPersonScheme>(personSid);

            // Подготовка актёров
            var humanStartNode = sectorManager.CurrentSector.Map.Nodes.SelectByHexCoords(startCoords.X, startCoords.Y);
            var humanActor = CreateHumanActor(humanPlayer, personScheme, humanStartNode, perkResolver);

            humanTaskSource.SwitchActor(humanActor);

            actorManager.Add(humanActor);

            var humanActroViewModelMock = new Mock<IActorViewModel>();
            humanActroViewModelMock.SetupGet(x => x.Actor).Returns(humanActor);
            var humanActroViewModel = humanActroViewModelMock.Object;
            playerState.ActiveActor = humanActroViewModel;
        }

        public void AddMonsterActor(string monsterSid, int monsterId, OffsetCoords startCoords)
        {
            var schemeService = ServiceProvider.GetRequiredService<ISchemeService>();
            var sectorManager = ServiceProvider.GetRequiredService<ISectorManager>();
            var actorManager = sectorManager.CurrentSector.ActorManager;
            var botPlayer = ServiceProvider.GetRequiredService<IBotPlayer>();

            var monsterScheme = schemeService.GetScheme<IMonsterScheme>(monsterSid);
            var monsterStartNode = sectorManager.CurrentSector.Map.Nodes.SelectByHexCoords(startCoords.X, startCoords.Y);

            var monster = CreateMonsterActor(botPlayer, monsterScheme, monsterStartNode);
            monster.Person.Id = monsterId;

            actorManager.Add(monster);
        }

        public IPropContainer AddChest(int id, OffsetCoords nodeCoords)
        {
            var sectorManager = ServiceProvider.GetRequiredService<ISectorManager>();

            var node = sectorManager.CurrentSector.Map.Nodes.SelectByHexCoords(nodeCoords.X, nodeCoords.Y);
            var chest = new FixedPropChest(new IProp[0]);
            var staticObject = new StaticObject(node, id);
            staticObject.AddModule<IPropContainer>(chest);

            sectorManager.CurrentSector.StaticObjectManager.Add(staticObject);

            return chest;
        }

        public void AddResourceToActor(string resourceSid, int count, IActor actor)
        {
            var schemeService = ServiceProvider.GetRequiredService<ISchemeService>();

            var resourceScheme = schemeService.GetScheme<IPropScheme>(resourceSid);

            AddResourceToActor(resourceScheme, count, actor);
        }

        public void AddResourceToActor(IPropScheme resourceScheme, int count, IActor actor)
        {
            var resource = new Resource(resourceScheme, count);

            actor.Person.Inventory.Add(resource);
        }

        public IActor GetMonsterById(int id)
        {
            var sectorManager = ServiceProvider.GetRequiredService<ISectorManager>();
            var actorManager = sectorManager.CurrentSector.ActorManager;

            var monster = actorManager.Items
                .SingleOrDefault(x => x.Person is MonsterPerson && x.Person.Id == id);

            return monster;
        }

        public void SpecifyTacticalActUsageRandomSource(ITacticalActUsageRandomSource actUsageRandomSource)
        {
            _specificActUsageRandomSource = actUsageRandomSource;
        }

        private IActor CreateHumanActor([NotNull] IPlayer player,
            [NotNull] IPersonScheme personScheme,
            [NotNull] IGraphNode startNode,
            [NotNull] IPerkResolver perkResolver)
        {
            var schemeService = ServiceProvider.GetRequiredService<ISchemeService>();
            var survivalRandomSource = ServiceProvider.GetRequiredService<ISurvivalRandomSource>();

            var evolutionData = new EvolutionData(schemeService);

            var inventory = new Inventory();

            var defaultActScheme = schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme,
                defaultActScheme,
                evolutionData,
                survivalRandomSource,
                inventory);

            var actor = new Actor(person, player, startNode, perkResolver);

            return actor;
        }

        private IActor CreateMonsterActor([NotNull] IBotPlayer player,
            [NotNull] IMonsterScheme monsterScheme,
            [NotNull] IGraphNode startNode)
        {
            var monsterPerson = new MonsterPerson(monsterScheme);

            var actor = new Actor(monsterPerson, player, startNode);

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
            serviceCollection.AddSingleton<ISectorManager, SectorManager>();
            serviceCollection.AddSingleton<IRoomGenerator, RoomGenerator>();
            serviceCollection.AddSingleton<IScoreManager, ScoreManager>();
            serviceCollection.AddSingleton<ICitizenGenerator, CitizenGenerator>();
            serviceCollection.AddSingleton<ICitizenGeneratorRandomSource, CitizenGeneratorRandomSource>();
            serviceCollection.AddSingleton<IActorInteractionBus, ActorInteractionBus>();
        }

        private static void RegisterGameLoop(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IGameLoop, GameLoop>();
        }

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices(ServiceCollection serviceCollection)
        {
            var dice = new LinearDice(123);
            serviceCollection.AddSingleton<IDice>(factory => dice);

            var decisionSourceMock = new Mock<DecisionSource>(dice).As<IDecisionSource>();
            decisionSourceMock.CallBase = true;
            var decisionSource = decisionSourceMock.Object;

            serviceCollection.AddSingleton(factory => decisionSource);
            serviceCollection.AddSingleton(factory => CreateActUsageRandomSource(dice));

            serviceCollection.AddSingleton<IPerkResolver, PerkResolver>();
            serviceCollection.AddSingleton<ITacticalActUsageService, TacticalActUsageService>();
            serviceCollection.AddSingleton<IPropFactory, PropFactory>();
            serviceCollection.AddSingleton<IDropResolver, DropResolver>();
            serviceCollection.AddSingleton<IDropResolverRandomSource, DropResolverRandomSource>();
            serviceCollection.AddSingleton(factory => CreateSurvivalRandomSource());

            serviceCollection.AddSingleton<IEquipmentDurableService, EquipmentDurableService>();
            serviceCollection.AddSingleton<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>();

            serviceCollection.AddSingleton<IUserTimeProvider, UserTimeProvider>();
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

        private ISurvivalRandomSource CreateSurvivalRandomSource()
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
            serviceCollection.AddSingleton<UseSelfCommand>();
            serviceCollection.AddSingleton<AttackCommand>();

            serviceCollection.AddTransient<PropTransferCommand>();
            serviceCollection.AddTransient<EquipCommand>();
        }

        private static void RegisterPlayerServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<HumanPlayer>();
            serviceCollection.AddSingleton<IBotPlayer, BotPlayer>();
            serviceCollection.AddSingleton<IHumanActorTaskSource, HumanActorTaskSource>();
            serviceCollection.AddSingleton<MonsterBotActorTaskSource>();
            RegisterManager.RegisterBot(serviceCollection);
        }

        private void InitClientServices()
        {
            var humanTaskSource = ServiceProvider.GetRequiredService<IHumanActorTaskSource>();
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
