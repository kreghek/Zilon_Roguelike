using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.World;

namespace Zilon.Core.Benchmarks.Move
{
    public class MoveBench
    {
        private IGlobe _globe;
        private ServiceProvider _serviceProvider;

        [Benchmark(Description = "Move100")]
        public void Move100()
        {
            var player = _serviceProvider.GetRequiredService<IPlayer>();
            var playerState = _serviceProvider.GetRequiredService<ISectorUiState>();
            var moveCommand = _serviceProvider.GetRequiredService<MoveCommand>();
            var commandManger = _serviceProvider.GetRequiredService<ICommandManager>();

            var gameLoop = new GameLoop(_globe);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            gameLoop.StartProcessAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            for (var i = 0; i < 100; i++)
            {
                var currentActorNode = playerState.ActiveActor.Actor.Node;
                var nextNodes = player.SectorNode.Sector.Map.GetNext(currentActorNode);
                var moveTargetNode = (HexNode)nextNodes.First();

                playerState.SelectedViewModel = new TestNodeViewModel {Node = moveTargetNode};

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
            var player = _serviceProvider.GetRequiredService<IPlayer>();
            var playerState = _serviceProvider.GetRequiredService<ISectorUiState>();
            var moveCommand = _serviceProvider.GetRequiredService<MoveCommand>();
            var commandManger = _serviceProvider.GetRequiredService<ICommandManager>();

            var sector = player.SectorNode.Sector;
            for (var i = 0; i < 1; i++)
            {
                var currentActorNode = (HexNode)playerState.ActiveActor.Actor.Node;
                var nextNodes = HexNodeHelper.GetSpatialNeighbors(currentActorNode, sector.Map.Nodes.Cast<HexNode>());
                var moveTargetNode = nextNodes.First();

                playerState.SelectedViewModel = new TestNodeViewModel {Node = moveTargetNode};

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

            var playerState = _serviceProvider.GetRequiredService<ISectorUiState>();
            var schemeService = _serviceProvider.GetRequiredService<ISchemeService>();
            var humanPlayer = _serviceProvider.GetRequiredService<IPlayer>();
            var humanActorTaskSource =
                _serviceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();

            var personScheme = schemeService.GetScheme<IPersonScheme>("human-person");

            _globe = new TestGlobe(personScheme, humanActorTaskSource);

            IActor actor = _globe.SectorNodes.SelectMany(x => x.Sector.ActorManager.Items).Single();
            var person = actor.Person;

            humanPlayer.BindPerson(_globe, person);

            var actorViewModel = new TestActorViewModel {Actor = actor};

            playerState.ActiveActor = actorViewModel;
        }
    }
}