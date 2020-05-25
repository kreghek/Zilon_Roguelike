using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.World;

namespace Zilon.GlobeObserver
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new StartUp();
            startUp.RegisterServices(serviceContainer);
            var serviceProvider = serviceContainer.BuildServiceProvider();

            // Create globe
            var globeInitializer = serviceProvider.GetRequiredService<IGlobeInitializer>();
            var globe = globeInitializer.CreateGlobeAsync();

            // Iterate globe
            do
            {
                var iterationCount = int.Parse(Console.ReadLine());
                for (var i = 0; i < iterationCount; i++)
                {
                    globe.Update();
                }

            } while (true);
        }
    }
}
