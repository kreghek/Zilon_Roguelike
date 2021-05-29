using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Sdk;
using Zilon.Core.World;
using Zilon.Emulation.Common;

namespace Zilon.Core.Benchmarks.MassGameplay
{
    [MemoryDiagnoser]
    public class MassGamePlayBench
    {
        [Benchmark(Description = "Mass Game Play 40")]
        [SuppressMessage("Performance",
            "CA1822:Mark members as static",
            Justification = "Benchmarks MUST be instance methods, static methods are not supported.")]
        public async Task GamePlayBenchAsync()
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new AutoPersonStartup();
            startUp.RegisterServices(serviceContainer);
            var serviceProvider = serviceContainer.BuildServiceProvider();

            var botSettings = new BotSettings { Mode = "duncan" };

            var globeInitializer = serviceProvider.GetRequiredService<IGlobeInitializer>();

            var autoPlayEngine = new AutoplayEngine(
                startUp,
                botSettings,
                globeInitializer);

            var globe = await autoPlayEngine.CreateGlobeAsync().ConfigureAwait(false);

            var context = new MassAutoplayContext(globe);

            await autoPlayEngine.StartAsync(globe, context).ConfigureAwait(false);
        }
    }
}