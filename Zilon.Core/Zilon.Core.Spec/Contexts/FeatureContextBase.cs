using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using JetBrains.Annotations;

using LightInject;

using Moq;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.PrimitiveStyle;
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

namespace Zilon.Core.Spec.Contexts
{
    public abstract class FeatureContextBase
    {
        private ITacticalActUsageRandomSource _specificActUsageRandomSource;

        private HumanPlayer _humanPlayer;
        private BotPlayer _botPlayer;

        public ServiceContainer Container { get; }

        public List<VisualEventInfo> VisualEvents { get; }

        protected FeatureContextBase()
        {
            Container = new ServiceContainer();

            RegisterSchemeService();
            RegisterSectorService();
            RegisterGameLoop();
            RegisterAuxServices();
            RegisterTaskSources();
            RegisterClientServices();
            RegisterCommands();

            InitPlayers();
            InitClientServices();

            VisualEvents = new List<VisualEventInfo>();
        }

        public void CreateSector(int mapSize)
        {
            var mapFactory = (FuncMapFactory)Container.GetInstance<IMapFactory>();
            mapFactory.SetFunc(() =>
            {
                var map = SquareMapFactory.Create(mapSize);
                return map;
            });

            var sectorManager = Container.GetInstance<ISectorManager>();
            sectorManager.CreateSector();
        }

        public void AddWall(int x1, int y1, int x2, int y2)
        {
            var sectorManager = Container.GetInstance<ISectorManager>();

            var map = sectorManager.CurrentSector.Map;

            map.RemoveEdge(x1, y1, x2, y2);
        }

        public IActor GetActiveActor()
        {
            var playerState = Container.GetInstance<IPlayerState>();
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
            var playerState = Container.GetInstance<IPlayerState>();
            var schemeService = Container.GetInstance<ISchemeService>();
            var sectorManager = Container.GetInstance<ISectorManager>();
            var humanTaskSource = Container.GetInstance<IHumanActorTaskSource>();
            var actorManager = Container.GetInstance<IActorManager>();

            var personScheme = schemeService.GetScheme<IPersonScheme>(personSid);

            // Подготовка актёров
            var humanStartNode = sectorManager.CurrentSector.Map.Nodes.Cast<HexNode>().SelectBy(startCoords.X, startCoords.Y);
            var humanActor = CreateHumanActor(_humanPlayer, personScheme, humanStartNode);

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

            var monsterScheme = schemeService.GetScheme<IMonsterScheme>(monsterSid);
            var monsterStartNode = sectorManager.CurrentSector.Map.Nodes.Cast<HexNode>().SelectBy(startCoords.X, startCoords.Y);

            var monster = CreateMonsterActor(_botPlayer, monsterScheme, monsterStartNode);
            monster.Person.Id = monsterId;

            monster.OnDefence += Monster_OnDefence;

            actorManager.Add(monster);
        }

        private void Monster_OnDefence(object sender, EventArgs e)
        {
            VisualEvents.Add(new VisualEventInfo((IActor)sender, e, nameof(IActor.OnDefence)));
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
            [NotNull] IMapNode startNode)
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

            var actor = new Actor(person, player, startNode);

            return actor;
        }

        private IActor CreateMonsterActor([NotNull] IBotPlayer player,
            [NotNull] IMonsterScheme monsterScheme,
            [NotNull] IMapNode startNode)
        {
            var survivalRandomSource = Container.GetInstance<ISurvivalRandomSource>();


            var monsterPerson = new MonsterPerson(monsterScheme, survivalRandomSource);

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
            Container.Register<ISectorProceduralGenerator, TestEmptySectorGenerator>(new PerContainerLifetime());
            Container.Register<ISectorManager, SectorManager>(new PerContainerLifetime());
            Container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            Container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());
            Container.Register<IRoomGenerator, RoomGenerator>(new PerContainerLifetime());
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
            actUsageRandomSourceMock.Setup(x => x.RollToHit())
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
            Container.Register<IPlayerState, PlayerState>(new PerContainerLifetime());
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

        private void RegisterTaskSources()
        {
            Container.Register<IBotPlayer, BotPlayer>(new PerContainerLifetime());
            Container.Register<IHumanActorTaskSource, HumanActorTaskSource>(new PerContainerLifetime());
            Container.Register<IActorTaskSource, MonsterActorTaskSource>("monster", new PerContainerLifetime());
        }

        private void InitPlayers()
        {
            _humanPlayer = new HumanPlayer();
            _botPlayer = (BotPlayer)Container.GetInstance<IBotPlayer>();
        }

        private void InitClientServices()
        {
            var humanTaskSource = Container.GetInstance<IHumanActorTaskSource>();
            var playerState = Container.GetInstance<IPlayerState>();

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
