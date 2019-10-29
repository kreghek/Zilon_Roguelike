using System.Linq;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using LightInject;

using Zilon.Bot.Players;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices;
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
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.World;
using Zilon.Emulation.Common;
using Zilon.IoC;

namespace Zilon.Core.Benchmark
{
    public abstract class CreateSectorBenchBase
    {
        protected ServiceContainer _container;

        [IterationSetup]
        public void IterationSetup()
        {
            _container = new ServiceContainer();

            RegisterServices();
        }

        [IterationCleanup]
        public void IterationCleanUp()
        {
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
        }

        private void RegisterServices()
        {
            // инстанцируем явно, чтобы обеспечить одинаковый рандом для всех запусков тестов.
            _container.Register<IDice>(factory => new LinearDice(123), LightInjectWrapper.CreateSingleton());
            _container.Register<IDecisionSource, DecisionSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<IRoomGeneratorRandomSource, FixCompactRoomGeneratorRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<ISchemeService, SchemeService>(LightInjectWrapper.CreateSingleton());
            _container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(LightInjectWrapper.CreateSingleton());
            _container.Register<IPropFactory, PropFactory>(LightInjectWrapper.CreateSingleton());
            _container.Register<IDropResolver, DropResolver>(LightInjectWrapper.CreateSingleton());
            _container.Register<IDropResolverRandomSource, DropResolverRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<IPerkResolver, PerkResolver>(LightInjectWrapper.CreateSingleton());
            _container.Register<ISurvivalRandomSource, SurvivalRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<IChestGenerator, ChestGenerator>(LightInjectWrapper.CreateSingleton());
            _container.Register(factory => CreateChestGeneratorRandomSource(), LightInjectWrapper.CreateSingleton());
            _container.Register<IMonsterGenerator, MonsterGenerator>(LightInjectWrapper.CreateSingleton());
            _container.Register(factory => CreateFakeMonsterGeneratorRandomSource(), LightInjectWrapper.CreateSingleton());
            _container.Register<ICitizenGenerator, CitizenGenerator>(LightInjectWrapper.CreateSingleton());
            //TODO Сделать фейковый генератор
            _container.Register<ICitizenGeneratorRandomSource, CitizenGeneratorRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<ISectorFactory, SectorFactory>(LightInjectWrapper.CreateSingleton());
            _container.Register<IEquipmentDurableService, EquipmentDurableService>(LightInjectWrapper.CreateSingleton());
            _container.Register<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<IUserTimeProvider, UserTimeProvider>(LightInjectWrapper.CreateSingleton());

            _container.Register<HumanPlayer>(LightInjectWrapper.CreateSingleton());
            _container.Register<IBotPlayer, BotPlayer>(LightInjectWrapper.CreateSingleton());

            _container.Register(factory => BenchHelper.CreateSchemeLocator(), LightInjectWrapper.CreateSingleton());

            _container.Register<IGameLoop, GameLoop>(LightInjectWrapper.CreateSingleton());
            _container.Register<ICommandManager, QueueCommandManager>(LightInjectWrapper.CreateSingleton());
            _container.Register<ISectorUiState, SectorUiState>(LightInjectWrapper.CreateSingleton());
            _container.Register<IActorManager, ActorManager>(LightInjectWrapper.CreateSingleton());
            _container.Register<IPropContainerManager, PropContainerManager>(LightInjectWrapper.CreateSingleton());
            _container.Register<IHumanActorTaskSource, HumanActorTaskSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<MonsterBotActorTaskSource>(lifetime: LightInjectWrapper.CreateSingleton());
            _container.Register<ISectorGenerator, SectorGenerator>(LightInjectWrapper.CreateSingleton());
            _container.Register<IRoomGenerator, RoomGenerator>(LightInjectWrapper.CreateSingleton());
            _container.Register<IMapFactory, RoomMapFactory>(LightInjectWrapper.CreateSingleton());
            _container.Register<IMapFactorySelector, LightInjectSwitchMapFactorySelector>(LightInjectWrapper.CreateSingleton());
            _container.Register<ITacticalActUsageService, TacticalActUsageService>(LightInjectWrapper.CreateSingleton());
            _container.Register<ITacticalActUsageRandomSource, TacticalActUsageRandomSource>(LightInjectWrapper.CreateSingleton());

            _container.Register<ISectorManager, SectorManager>(LightInjectWrapper.CreateSingleton());
            _container.Register<IWorldManager, WorldManager>(LightInjectWrapper.CreateSingleton());

            // Специализированные сервисы для Ui.
            _container.Register<IInventoryState, InventoryState>(LightInjectWrapper.CreateSingleton());

            // Комманды актёра.
            _container.Register<ICommand, MoveCommand>(serviceName: "move-command", lifetime: LightInjectWrapper.CreateSingleton());
            _container.Register<ICommand, AttackCommand>(serviceName: "attack-command", lifetime: LightInjectWrapper.CreateSingleton());
            _container.Register<ICommand, OpenContainerCommand>(serviceName: "open-container-command", lifetime: LightInjectWrapper.CreateSingleton());
            _container.Register<ICommand, NextTurnCommand>(serviceName: "next-turn-command", lifetime: LightInjectWrapper.CreateSingleton());
            _container.Register<ICommand, UseSelfCommand>(serviceName: "use-self-command", lifetime: LightInjectWrapper.CreateSingleton());

            // Комадны для UI.
            _container.Register<ICommand, ShowContainerModalCommand>(serviceName: "show-container-modal-command", lifetime: LightInjectWrapper.CreateSingleton());
            _container.Register<ICommand, ShowInventoryModalCommand>(serviceName: "show-inventory-command", lifetime: LightInjectWrapper.CreateSingleton());
            _container.Register<ICommand, ShowPerksModalCommand>(serviceName: "show-perks-command", lifetime: LightInjectWrapper.CreateSingleton());

            // Специализированные команды для Ui.
            _container.Register<ICommand, EquipCommand>(serviceName: "show-container-modal-command");
            _container.Register<ICommand, PropTransferCommand>(serviceName: "show-container-modal-command");
        }

