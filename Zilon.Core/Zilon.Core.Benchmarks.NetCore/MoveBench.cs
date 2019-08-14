using System;
using System.Linq;

using BenchmarkDotNet.Attributes;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
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

namespace Zilon.Core.Benchmarks
{
    public class MoveBench
    {
        private IServiceProvider _serviceProvider;

        [Benchmark(Description = "Move100")]
        public void Move100()
        {
            var sectorManager = _serviceProvider.GetService<ISectorManager>();
            var playerState = _serviceProvider.GetService<ISectorUiState>();
            var moveCommand = _serviceProvider.GetService<MoveCommand>();
            var commandManger = _serviceProvider.GetService<ICommandManager>();

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
            var sectorManager = _serviceProvider.GetService<ISectorManager>();
            var playerState = _serviceProvider.GetService<ISectorUiState>();
            var moveCommand = _serviceProvider.GetService<MoveCommand>();
            var commandManger = _serviceProvider.GetService<ICommandManager>();

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

            var serviceCollection = new ServiceCollection();

            // инстанцируем явно, чтобы обеспечить одинаковый рандом для всех запусков тестов.
            serviceCollection.AddSingleton<IDice>(factory => new Dice(123));
            serviceCollection.AddSingleton<IDecisionSource, DecisionSource>();
            serviceCollection.AddSingleton<IRoomGeneratorRandomSource, RoomGeneratorRandomSource>();
            serviceCollection.AddSingleton<ISchemeService, SchemeService>();
            serviceCollection.AddSingleton<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>();
            serviceCollection.AddSingleton<IPropFactory, PropFactory>();
            serviceCollection.AddSingleton<IDropResolver, DropResolver>();
            serviceCollection.AddSingleton<IDropResolverRandomSource, DropResolverRandomSource>();
            serviceCollection.AddSingleton<IPerkResolver, PerkResolver>();
            serviceCollection.AddSingleton<ISurvivalRandomSource, SurvivalRandomSource>();
            serviceCollection.AddSingleton<IChestGenerator, ChestGenerator>();
            serviceCollection.AddSingleton<IChestGeneratorRandomSource, ChestGeneratorRandomSource>();
            serviceCollection.AddSingleton<IMonsterGenerator, MonsterGenerator>();
            serviceCollection.AddSingleton<IMonsterGeneratorRandomSource, MonsterGeneratorRandomSource>();
            serviceCollection.AddSingleton<ICitizenGenerator, CitizenGenerator>();
            serviceCollection.AddSingleton<ICitizenGeneratorRandomSource, CitizenGeneratorRandomSource>();
            serviceCollection.AddSingleton<ISectorFactory, SectorFactory>();
            serviceCollection.AddSingleton<IEquipmentDurableService, EquipmentDurableService>();
            serviceCollection.AddSingleton<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>();

            serviceCollection.AddSingleton<HumanPlayer>();
            serviceCollection.AddSingleton<IBotPlayer, BotPlayer>();

            serviceCollection.AddSingleton<ISchemeLocator>(factory => CreateSchemeLocator());

            serviceCollection.AddSingleton<IGameLoop, GameLoop>();
            serviceCollection.AddSingleton<ICommandManager, QueueCommandManager>();
            serviceCollection.AddSingleton<ISectorUiState, SectorUiState>();
            serviceCollection.AddSingleton<IActorManager, ActorManager>();
            serviceCollection.AddSingleton<IPropContainerManager, PropContainerManager>();
            serviceCollection.AddSingleton<IHumanActorTaskSource, HumanActorTaskSource>();
            serviceCollection.AddSingleton<MonsterBotActorTaskSource>();
            serviceCollection.AddSingleton<ISectorGenerator, SectorGenerator>();
            serviceCollection.AddSingleton<IRoomGenerator, RoomGenerator>();
            serviceCollection.AddSingleton<IRoomGeneratorRandomSource, RoomGeneratorRandomSource>();
            serviceCollection.AddSingleton<IMapFactory, RoomMapFactory>();
            serviceCollection.AddSingleton<ITacticalActUsageService, TacticalActUsageService>();
            serviceCollection.AddSingleton<ITacticalActUsageRandomSource, TacticalActUsageRandomSource>();

            serviceCollection.AddSingleton<ISectorManager, SectorManager>();
            serviceCollection.AddSingleton<IWorldManager, WorldManager>();


            // Специализированные сервисы для Ui.
            serviceCollection.AddSingleton<IInventoryState, InventoryState>();

            // Комманды актёра.
            serviceCollection.AddSingleton<MoveCommand>();
            serviceCollection.AddSingleton<AttackCommand>();
            serviceCollection.AddSingleton<OpenContainerCommand>();
            serviceCollection.AddSingleton<NextTurnCommand>();
            serviceCollection.AddSingleton<UseSelfCommand>();

            // Комадны для UI.
            serviceCollection.AddSingleton<ShowContainerModalCommand>();
            serviceCollection.AddSingleton<ShowInventoryModalCommand>();
            serviceCollection.AddSingleton<ShowPerksModalCommand>();

            // Специализированные команды для Ui.
            serviceCollection.AddSingleton<EquipCommand>();
            serviceCollection.AddSingleton<PropTransferCommand>();

            _serviceProvider = serviceCollection.BuildServiceProvider();





            var sectorManager = _serviceProvider.GetService<ISectorManager>();
            var playerState = _serviceProvider.GetService<ISectorUiState>();
            var moveCommand = _serviceProvider.GetService<MoveCommand>();
            var schemeService = _serviceProvider.GetService<ISchemeService>();
            var humanPlayer = _serviceProvider.GetService<HumanPlayer>();
            var actorManager = _serviceProvider.GetService<IActorManager>();
            var humanActorTaskSource = _serviceProvider.GetService<IHumanActorTaskSource>();
            var commandManger = _serviceProvider.GetService<ICommandManager>();

            var sectorGenerator = _serviceProvider.GetService<ISectorGenerator>();

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

            // Эта строка не нужна для LightInject.
            // Он сам внедряет зависимости свойств. Опасная вещь.
            // DI корки так не делает.
            playerState.TaskSource = humanActorTaskSource;
            playerState.ActiveActor = playerActorVm;
            humanActorTaskSource.SwitchActor(playerState.ActiveActor.Actor);

            // Эта строка не нужна для LightInject.
            // Он сам внедряет зависимости свойств. Опасная вещь.
            // DI корки так не делает.
            var gameLoop = _serviceProvider.GetService<IGameLoop>();
            var monsterTaskSource = _serviceProvider.GetService<MonsterBotActorTaskSource>();
            gameLoop.ActorTaskSources = new IActorTaskSource[] {
                humanActorTaskSource,
                monsterTaskSource
            };
        }

        private FileSchemeLocator CreateSchemeLocator()
        {
            var schemePath = Environment.GetEnvironmentVariable("zilon_SchemeCatalog");
            var schemeLocator = new FileSchemeLocator(schemePath);
            return schemeLocator;
        }

        private IActorViewModel CreateHumanActorVm([NotNull] IPlayer player,
        [NotNull] IPersonScheme personScheme,
        [NotNull] IActorManager actorManager,
        [NotNull] IMapNode startNode)
        {
            var schemeService = _serviceProvider.GetService<ISchemeService>();
            var survivalRandomSource = _serviceProvider.GetService<ISurvivalRandomSource>();


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
