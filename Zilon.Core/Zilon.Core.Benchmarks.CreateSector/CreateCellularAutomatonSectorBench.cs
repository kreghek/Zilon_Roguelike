using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Benchmarks.CreateSector
{
    public class CreateCellularAutomatonSectorBench
    {
        private ServiceProvider _serviceProvider;

        [Benchmark(Description = "Create CA Sector")]
        public async Task CreateSectorAsync()
        {
            var biomInitializer = _serviceProvider.GetRequiredService<IBiomeInitializer>();
            var schemeService = _serviceProvider.GetRequiredService<ISchemeService>();

            var testScheme = schemeService.GetScheme<ILocationScheme>("intro");

            await biomInitializer.InitBiomeAsync(testScheme).ConfigureAwait(false);
        }


        [IterationSetup]
        public void IterationSetup()
        {
            var startUp = new Startup();
            var serviceCollection = new ServiceCollection();
            startUp.RegisterServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}