using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Persons;
using Zilon.Core.World;

namespace Zilon.GlobeObserver
{
    internal static class Program
    {
        private static async Task<IGlobe> GenerateGlobeAsync(ServiceProvider serviceProvider)
        {
            // Create globe
            var globeInitializer = serviceProvider.GetRequiredService<IGlobeInitializer>();
            var globe = await globeInitializer.CreateGlobeAsync("intro").ConfigureAwait(false);

            return globe;
        }

        private static async Task Main()
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new StartUp();
            startUp.RegisterServices(serviceContainer);

            using var serviceProvider = serviceContainer.BuildServiceProvider();
            var globe = await GenerateGlobeAsync(serviceProvider).ConfigureAwait(false);

            // Iterate globe
            var globeIterationCounter = 0L;
            do
            {
                Console.WriteLine("Iteratin count:");

                var input = Console.ReadLine();

                if (string.Equals(input, "stop", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Stop to generate the world and print results.

                    PrintReport(globe);

                    break;
                }

                var iterationCount = int.Parse(input, CultureInfo.InvariantCulture);

                for (var i = 0; i < iterationCount; i++)
                {
                    await RunGlobeIteration(globe).ConfigureAwait(false);

                    globeIterationCounter++;

                    var hasActors = globe.SectorNodes.SelectMany(x => x.Sector.ActorManager.Items)
                        .Any(x => x.Person.Fraction != Fractions.MonsterFraction);
                    if (!hasActors)
                    {
                        // There is no human persons.
                        // We can stop update the globe, because monsters can't change the world. Only humans.
                        break;
                    }
                }

                Console.WriteLine($"Globe Iteration: {globeIterationCounter}");
                PrintReport(globe);
            } while (true);
        }

        private static async Task RunGlobeIteration(IGlobe globe)
        {
            for (var i = 0; i < GlobeMetrics.OneIterationLength; i++)
            {
                await globe.UpdateAsync().ConfigureAwait(false);
            }
        }

        private static void PrintReport(IGlobe globe)
        {
            var sectorNodesDiscoveredCount = globe.SectorNodes.Count();
            Console.WriteLine($"Sector Nodes Discovered: {sectorNodesDiscoveredCount}");

            var actorCount = globe.SectorNodes.SelectMany(x => x.Sector.ActorManager.Items).Count();
            Console.WriteLine($"Actors: {actorCount}");

            var fractions = globe.SectorNodes.SelectMany(x => x.Sector.ActorManager.Items)
                .GroupBy(x => x.Person.Fraction);
            foreach (var fractionGroup in fractions)
            {
                Console.WriteLine($"Fraction {fractionGroup.Key.Name}: {fractionGroup.Count()}");
            }
        }
    }
}