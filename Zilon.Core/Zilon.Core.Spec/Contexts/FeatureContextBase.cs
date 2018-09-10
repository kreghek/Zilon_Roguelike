using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

using JetBrains.Annotations;

using LightInject;

using Moq;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Spec.Contexts
{
    public abstract class FeatureContextBase
    {
        private HumanPlayer _humanPlayer;
        private BotPlayer _botPlayer;

        public ServiceContainer Container { get; }

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
        }

        public void CreateSector(int mapSize)
        {
            var map = new TestGridGenMap(mapSize);

            Container.Register<IMap>(factory => map, new PerContainerLifetime());
            Container.Register<ISector, Sector>(new PerContainerLifetime());

            // Это нужно для того, чтобы объкт был создан и выполнился код из конструктора.
            // Там обработка на события внутренних сервисов.
            var sector = Container.GetInstance<ISector>();
        }

        public void AddWall(int x1, int y1, int x2, int y2)
        {
            var sector = Container.GetInstance<ISector>();

            var map = sector.Map;

            RemoveEdge(map.Edges, x1, y1, x2, y2);
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

            var propScheme = schemeService.GetScheme<PropScheme>(propSid);

            var equipment = propFactory.CreateEquipment(propScheme);
            return equipment;
        }

        public void AddHumanActor(string personSid, OffsetCoords startCoords)
        {
            var playerState = Container.GetInstance<IPlayerState>();
            var schemeService = Container.GetInstance<ISchemeService>();
            var sector = Container.GetInstance<ISector>();
            var humanTaskSource = Container.GetInstance<IHumanActorTaskSource>();
            var actorManager = Container.GetInstance<IActorManager>();

            var personScheme = schemeService.GetScheme<PersonScheme>(personSid);

            // Подготовка актёров
            var humanStartNode = sector.Map.Nodes.Cast<HexNode>().SelectBy(startCoords.X, startCoords.Y);
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
            var sector = Container.GetInstance<ISector>();
            var actorManager = Container.GetInstance<IActorManager>();

            var monsterScheme = schemeService.GetScheme<MonsterScheme>(monsterSid);
            var monsterStartNode = sector.Map.Nodes.Cast<HexNode>().SelectBy(startCoords.X, startCoords.Y);

            var monster = CreateMonsterActor(_botPlayer, monsterScheme, monsterStartNode);
            monster.Person.Id = monsterId;

            actorManager.Add(monster);
        }

        public IPropContainer AddChest(int id, OffsetCoords nodeCoords)
        {
            var containerManager = Container.GetInstance<IPropContainerManager>();

            var sector = Container.GetInstance<ISector>();
            var node = sector.Map.Nodes.Cast<HexNode>().SelectBy(nodeCoords.X, nodeCoords.Y);
            var chest = new FixedPropContainer(node, new IProp[0])
            {
                Id = id
            };

            containerManager.Add(chest);

            return chest;
        }

        public void AddResourceToActor(string resourceSid, int count, IActor actor)
        {
            var schemeService = Container.GetInstance<ISchemeService>();

            var resourceScheme = schemeService.GetScheme<PropScheme>(resourceSid);

            AddResourceToActor(resourceScheme, count, actor);
        }

        public void AddResourceToActor(PropScheme resourceScheme, int count, IActor actor)
        {
            var resource = new Resource(resourceScheme, count);

            actor.Person.Inventory.Add(resource);
        }

        public IActor GetMonsterById(int id)
        {
            var actorManager = Container.GetInstance<IActorManager>();

            var monster = actorManager.Actors
                .SingleOrDefault(x => x.Person is MonsterPerson && x.Person.Id == id);

            return monster;
        }


        private IActor CreateHumanActor([NotNull] IPlayer player,
            [NotNull] PersonScheme personScheme,
            [NotNull] IMapNode startNode)
        {

            var schemeService = Container.GetInstance<ISchemeService>();

            var evolutionData = new EvolutionData(schemeService);

            var inventory = new Inventory();

            var defaultActScheme = schemeService.GetScheme<TacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme, defaultActScheme, evolutionData, inventory);

            var actor = new Actor(person, player, startNode);

            return actor;
        }

        private IActor CreateMonsterActor([NotNull] IPlayer player,
            [NotNull] MonsterScheme monsterScheme,
            [NotNull] IMapNode startNode)
        {

            var schemeService = Container.GetInstance<ISchemeService>();

            var evolutionData = new EvolutionData(schemeService);

            var inventory = new Inventory();

            var monsterPerson = new MonsterPerson(monsterScheme);

            var actor = new Actor(monsterPerson, player, startNode);

            return actor;
        }

        private IActor CreateActor([NotNull] IPlayer player,
            [NotNull] IPerson person,
            [NotNull] IMapNode startNode)
        {
            var actorManager = Container.GetInstance<IActorManager>();

            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);

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
            Container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            Container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());
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
            var dice = new Dice();
            Container.Register<IDice>(factory => dice, new PerContainerLifetime());


            var decisionSourceMock = new Mock<DecisionSource>(dice).As<IDecisionSource>();
            decisionSourceMock.CallBase = true;
            decisionSourceMock.Setup(x => x.SelectEfficient(It.IsAny<float>(), It.IsAny<float>()))
                .Returns<float, float>((min, max) => (float)Math.Round((max - min) / 2 + min, 1));
            var decisionSource = decisionSourceMock.Object;

            Container.Register(factory => decisionSource, new PerContainerLifetime());

            Container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            Container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());
            Container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
        }

        private void RegisterClientServices()
        {
            Container.Register<IPlayerState, PlayerState>(new PerContainerLifetime());
            Container.Register<ISectorManager, SectorManager>(new PerContainerLifetime());
            Container.Register<IInventoryState, InventoryState>(new PerContainerLifetime());
        }

        private void RegisterCommands()
        {
            Container.Register<ICommand, MoveCommand>("move", new PerContainerLifetime());
            Container.Register<ICommand, UseSelfCommand>("use-self", new PerContainerLifetime());
            Container.Register<ICommand, AttackCommand>("attack", new PerContainerLifetime());

            Container.Register<ICommand, PropTransferCommand>("prop-transfer");
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

        private IEdge GetEdge(IList<IEdge> edges, int offsetX1, int offsetY1, int offsetX2, int offsetY2)
        {
            var foundFromStart = from edge in edges
                                 from node in edge.Nodes
                                 let hexNode = (HexNode)node
                                 where hexNode.OffsetX == offsetX1 && hexNode.OffsetY == offsetY1
                                 select edge;

            var foundToEnd = from edge in foundFromStart
                             from node in edge.Nodes
                             let hexNode = (HexNode)node
                             where hexNode.OffsetX == offsetX2 && hexNode.OffsetY == offsetY2
                             select edge;

            return foundToEnd.SingleOrDefault();
        }

        private void RemoveEdge(IList<IEdge> edges, int offsetX1, int offsetY1, int offsetX2, int offsetY2)
        {
            var edge = GetEdge(edges, offsetX1, offsetY1, offsetX2, offsetY2);
            edges.Remove(edge);
        }
    }
}
