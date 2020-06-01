using System;
using System.Linq;

using BenchmarkDotNet.Attributes;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Zilon.Core.Benchmark;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Benchmarks.Move
{
    public class MoveBench
    {
        private ServiceProvider _serviceProvider;

        [Benchmark(Description = "Move100")]
        public void Move100()
        {
            var sectorManager = _serviceProvider.GetRequiredService<ISectorManager>();
            var playerState = _serviceProvider.GetRequiredService<ISectorUiState>();
            var moveCommand = _serviceProvider.GetRequiredService<MoveCommand>();
            var commandManger = _serviceProvider.GetRequiredService<ICommandManager>();

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
            var sectorManager = _serviceProvider.GetRequiredService<ISectorManager>();
            var playerState = _serviceProvider.GetRequiredService<ISectorUiState>();
            var moveCommand = _serviceProvider.GetRequiredService<MoveCommand>();
            var commandManger = _serviceProvider.GetRequiredService<ICommandManager>();

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
            var startUp = new Startup();
            var serviceCollection = new ServiceCollection();
            startUp.RegisterServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            var sectorManager = _serviceProvider.GetRequiredService<ISectorManager>();
            var playerState = _serviceProvider.GetRequiredService<ISectorUiState>();
            var schemeService = _serviceProvider.GetRequiredService<ISchemeService>();
            var humanPlayer = _serviceProvider.GetRequiredService<HumanPlayer>();
            var actorManager = _serviceProvider.GetRequiredService<IActorManager>();
            var humanActorTaskSource = _serviceProvider.GetRequiredService<IHumanActorTaskSource>();

            TestSectorSubScheme testSectorSubScheme = new TestSectorSubScheme
            {
                RegularMonsterSids = new[] { "rat" },
                RegionMonsterCount = 0,

                MapGeneratorOptions = new TestSectorRoomMapFactoryOptionsSubScheme
                {
                    RegionCount = 20,
                    RegionSize = 20,
                },

                IsStart = true,

                ChestDropTableSids = new[] { "survival", "default" },
                RegionChestCountRatio = 9,
                TotalChestCount = 0
            };

            var sectorNode = new TestMaterializedSectorNode(testSectorSubScheme);

            humanPlayer.BindSectorNode(sectorNode);

            sectorManager.CreateSectorAsync().Wait();

            var personScheme = schemeService.GetScheme<IPersonScheme>("human-person");

            var playerActorStartNode = sectorManager.CurrentSector.Map.Regions
                .SingleOrDefault(x => x.IsStart).Nodes
                .First();

            var survivalRandomSource = _serviceProvider.GetRequiredService<ISurvivalRandomSource>();
            var playerActorVm = BenchHelper.CreateHumanActorVm(humanActorTaskSource,
                schemeService,
                survivalRandomSource,
                personScheme,
                actorManager,
                playerActorStartNode);

            //Лучше централизовать переключение текущего актёра только в playerState
            playerState.ActiveActor = playerActorVm;
            humanActorTaskSource.SwitchActiveActor(playerState.ActiveActor.Actor);
        }
    }
}
