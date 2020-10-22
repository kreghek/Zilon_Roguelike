using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;
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
            serviceContainer.AddSingleton<IPersonInitializer, AutoPersonInitializer>();

            using var serviceProvider = serviceContainer.BuildServiceProvider();

            // Create globe
            var globeInitializer = serviceProvider.GetRequiredService<IGlobeInitializer>();
            var globe = await globeInitializer.CreateGlobeAsync("intro");

            var gameLoop = new GameLoop(globe);

            // Play

            gameLoop.StartProcessAsync();

            do
            {
                var command = Console.ReadLine();
                if (command.StartsWith("m"))
                {
                    var taskSource = serviceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
                    var uiState = serviceProvider.GetRequiredService<ISectorUiState>();
                }

                if (command.StartsWith("exit"))
                {
                    break;
                }
            } while (true);

            // Iterate globe
            var globeIterationCounter = 0L;
            do
            {
                Console.WriteLine("Iteratin count:");
                var iterationCount = int.Parse(Console.ReadLine());
                for (var i = 0; i < iterationCount; i++)
                {
                    await globe.UpdateAsync();

                    globeIterationCounter++;

                    var hasActors = globe.SectorNodes.SelectMany(x => x.Sector.ActorManager.Items).Any(x => x.Person.Fraction != Fractions.MonsterFraction);
                    if (!hasActors)
                    {
                        // Все персонажи-немонстры вымерли.
                        break;
                    }
                }

                Console.WriteLine($"Globe Iteration: {globeIterationCounter}");
                PrintReport(globe);

            } while (true);
        }

        private static void PrintReport(IGlobe globe)
        {
            var sectorNodesDiscoveredCount = globe.SectorNodes.Count();
            Console.WriteLine($"Sector Nodes Discovered: {sectorNodesDiscoveredCount}");

            var actorCount = globe.SectorNodes.SelectMany(x => x.Sector.ActorManager.Items).Count();
            Console.WriteLine($"Actors: {actorCount}");

            var fractions = globe.SectorNodes.SelectMany(x => x.Sector.ActorManager.Items).GroupBy(x => x.Person.Fraction);
            foreach (var fractionGroup in fractions)
            {
                Console.WriteLine($"Fraction {fractionGroup.Key.Name}: {fractionGroup.Count()}");
            }
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
}