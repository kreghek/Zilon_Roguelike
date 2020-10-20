using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Tactics;

namespace Zilon.Core.Benchmarks.CreateSector
{
    public class CreateCellularAutomatonSectorBench
    {
        private ServiceProvider _serviceProvider;

        [Benchmark(Description = "Create CA Sector")]
        public async System.Threading.Tasks.Task CreateSectorAsync()
        {
            var sectorManager = _serviceProvider.GetRequiredService<ISectorManager>();

            await sectorManager.CreateSectorAsync().ConfigureAwait(false);
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