using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.TextClient
{
    static class Program
    {
        static async System.Threading.Tasks.Task Main()
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new StartUp();
            startUp.RegisterServices(serviceContainer);

            serviceContainer.AddSingleton<IGlobeInitializer, GlobeInitializer>();
            serviceContainer.AddSingleton<IGlobeExpander>(provider => (BiomeInitializer)provider.GetRequiredService<IBiomeInitializer>());
            serviceContainer.AddSingleton<IGlobeTransitionHandler, GlobeTransitionHandler>();
            serviceContainer.AddSingleton<IPersonInitializer, HumanPersonInitializer>();
            serviceContainer.AddSingleton<IPlayer, HumanPlayer>();
            serviceContainer.AddScoped<MoveCommand>();

            using var serviceProvider = serviceContainer.BuildServiceProvider();

            // Create globe

            using var scope = serviceProvider.CreateScope();

            var globeInitializer = scope.ServiceProvider.GetRequiredService<IGlobeInitializer>();
            var globe = await globeInitializer.CreateGlobeAsync("intro");
            var player = scope.ServiceProvider.GetRequiredService<IPlayer>();

            var gameLoop = new GameLoop(globe);
            var uiState = scope.ServiceProvider.GetRequiredService<ISectorUiState>();
            var playerActor = (from sectorNode in globe.SectorNodes
                               from actor in sectorNode.Sector.ActorManager.Items
                               where actor.Person == player.MainPerson
                               select actor).SingleOrDefault();
            var playerActorSectorNode = (from sectorNode in globe.SectorNodes
                                         from actor in sectorNode.Sector.ActorManager.Items
                                         where actor.Person == player.MainPerson
                                         select sectorNode).SingleOrDefault();

            // This is code smells. It is not good settings
            player.BindPerson(globe, playerActor.Person);

            uiState.ActiveActor = new ActorViewModel { Actor = playerActor };

            // Play

            gameLoop.StartProcessAsync();

            do
            {
                PrintState(uiState.ActiveActor.Actor);
                Console.WriteLine("Input command:");
                var inputText = Console.ReadLine();
                if (inputText.StartsWith("m"))
                {
                    var components = inputText.Split(' ');
                    var x = int.Parse(components[1]);
                    var y = int.Parse(components[2]);
                    var offsetCoords = new OffsetCoords(x, y);

                    ISectorMap map = playerActorSectorNode.Sector.Map;

                    var targetNode = map.Nodes.OfType<HexNode>().SingleOrDefault(node => node.OffsetCoords == offsetCoords);

                    var taskSource = scope.ServiceProvider.GetRequiredService<IActorTaskSource<ISectorTaskSourceContext>>();

                    var moveCommand = scope.ServiceProvider.GetRequiredService<MoveCommand>();

                    uiState.SelectedViewModel = new NodeViewModel { Node = targetNode };

                    moveCommand.Execute();
                }

                if (inputText.StartsWith("look"))
                {
                    var nextMoveNodes = playerActorSectorNode.Sector.Map.GetNext(uiState.ActiveActor.Actor.Node);
                    Console.WriteLine("Nodes:");
                    Console.WriteLine();
                    foreach (var nextNode in nextMoveNodes)
                    {
                        Console.Write(nextNode);
                        if (playerActorSectorNode.Sector.Map.Transitions.TryGetValue(nextNode, out var _))
                        {
                            Console.Write(" t");
                        }
                        Console.WriteLine();
                    }
                }

                if (inputText.StartsWith("exit"))
                {
                    break;
                }
            } while (true);
        }

        private static void PrintState(IActor actor)
        {
            Console.WriteLine(new string('=', 10));
            if (actor.Person.GetModule<IEffectsModule>().Items.Any())
            {
                Console.WriteLine("Effects:");
                foreach (var effect in actor.Person.GetModule<IEffectsModule>().Items)
                {
                    Console.WriteLine(effect);
                }
            }

            Console.WriteLine($"Position:{actor.Node}");
        }
    }

    class GameLoop
    {
        private readonly IGlobe _globe;

        public GameLoop(IGlobe globe)
        {
            _globe = globe ?? throw new ArgumentNullException(nameof(globe));
        }

        public async Task StartProcessAsync()
        {
            while (true)
            {
                await _globe.UpdateAsync();
            }
        }
    }

    class NodeViewModel : IMapNodeViewModel
    {
        public HexNode Node { get; set; }
        public object Item { get => Node; }
    }

    class ActorViewModel : IActorViewModel
    {
        public IActor Actor { get; set; }
        public object Item { get => Actor; }
    }
}