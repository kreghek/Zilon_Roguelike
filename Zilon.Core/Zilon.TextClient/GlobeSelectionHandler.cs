using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.World;

namespace Zilon.TextClient
{
    internal class GlobeSelectionHandler
    {
        public async Task<GameScreen> StartProcessingAsync(IServiceScope serviceScope)
        {
            var globeInitializer = serviceScope.ServiceProvider.GetRequiredService<IGlobeInitializer>();
            _ = await globeInitializer.CreateGlobeAsync("intro");

            return GameScreen.Main;
        }
    }
}