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
