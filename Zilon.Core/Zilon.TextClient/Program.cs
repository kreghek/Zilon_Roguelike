using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
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
            player.BindSectorNode(playerActorSectorNode);
            uiState.TaskSource = (IHumanActorTaskSource<ISectorTaskSourceContext>)playerActor.TaskSource;

            uiState.ActiveActor = new ActorViewModel { Actor = playerActor };

            // Play

            gameLoop.StartProcessAsync();

            do
            {
                var command = Console.ReadLine();
                if (command.StartsWith("m"))
                {
                    var taskSource = scope.ServiceProvider.GetRequiredService<IActorTaskSource<ISectorTaskSourceContext>>();

                    var moveCommand = scope.ServiceProvider.GetRequiredService<MoveCommand>();

                    var nextMoveNode = playerActorSectorNode.Sector.Map.GetNext(uiState.ActiveActor.Actor.Node).First();
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

    class ActorViewModel : IActorViewModel
    {
        public IActor Actor { get; set; }
        public object Item { get => Actor; }
    }
}