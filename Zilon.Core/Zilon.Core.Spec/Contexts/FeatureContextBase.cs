using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using LightInject;

using Moq;

using Zilon.Bot.Players;
using Zilon.Bot.Players.LightInject;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Spec.Mocks;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Spec.Contexts
{
    public abstract class FeatureContextBase
    {
        private ITacticalActUsageRandomSource _specificActUsageRandomSource;

        public ServiceContainer Container { get; }

        public List<IActorInteractionEvent> RaisedActorInteractionEvents { get; }

        protected FeatureContextBase()
        {
            Container = new ServiceContainer();
            RaisedActorInteractionEvents = new List<IActorInteractionEvent>();

            RegisterSchemeService();
            RegisterSectorService();
            RegisterGameLoop();
            RegisterAuxServices();
            RegisterPlayerServices();
            RegisterClientServices();
            RegisterCommands();
            RegisterWorldServices();

            InitClientServices();

            var eventMessageBus = Container.GetInstance<IActorInteractionBus>();
            eventMessageBus.NewEvent += EventMessageBus_NewEvent;
        }

        private void EventMessageBus_NewEvent(object sender, NewActorInteractionEventArgs e)
        {
            RaisedActorInteractionEvents.Add(e.ActorInteractionEvent);
        }

        public async Task CreateSectorAsync(int mapSize)
        {
            var mapFactory = (FuncMapFactory)Container.GetInstance<IMapFactory>();
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

            var sectorManager = Container.GetInstance<ISectorManager>();
            var sectorGenerator = Container.GetInstance<ISectorGenerator>();
            var humanPlayer = Container.GetInstance<HumanPlayer>();

            var locationScheme = new TestLocationScheme
            {
                SectorLevels = new ISectorSubScheme[]
               {
                    new TestSectorSubScheme
                    {
                        RegularMonsterSids = new[] { "rat" },
                    }
               }
            };
            var globeNode = new GlobeRegionNode(0, 0, locationScheme);
            humanPlayer.GlobeNode = globeNode;

            await sectorManager.CreateSectorAsync();
        }

        public ISector GetSector()
        {
            var sectorManager = Container.GetInstance<ISectorManager>();
            return sectorManager.CurrentSector;
        }

        public void AddWall(int x1, int y1, int x2, int y2)
        {
            var sectorManager = Container.GetInstance<ISectorManager>();

            var map = sectorManager.CurrentSector.Map;

            map.RemoveEdge(x1, y1, x2, y2);
        }

        public IActor GetActiveActor()
        {
            var playerState = Container.GetInstance<ISectorUiState>();
            var actor = playerState.ActiveActor.Actor;
            return actor;
        }

        public Equipment CreateEquipment(string propSid)
        {
            var schemeService = Container.GetInstance<ISchemeService>();
            var propFactory = Container.GetInstance<IPropFactory>();

            var propScheme = schemeService.GetScheme<IPropScheme>(propSid);

            var equipment = propFactory.CreateEquipment(propScheme);
            return equipment;
        }

        public void AddHumanActor(string personSid, OffsetCoords startCoords)
        {
            var playerState = Container.GetInstance<ISectorUiState>();
            var schemeService = Container.GetInstance<ISchemeService>();
            var sectorManager = Container.GetInstance<ISectorManager>();
            var humanTaskSource = Container.GetInstance<IHumanActorTaskSource>();
            var actorManager = Container.GetInstance<IActorManager>();
            var humanPlayer = Container.GetInstance<HumanPlayer>();
            var perkResolver = Container.GetInstance<IPerkResolver>();

            var personScheme = schemeService.GetScheme<IPersonScheme>(personSid);

            // Подготовка актёров
            var humanStartNode = sectorManager.CurrentSector.Map.Nodes.Cast<HexNode>().SelectBy(startCoords.X, startCoords.Y);
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
            var schemeService = Container.GetInstance<ISchemeService>();
            var sectorManager = Container.GetInstance<ISectorManager>();
            var actorManager = Container.GetInstance<IActorManager>();
            var botPlayer = Container.GetInstance<IBotPlayer>();

            var monsterScheme = schemeService.GetScheme<IMonsterScheme>(monsterSid);
            var monsterStartNode = sectorManager.CurrentSector.Map.Nodes.Cast<HexNode>().SelectBy(startCoords.X, startCoords.Y);

            var monster = CreateMonsterActor(botPlayer, monsterScheme, monsterStartNode);
            monster.Person.Id = monsterId;

            actorManager.Add(monster);
        }

        public IPropContainer AddChest(int id, OffsetCoords nodeCoords)
        {
            var containerManager = Container.GetInstance<IPropContainerManager>();
            var sectorManager = Container.GetInstance<ISectorManager>();

            var node = sectorManager.CurrentSector.Map.Nodes.Cast<HexNode>().SelectBy(nodeCoords.X, nodeCoords.Y);
            var chest = new FixedPropChest(node, new IProp[0], id);

            containerManager.Add(chest);

            return chest;
        }

        public void AddResourceToActor(string resourceSid, int count, IActor actor)
        {
            var schemeService = Container.GetInstance<ISchemeService>();

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
            var actorManager = Container.GetInstance<IActorManager>();

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
            [NotNull] IMapNode startNode,
            [NotNull] IPerkResolver perkResolver)
        {

            var schemeService = Container.GetInstance<ISchemeService>();
            var survivalRandomSource = Container.GetInstance<ISurvivalRandomSource>();

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
            [NotNull] IMapNode startNode)
        {
            var monsterPerson = new MonsterPerson(monsterScheme);

            var actor = new Actor(monsterPerson, player, startNode);

            return actor;
        }

        private void RegisterSchemeService()
        {
            Container.Register<ISchemeLocator>(factory =>
            {
                var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

                var schemeLocator = new FileSchemeLocator(schemePath);

                return schemeLocator;
            }, new PerContainerLifetime());

            Container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());

            Container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
        }

        private void RegisterSectorService()
        {
            Container.Register<IMapFactory, FuncMapFactory>(new PerContainerLifetime());
            Container.Register<ISectorGenerator, TestEmptySectorGenerator>(new PerContainerLifetime());
            Container.Register<ISectorManager, SectorManager>(new PerContainerLifetime());
            Container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            Container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());
            Container.Register<IRoomGenerator, RoomGenerator>(new PerContainerLifetime());
            Container.Register<IScoreManager, ScoreManager>(new PerContainerLifetime());
            Container.Register<ICitizenGenerator, CitizenGenerator>(new PerContainerLifetime());
            Container.Register<ICitizenGeneratorRandomSource, CitizenGeneratorRandomSource>(new PerContainerLifetime());
            Container.Register<IActorInteractionBus, ActorInteractionBus>(new PerContainerLifetime());
        }

        private void RegisterGameLoop()
        {
            Container.Register<IGameLoop, GameLoop>(new PerContainerLifetime());
        }

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices()
        {
            var dice = new Dice(123);
            Container.Register<IDice>(factory => dice, new PerContainerLifetime());


            var decisionSourceMock = new Mock<DecisionSource>(dice).As<IDecisionSource>();
            decisionSourceMock.CallBase = true;
            var decisionSource = decisionSourceMock.Object;

            Container.Register(factory => decisionSource, new PerContainerLifetime());
            Container.Register(factory => CreateActUsageRandomSource(dice), new PerContainerLifetime());

            Container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            Container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());
            Container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
            Container.Register<IDropResolver, DropResolver>(new PerContainerLifetime());
            Container.Register<IDropResolverRandomSource, DropResolverRandomSource>(new PerContainerLifetime());
            Container.Register(factory => CreateSurvivalRandomSource(), new PerContainerLifetime());

            Container.Register<IEquipmentDurableService, EquipmentDurableService>(new PerContainerLifetime());
            Container.Register<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>(new PerContainerLifetime());
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

        private void RegisterClientServices()
        {
            Container.Register<ISectorUiState, SectorUiState>(new PerContainerLifetime());
            Container.Register<IInventoryState, InventoryState>(new PerContainerLifetime());
        }

        private void RegisterCommands()
        {
            Container.Register<ICommand, MoveCommand>("move", new PerContainerLifetime());
            Container.Register<ICommand, UseSelfCommand>("use-self", new PerContainerLifetime());
            Container.Register<ICommand, AttackCommand>("attack", new PerContainerLifetime());

            Container.Register<ICommand, PropTransferCommand>("prop-transfer");
            Container.Register<ICommand, EquipCommand>("equip");
        }

        private void RegisterPlayerServices()
        {
            Container.Register<HumanPlayer>(new PerContainerLifetime());
            Container.Register<IBotPlayer, BotPlayer>(new PerContainerLifetime());
            Container.Register<IHumanActorTaskSource, HumanActorTaskSource>(new PerContainerLifetime());
            Container.Register<MonsterBotActorTaskSource>(new PerContainerLifetime());
            RegisterManager.RegisterBot(Container);
            RegisterManager.ConfigureAuxServices(Container);
        }

        private void RegisterWorldServices()
        {
            Container.Register<IWorldManager, WorldManager>(new PerContainerLifetime());
        }

        private void InitClientServices()
        {
            var humanTaskSource = Container.GetInstance<IHumanActorTaskSource>();
            var playerState = Container.GetInstance<ISectorUiState>();

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
