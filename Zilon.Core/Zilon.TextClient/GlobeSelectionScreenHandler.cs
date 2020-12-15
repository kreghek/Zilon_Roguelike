using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.World;

namespace Zilon.TextClient
{
    internal class GlobeSelectionScreenHandler : IScreenHandler
    {
        public async Task<GameScreen> StartProcessingAsync(GameState gameState)
        {
            var serviceScope = gameState.ServiceScope;
            var globeInitializer = serviceScope.ServiceProvider.GetRequiredService<IGlobeInitializer>();
            var globe = await globeInitializer.CreateGlobeAsync("intro").ConfigureAwait(false);

            Console.WriteLine(UiResource.GlobeGenerationCompleteMessage);

            var sectors = globe.SectorNodes.Select(x => x.Sector);
            Console.WriteLine($"{UiResource.NodesLabel}: {sectors.Count()}");

            var persons = sectors.SelectMany(x => x.ActorManager.Items);
            Console.WriteLine($"{UiResource.PersonsLabel}: {persons.Count()}");

            Console.WriteLine(UiResource.PressEnterToContinuePropmpt);

            Console.ReadLine();

            return GameScreen.Main;
        }
    }
}