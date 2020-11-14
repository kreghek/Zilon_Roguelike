using System.Linq;

using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.Benchmarks.CreateSector
{
    public class CreateCellularAutomatonSectorBench
    {
        private ServiceProvider _serviceProvider;

        [Benchmark(Description = "Create CA Sector")]
        public async System.Threading.Tasks.Task CreateSectorAsync()
        {
            var sectorGenerator = _serviceProvider.GetRequiredService<ISectorGenerator>();
            var biomInitializer = _serviceProvider.GetRequiredService<IBiomeInitializer>();
            var schemeService = _serviceProvider.GetRequiredService<ISchemeService>();

            var testScheme = schemeService.GetScheme<ILocationScheme>("intro");

            var biom = await biomInitializer.InitBiomeAsync(testScheme);
            var tesSectorNode = biom.Sectors.First();

            await sectorGenerator.GenerateAsync(tesSectorNode).ConfigureAwait(false);
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