using System;
using System.Linq;

using BenchmarkDotNet.Attributes;

using JetBrains.Annotations;

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
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.World;
using Zilon.Emulation.Common;
using Zilon.IoC;

namespace Zilon.Core.Benchmarks.NetCore.Move
{
    public class MoveBench
    {
        private ServiceContainer _container;

        [Benchmark(Description = "Move100")]
        public void Move100()
        {
            var sectorManager = _container.GetInstance<ISectorManager>();
            var playerState = _container.GetInstance<ISectorUiState>();
            var moveCommand = _container.GetInstance<MoveCommand>();
            var commandManger = _container.GetInstance<ICommandManager>();

            for (var i = 0; i < 100; i++)
            {
                var currentActorNode = playerState.ActiveActor.Actor.Node;
                var nextNodes = sectorManager.CurrentSector.Map.GetNext(currentActorNode);
                var moveTargetNode = (HexNode)nextNodes.First();

                playerState.SelectedViewModel = new TestNodeViewModel
                {
                    Node = moveTargetNode
                };

                commandManger.Push(moveCommand);

                ICommand command;
                do
                {
                    command = commandManger.Pop();

                    try
                    {
                        command?.Execute();
                    }
                    catch (Exception exception)
                    {
                        throw new InvalidOperationException($"Не удалось выполнить команду {command}.", exception);
                    }
                } while (command != null);
            }
        }

        [Benchmark(Description = "Move1")]
        public void Move1()
        {
            var sectorManager = _container.GetInstance<ISectorManager>();
            var playerState = _container.GetInstance<ISectorUiState>();
            var moveCommand = _container.GetInstance<MoveCommand>();
            var commandManger = _container.GetInstance<ICommandManager>();

            for (var i = 0; i < 1; i++)
            {
                var currentActorNode = (HexNode)playerState.ActiveActor.Actor.Node;
                var nextNodes = HexNodeHelper.GetSpatialNeighbors(currentActorNode, sectorManager.CurrentSector.Map.Nodes.Cast<HexNode>());
                var moveTargetNode = nextNodes.First();

                playerState.SelectedViewModel = new TestNodeViewModel
                {
                    Node = moveTargetNode
                };

                commandManger.Push(moveCommand);

                ICommand command;
                do
                {
                    command = commandManger.Pop();

                    try
                    {
                        command?.Execute();
                    }
                    catch (Exception exception)
                    {
                        throw new InvalidOperationException($"Не удалось выполнить команду {command}.", exception);
                    }
                } while (command != null);
            }
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _container = new ServiceContainer();

            // инстанцируем явно, чтобы обеспечить одинаковый рандом для всех запусков тестов.
            _container.Register<IDice>(factory => new LinearDice(123), LightInjectWrapper.CreateSingleton());
            _container.Register<IDecisionSource, DecisionSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<IRoomGeneratorRandomSource, RoomGeneratorRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<ISchemeService, SchemeService>(LightInjectWrapper.CreateSingleton());
            _container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(LightInjectWrapper.CreateSingleton());
            _container.Register<IPropFactory, PropFactory>(LightInjectWrapper.CreateSingleton());
            _container.Register<IDropResolver, DropResolver>(LightInjectWrapper.CreateSingleton());
            _container.Register<IDropResolverRandomSource, DropResolverRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<IPerkResolver, PerkResolver>(LightInjectWrapper.CreateSingleton());
            _container.Register<ISurvivalRandomSource, SurvivalRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<IChestGenerator, ChestGenerator>(LightInjectWrapper.CreateSingleton());
            _container.Register<IChestGeneratorRandomSource, ChestGeneratorRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<IMonsterGenerator, MonsterGenerator>(LightInjectWrapper.CreateSingleton());
            _container.Register<IMonsterGeneratorRandomSource, MonsterGeneratorRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<ICitizenGenerator, CitizenGenerator>(LightInjectWrapper.CreateSingleton());
            _container.Register<ICitizenGeneratorRandomSource, CitizenGeneratorRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<ISectorFactory, SectorFactory>(LightInjectWrapper.CreateSingleton());
            _container.Register<IEquipmentDurableService, EquipmentDurableService>(LightInjectWrapper.CreateSingleton());
            _container.Register<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<IUserTimeProvider, UserTimeProvider>(LightInjectWrapper.CreateSingleton());

            _container.Register<HumanPlayer>(LightInjectWrapper.CreateSingleton());
            _container.Register<IBotPlayer, BotPlayer>(LightInjectWrapper.CreateSingleton());

            _container.Register<ISchemeLocator>(factory => CreateSchemeLocator(), LightInjectWrapper.CreateSingleton());

            _container.Register<IGameLoop, GameLoop>(LightInjectWrapper.CreateSingleton());
            _container.Register<ICommandManager, QueueCommandManager>(LightInjectWrapper.CreateSingleton());
            _container.Register<ISectorUiState, SectorUiState>(LightInjectWrapper.CreateSingleton());
            _container.Register<IActorManager, ActorManager>(LightInjectWrapper.CreateSingleton());
            _container.Register<IPropContainerManager, PropContainerManager>(LightInjectWrapper.CreateSingleton());
            _container.Register<IHumanActorTaskSource, HumanActorTaskSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<MonsterBotActorTaskSource>(lifetime: LightInjectWrapper.CreateSingleton());
            _container.Register<ISectorGenerator, SectorGenerator>(LightInjectWrapper.CreateSingleton());
            _container.Register<IRoomGenerator, RoomGenerator>(LightInjectWrapper.CreateSingleton());
            _container.Register<IRoomGeneratorRandomSource, RoomGeneratorRandomSource>(LightInjectWrapper.CreateSingleton());
            _container.Register<IMapFactory, RoomMapFactory>(LightInjectWrapper.CreateSingleton());
            _container.Register<IMapFactorySelector, LightInjectSwitchMapFactorySelector>(LightInjectWrapper.CreateSingleton());
            _container.Register<ITacticalActUsageService, TacticalActUsageService>(LightInjectWrapper.CreateSingleton());
            _container.Register<ITacticalActUsageRandomSource, TacticalActUsageRandomSource>(LightInjectWrapper.CreateSingleton());

            _container.Register<ISectorManager, SectorManager>(LightInjectWrapper.CreateSingleton());
            _container.Register<IWorldManager, WorldManager>(LightInjectWrapper.CreateSingleton());

            // Специализированные сервисы для Ui.
            _container.Register<IInventoryState, InventoryState>(LightInjectWrapper.CreateSingleton());

            // Комманды актёра.
            _container.Register<MoveCommand>(lifetime: LightInjectWrapper.CreateSingleton());

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
                        RegionMonsterCount = 0,

                        RegionCount = 20,
                        RegionSize = 20,

                        IsStart = true,

                        ChestDropTableSids = new[] {"survival", "default" },
                        RegionChestCountRatio = 9,
                        TotalChestCount = 0
                    }
               }
            };

            var globeNode = new GlobeRegionNode(0, 0, locationScheme);
            humanPlayer.GlobeNode = globeNode;

            sectorManager.CreateSectorAsync().Wait();

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
        }

        private static FileSchemeLocator CreateSchemeLocator()
        {
            var schemePath = Environment.GetEnvironmentVariable("ZILON_LIV_SCHEME_CATALOG");
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