        public async Task CreateInnerAsync()
        {
            var sectorManager = _container.GetInstance<ISectorManager>();
            var playerState = _container.GetInstance<ISectorUiState>();
            var schemeService = _container.GetInstance<ISchemeService>();
            var humanPlayer = _container.GetInstance<HumanPlayer>();
            var actorManager = _container.GetInstance<IActorManager>();
            var humanActorTaskSource = _container.GetInstance<IHumanActorTaskSource>();

            var locationScheme = new TestLocationScheme
            {
                SectorLevels = new ISectorSubScheme[]
                {
                    new TestSectorSubScheme
                    {
                        RegularMonsterSids = new[] { "rat" },
                        RareMonsterSids = new[] { "rat" },
                        ChampionMonsterSids = new[] { "rat" },

                        RegionCount = 20,
                        RegionSize = 20,

                        IsStart = true,

                        ChestDropTableSids = new[] {"survival", "default" },
                        RegionChestCountRatio = 9,
                        TotalChestCount = 20
                    }
                }
            };

            var globeNode = new GlobeRegionNode(0, 0, locationScheme);
            humanPlayer.GlobeNode = globeNode;

            await sectorManager.CreateSectorAsync().ConfigureAwait(false);

            var personScheme = schemeService.GetScheme<IPersonScheme>("human-person");

            var playerActorStartNode = sectorManager.CurrentSector.Map.Regions
                .SingleOrDefault(x => x.IsStart).Nodes
                .First();

            var survivalRandomSource = _container.GetInstance<ISurvivalRandomSource>();

            var playerActorVm = BenchHelper.CreateHumanActorVm(humanPlayer,
                schemeService,
                survivalRandomSource,
                personScheme,
                actorManager,
                playerActorStartNode);

            //Лучше централизовать переключение текущего актёра только в playerState
            playerState.ActiveActor = playerActorVm;
            humanActorTaskSource.SwitchActor(playerState.ActiveActor.Actor);

            var gameLoop = _container.GetInstance<IGameLoop>();
            var monsterTaskSource = _container.GetInstance<MonsterBotActorTaskSource>();
            gameLoop.ActorTaskSources = new IActorTaskSource[] {
                humanActorTaskSource,
                monsterTaskSource
            };
        }

        protected abstract IMonsterGeneratorRandomSource CreateFakeMonsterGeneratorRandomSource();
        protected abstract IChestGeneratorRandomSource CreateChestGeneratorRandomSource();
    }
}
