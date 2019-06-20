using System;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.Benchmarks
{
    public class CreateGlobeBench
    {
        private IWorldGenerator _generator;

        [Benchmark(Description = "CreateGlobeBench")]
        public async Task Run()
        {
            await _generator.GenerateGlobeAsync();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            var serviceCollection = new ServiceCollection();

            // инстанцируем явно, чтобы обеспечить одинаковый рандом для всех запусков тестов.
            serviceCollection.AddSingleton<IDice>(factory => new Dice(1));
            serviceCollection.AddSingleton<ISchemeLocator>(factory => CreateSchemeLocator());
            serviceCollection.AddSingleton<ISchemeService, SchemeService>();
            serviceCollection.AddSingleton<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>();

            // Для мира
            serviceCollection.AddSingleton<IWorldGenerator, WorldGenerator>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _generator = serviceProvider.GetRequiredService<IWorldGenerator>();
        }

        private FileSchemeLocator CreateSchemeLocator()
        {
            var schemePath = Environment.GetEnvironmentVariable("zilon_SchemeCatalog");
            var schemeLocator = new FileSchemeLocator(schemePath);
            return schemeLocator;
        }
    }
}
