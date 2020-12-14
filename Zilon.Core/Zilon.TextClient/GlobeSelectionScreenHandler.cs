using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.World;

namespace Zilon.TextClient
{
    internal class GlobeSelectionScreenHandler: IScreenHandler
    {
        public async Task<GameScreen> StartProcessingAsync(IServiceScope serviceScope)
        {
            var globeInitializer = serviceScope.ServiceProvider.GetRequiredService<IGlobeInitializer>();
            var globe = await globeInitializer.CreateGlobeAsync("intro");

            Console.WriteLine("Globe created");
            Console.WriteLine($"Nodes: {globe.SectorNodes.Count()}");
            Console.WriteLine($"Persons: {globe.SectorNodes.Select(x=>x.Sector).SelectMany(x=>x.ActorManager.Items).Count()}");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();

            return GameScreen.Main;
        }
    }
}