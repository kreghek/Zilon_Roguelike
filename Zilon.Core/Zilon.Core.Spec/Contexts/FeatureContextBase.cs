using System.Configuration;
using System.Linq;

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
        protected readonly ServiceContainer Container;

        protected HumanPlayer HumanPlayer;

        protected FeatureContextBase()
        {
            Container = new ServiceContainer();
            Container.SetDefaultLifetime<PerContainerLifetime>();

            RegisterSchemeService();
            RegisterSectorService();
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

            Container.Register<IMap>(factory => map);
            Container.Register<ISector, Sector>();
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
            var map = Container.GetInstance<IMap>();
            var humanTaskSource = Container.GetInstance<IHumanActorTaskSource>();

            var personScheme = schemeService.GetScheme<PersonScheme>(personSid);

            // Подготовка актёров
            var humanStartNode = map.Nodes.Cast<HexNode>().SelectBy(startCoords.X, startCoords.Y);
            var humanActor = CreateHumanActor(HumanPlayer, personScheme, humanStartNode);

            humanTaskSource.SwitchActor(humanActor);

            var humanActroViewModelMock = new Mock<IActorViewModel>();
            humanActroViewModelMock.SetupGet(x => x.Actor).Returns(humanActor);
            var humanActroViewModel = humanActroViewModelMock.Object;
            playerState.ActiveActor = humanActroViewModel;
        }

        public void AddResourceToActor(string resourceSid, int count, IActor actor)
        {
            var schemeService = Container.GetInstance<ISchemeService>();

            var resourceScheme = schemeService.GetScheme<PropScheme>(resourceSid);

            AddResourceToActor(resourceScheme, 1, actor);
        }

        public void AddResourceToActor(PropScheme resourceScheme, int count, IActor actor)
        {
            var resource = new Resource(resourceScheme, count);

            actor.Person.Inventory.Add(resource);
        }


        private IActor CreateHumanActor([NotNull] IPlayer player,
            [NotNull] PersonScheme personScheme,
            [NotNull] IMapNode startNode)
        {
            var actorManager = Container.GetInstance<IActorManager>();
            var schemeService = Container.GetInstance<ISchemeService>();

            var evolutionData = new EvolutionData(schemeService);

            var inventory = new Inventory();

            var person = new HumanPerson(personScheme, evolutionData, inventory);

            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);

            // Указываем экипировку по умолчанию.
            var equipment = CreateEquipment("short-sword");
            actor.Person.EquipmentCarrier.SetEquipment(equipment, 0);

            // Второе оружие в инвернтаре
            var pistolEquipment = CreateEquipment("pistol");
            inventory.Add(pistolEquipment);

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

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices()
        {
            Container.Register<IDice>(factory => new Dice(), new PerContainerLifetime());
            Container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
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
        }

        private void RegisterTaskSources()
        {
            Container.Register<IHumanActorTaskSource, HumanActorTaskSource>();
        }

        private void InitPlayers()
        {
            HumanPlayer = new HumanPlayer();
        }

        private void InitClientServices()
        {
            var humanTaskSource = Container.GetInstance<IHumanActorTaskSource>();
            var playerState = Container.GetInstance<IPlayerState>();

            playerState.TaskSource = humanTaskSource;
        }
    }
}
