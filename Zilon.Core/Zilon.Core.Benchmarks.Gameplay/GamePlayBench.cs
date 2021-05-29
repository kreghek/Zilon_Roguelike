using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Sdk;
using Zilon.Core.Players;
using Zilon.Core.World;
using Zilon.Emulation.Common;

namespace Zilon.Core.Benchmarks.Move
{
    [MemoryDiagnoser]
    public class GamePlayBench
    {
        [Benchmark(Description = "GamePlay")]
        [SuppressMessage("Performance",
            "CA1822:Mark members as static",
            Justification = "Benchmarks MUST be instance methods, static methods are not supported.")]
        public async Task GamePlayBenchAsync()
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new Startup();
            startUp.RegisterServices(serviceContainer);
            var serviceProvider = serviceContainer.BuildServiceProvider();

            var botSettings = new BotSettings { Mode = "duncan" };

            var globeInitializer = serviceProvider.GetRequiredService<IGlobeInitializer>();
            var player = serviceProvider.GetRequiredService<IPlayer>();

            var autoPlayEngine = new AutoplayEngine(
                startUp,
                botSettings,
                globeInitializer);

            var globe = await autoPlayEngine.CreateGlobeAsync().ConfigureAwait(false);
            var followedPerson = player.MainPerson;

            var autoplayContext = new FollowSinglePersonAutoplayContext(followedPerson);

            await autoPlayEngine.StartAsync(globe, autoplayContext).ConfigureAwait(false);
        }
    }
}