using System.Configuration;

using LightInject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Core.Spec.Contexts
{
    public abstract class FeatureContextBase
    {
        protected readonly ServiceContainer _container;

        protected HumanPlayer _humanPlayer;
        protected BotPlayer _botPlayer;

        protected FeatureContextBase()
        {
            _container = new ServiceContainer();
            _container.SetDefaultLifetime<PerContainerLifetime>();

            RegisterSchemeService();
            RegisterSectorService();
            RegisterAuxServices();
            RegisterTaskSources();
            RegisterClientServices();
            RegisterCommands();

            InitPlayers();
            InitClientServices();
        }

        public IActor GetActiveActor()
        {
            var playerState = _container.GetInstance<IPlayerState>();
            var actor = playerState.ActiveActor.Actor;
            return actor;
        }

        private void RegisterSchemeService()
        {
            _container.Register<ISchemeLocator>(factory =>
            {
                var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

                var schemeLocator = new FileSchemeLocator(schemePath);

                return schemeLocator;
            }, new PerContainerLifetime());

            _container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());

            _container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
        }

        private void RegisterSectorService()
        {
            _container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            _container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());            
        }

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices()
        {
            _container.Register<IDice>(factory => new Dice(), new PerContainerLifetime());
            _container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
            _container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            _container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());
            _container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
        }

        private void RegisterClientServices()
        {
            _container.Register<IPlayerState, PlayerState>(new PerContainerLifetime());
            _container.Register<ISectorManager, SectorManager>(new PerContainerLifetime());
            _container.Register<IInventoryState, InventoryState>(new PerContainerLifetime());
        }

        private void RegisterCommands()
        {
            _container.Register<ICommand, MoveCommand>("move", new PerContainerLifetime());
            _container.Register<ICommand, UseSelfCommand>("use-self", new PerContainerLifetime());
        }

        private void RegisterTaskSources()
        {
            _container.Register<IHumanActorTaskSource, HumanActorTaskSource>();
        }

        private void InitPlayers()
        {
            _humanPlayer = new HumanPlayer();
            _botPlayer = new BotPlayer();
        }

        private void InitClientServices()
        {
            var humanTaskSource = _container.GetInstance<IHumanActorTaskSource>();
            var playerState = _container.GetInstance<IPlayerState>();

            playerState.TaskSource = humanTaskSource;
        }
    }
}
