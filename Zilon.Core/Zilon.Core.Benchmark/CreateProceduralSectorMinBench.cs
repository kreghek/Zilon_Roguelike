using System.Configuration;
using System.Linq;

using BenchmarkDotNet.Attributes;

using JetBrains.Annotations;

using LightInject;

using Moq;
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
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Benchmark
{
    public class CreateProceduralSectorMinBench
    {
        private ServiceContainer _container;

        [Benchmark(Description = "CreateProceduralMinSector")]
        public async System.Threading.Tasks.Task CreateAsync()
        {
            var sectorManager = _container.GetInstance<ISectorManager>();
            var playerState = _container.GetInstance<ISectorUiState>();
            var schemeService = _container.GetInstance<ISchemeService>();
            var humanPlayer = _container.GetInstance<HumanPlayer>();
            var actorManager = _container.GetInstance<IActorManager>();
            var humanActorTaskSource = _container.GetInstance<IHumanActorTaskSource>();
            var sectorGenerator = _container.GetInstance<ISectorGenerator>();

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

            await sectorManager.CreateSectorAsync();

            var personScheme = schemeService.GetScheme<IPersonScheme>("human-person");

            var playerActorStartNode = sectorManager.CurrentSector.Map.Regions
                .SingleOrDefault(x => x.IsStart).Nodes
                .First();

            var playerActorVm = CreateHumanActorVm(humanPlayer,
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

        [IterationSetup]
        public void IterationSetup()
        {
            _container = new ServiceContainer();

            // инстанцируем явно, чтобы обеспечить одинаковый рандом для всех запусков тестов.
            _container.Register<IDice>(factory => new Dice(123), new PerContainerLifetime());
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

            _container.Register<ISchemeLocator>(factory => CreateSchemeLocator(), new PerContainerLifetime());

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

        private IMonsterGeneratorRandomSource CreateFakeMonsterGeneratorRandomSource()
        {
            var mock = new Mock<IMonsterGeneratorRandomSource>();
            mock.Setup(x => x.RollRegionCount(It.IsAny<int>(), It.IsAny<int>())).Returns(0);
            return mock.Object;
        }

        private IChestGeneratorRandomSource CreateFakeChestGeneratorRandomSource()
        {
            var mock = new Mock<IChestGeneratorRandomSource>();
            mock.Setup(x => x.RollChestCount(It.IsAny<int>())).Returns(0);
            return mock.Object;
        }

        private FileSchemeLocator CreateSchemeLocator()
        {
            var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];
            var schemeLocator = new FileSchemeLocator(schemePath);
            return schemeLocator;
        }

        private IActorViewModel CreateHumanActorVm([NotNull] IPlayer player,
        [NotNull] IPersonScheme personScheme,
        [NotNull] IActorManager actorManager,
        [NotNull] IMapNode startNode)
        {
            var schemeService = _container.GetInstance<ISchemeService>();
            var survivalRandomSource = _container.GetInstance<ISurvivalRandomSource>();


            var inventory = new Inventory();

            var evolutionData = new EvolutionData(schemeService);

            var defaultActScheme = schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme,
                defaultActScheme,
                evolutionData,
                survivalRandomSource,
                inventory);

            var actor = new Actor(person, player, startNode);

            actorManager.Add(actor);

            var actorViewModel = new TestActorViewModel
            {
                Actor = actor
            };

            return actorViewModel;
        }
    }
}
