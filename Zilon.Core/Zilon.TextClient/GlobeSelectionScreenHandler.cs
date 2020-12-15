using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.World;

namespace Zilon.TextClient
{
    /// <summary>
    /// Screen to work with globe generation and selection.
    /// Potentially, game may have multiple globes. And the user can select different globe to play.
    /// </summary>
    internal class GlobeSelectionScreenHandler : IScreenHandler
    {
        /// <inheritdoc/>
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