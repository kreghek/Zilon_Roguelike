using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Persons;
using Zilon.Core.World;

namespace Zilon.GlobeObserver
{
    static class Program
    {
        static async Task Main()
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new StartUp();
            startUp.RegisterServices(serviceContainer);

            using var serviceProvider = serviceContainer.BuildServiceProvider();
            var globe = await GenerateGlobeAsync(serviceProvider);

            // Iterate globe
            var globeIterationCounter = 0L;
            do
            {
                Console.WriteLine("Iteratin count:");
                var iterationCount = int.Parse(Console.ReadLine());
                for (var i = 0; i < iterationCount; i++)
                {
                    for (var iterationPassIndex = 0; iterationPassIndex < GlobeMetrics.OneIterationLength; iterationPassIndex++)
                    {
                        await globe.UpdateAsync();
                    }

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

        private static async Task<IGlobe> GenerateGlobeAsync(ServiceProvider serviceProvider)
        {
            // Create globe
            var globeInitializer = serviceProvider.GetRequiredService<IGlobeInitializer>();
            var globe = await globeInitializer.CreateGlobeAsync("intro");

            return globe;
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
}