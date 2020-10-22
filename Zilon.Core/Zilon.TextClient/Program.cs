using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Players;
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

            var gameLoop = new GameLoop(globe);

            // Play

            gameLoop.StartProcessAsync();

            do
            {
                var command = Console.ReadLine();
                if (command.StartsWith("m"))
                {
                    var taskSource = scope.ServiceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
                    var uiState = scope.ServiceProvider.GetRequiredService<ISectorUiState>();
                    var player = scope.ServiceProvider.GetRequiredService<IPlayer>();
                    var moveCommand = scope.ServiceProvider.GetRequiredService<MoveCommand>();

                    var nextMoveNode = player.SectorNode.Sector.Map.GetNext(uiState.ActiveActor.Actor.Node).First();
                    uiState.SelectedViewModel = new NodeViewModel { Node = (HexNode)nextMoveNode };

                    moveCommand.Execute();
                }

                if (command.StartsWith("exit"))
                {
                    break;
                }
            } while (true);
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
}