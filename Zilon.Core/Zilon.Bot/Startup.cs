using System.Configuration;

using LightInject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.World;

namespace Zilon.Bot
{
    class Startup
    {
        public void ConfigureServices(IServiceRegistry container)
        {
            RegisterSchemeService(container);
            RegisterSectorService(container);
            RegisterGameLoop(container);
            RegisterAuxServices(container);
            RegisterPlayerServices(container);
            RegisterClientServices(container);
            RegisterCommands(container);
            RegisterWorldServices(container);
        }

        private void RegisterSchemeService(IServiceRegistry container)
        {
            container.Register<ISchemeLocator>(factory =>
            {
                var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

                var schemeLocator = new FileSchemeLocator(schemePath);

                return schemeLocator;
            }, new PerContainerLifetime());

            container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());

            container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
        }

        private void RegisterSectorService(IServiceRegistry container)
        {
            container.Register<IMapFactory, RoomMapFactory>(new PerContainerLifetime());
            container.Register<ISectorGenerator, SectorGenerator>(new PerContainerLifetime());
            container.Register<ISectorManager, SectorManager>(new PerContainerLifetime());
            container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());
            container.Register<ITraderManager, TraderManager>(new PerContainerLifetime());
            container.Register<IRoomGenerator, RoomGenerator>(new PerContainerLifetime());
            container.Register<IScoreManager, ScoreManager>(new PerContainerLifetime());
        }

        private void RegisterGameLoop(IServiceRegistry container)
        {
            container.Register<IGameLoop, GameLoop>(new PerContainerLifetime());
        }

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices(IServiceRegistry container)
        {
            var dice = new Dice(123);
            container.Register<IDice>(factory => dice, new PerContainerLifetime());

            container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
            container.Register<ITacticalActUsageRandomSource, TacticalActUsageRandomSource>(new PerContainerLifetime());

            container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());
            container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
            container.Register<IDropResolver, DropResolver>(new PerContainerLifetime());
            container.Register<IDropResolverRandomSource, DropResolverRandomSource>(new PerContainerLifetime());
            container.Register<ISurvivalRandomSource, SurvivalRandomSource>(new PerContainerLifetime());

            container.Register<IEquipmentDurableService, EquipmentDurableService>(new PerContainerLifetime());
            container.Register<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>(new PerContainerLifetime());
        }

        private void RegisterClientServices(IServiceRegistry container)
        {
            container.Register<ISectorUiState, SectorUiState>(new PerContainerLifetime());
            container.Register<IInventoryState, InventoryState>(new PerContainerLifetime());
        }

        private void RegisterCommands(IServiceRegistry container)
        {
            container.Register<ICommand, MoveCommand>("move", new PerContainerLifetime());
            container.Register<ICommand, UseSelfCommand>("use-self", new PerContainerLifetime());
            container.Register<ICommand, AttackCommand>("attack", new PerContainerLifetime());

            container.Register<ICommand, PropTransferCommand>("prop-transfer");
            container.Register<ICommand, EquipCommand>("equip");
        }

        private void RegisterPlayerServices(IServiceRegistry container)
        {
            container.Register<HumanPlayer>(new PerContainerLifetime());
            container.Register<IBotPlayer, BotPlayer>(new PerContainerLifetime());
            container.Register<IHumanActorTaskSource, HumanActorTaskSource>(new PerContainerLifetime());
            container.Register<IActorTaskSource, MonsterActorTaskSource>("monster", new PerContainerLifetime());
        }

        private void RegisterWorldServices(IServiceRegistry container)
        {
            container.Register<IWorldManager, WorldManager>(new PerContainerLifetime());
        }
    }
}
