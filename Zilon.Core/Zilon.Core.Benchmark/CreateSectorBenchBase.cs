using BenchmarkDotNet.Attributes;

using LightInject;

using Zilon.Bot.Players;
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

namespace Zilon.Core.Benchmark
{
    public abstract class CreateSectorBenchBase
    {
        private ServiceContainer _container;

        [IterationSetup]
        public void IterationSetup()
        {
            _container = new ServiceContainer();

            // инстанцируем явно, чтобы обеспечить одинаковый рандом для всех запусков тестов.
            _container.Register<IDice>(factory => new LinearDice(123), new PerContainerLifetime());
            _container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
            _container.Register<IRoomGeneratorRandomSource, FixCompactRoomGeneratorRandomSource>(new PerContainerLifetime());
            _container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());
            _container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
            _container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
            _container.Register<IDropResolver, DropResolver>(new PerContainerLifetime());
            _container.Register<IDropResolverRandomSource, DropResolverRandomSource>(new PerContainerLifetime());
            _container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            _container.Register<ISurvivalRandomSource, SurvivalRandomSource>(new PerContainerLifetime());
            _container.Register<IChestGenerator, ChestGenerator>(new PerContainerLifetime());
            _container.Register(factory => CreateFakeChestGeneratorRandomSource(), new PerContainerLifetime());
            _container.Register<IMonsterGenerator, MonsterGenerator>(new PerContainerLifetime());
            _container.Register(factory => CreateFakeMonsterGeneratorRandomSource(), new PerContainerLifetime());
            _container.Register<ICitizenGenerator, CitizenGenerator>(new PerContainerLifetime());
            //TODO Сделать фейковый генератор
            _container.Register<ICitizenGeneratorRandomSource, CitizenGeneratorRandomSource>(new PerContainerLifetime());
            _container.Register<ISectorFactory, SectorFactory>(new PerContainerLifetime());
            _container.Register<IEquipmentDurableService, EquipmentDurableService>(new PerContainerLifetime());
            _container.Register<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>(new PerContainerLifetime());

            _container.Register<HumanPlayer>(new PerContainerLifetime());
            _container.Register<IBotPlayer, BotPlayer>(new PerContainerLifetime());

            _container.Register(factory => BenchHelper.CreateSchemeLocator(), new PerContainerLifetime());

            _container.Register<IGameLoop, GameLoop>(new PerContainerLifetime());
            _container.Register<ICommandManager, QueueCommandManager>(new PerContainerLifetime());
            _container.Register<ISectorUiState, SectorUiState>(new PerContainerLifetime());
            _container.Register<IActorManager, ActorManager>(new PerContainerLifetime());
            _container.Register<IPropContainerManager, PropContainerManager>(new PerContainerLifetime());
            _container.Register<IHumanActorTaskSource, HumanActorTaskSource>(new PerContainerLifetime());
            _container.Register<MonsterBotActorTaskSource>(lifetime: new PerContainerLifetime());
            _container.Register<ISectorGenerator, SectorGenerator>(new PerContainerLifetime());
            _container.Register<IRoomGenerator, RoomGenerator>(new PerContainerLifetime());
            _container.Register<IMapFactory, RoomMapFactory>(new PerContainerLifetime());
            _container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerContainerLifetime());
            _container.Register<ITacticalActUsageRandomSource, TacticalActUsageRandomSource>(new PerContainerLifetime());

            _container.Register<ISectorManager, SectorManager>(new PerContainerLifetime());
            _container.Register<IWorldManager, WorldManager>(new PerContainerLifetime());


            // Специализированные сервисы для Ui.
            _container.Register<IInventoryState, InventoryState>(new PerContainerLifetime());

            // Комманды актёра.
            _container.Register<ICommand, MoveCommand>(serviceName: "move-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, AttackCommand>(serviceName: "attack-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, OpenContainerCommand>(serviceName: "open-container-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, NextTurnCommand>(serviceName: "next-turn-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, UseSelfCommand>(serviceName: "use-self-command", lifetime: new PerContainerLifetime());

            // Комадны для UI.
            _container.Register<ICommand, ShowContainerModalCommand>(serviceName: "show-container-modal-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, ShowInventoryModalCommand>(serviceName: "show-inventory-command", lifetime: new PerContainerLifetime());
            _container.Register<ICommand, ShowPerksModalCommand>(serviceName: "show-perks-command", lifetime: new PerContainerLifetime());

            // Специализированные команды для Ui.
            _container.Register<ICommand, EquipCommand>(serviceName: "show-container-modal-command");
            _container.Register<ICommand, PropTransferCommand>(serviceName: "show-container-modal-command");
        }

        protected abstract IMonsterGeneratorRandomSource CreateFakeMonsterGeneratorRandomSource();
        protected abstract IChestGeneratorRandomSource CreateFakeChestGeneratorRandomSource();
    }
}
