using System.Configuration;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using LightInject;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.Benchmark
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
            var container = new ServiceContainer();

            // инстанцируем явно, чтобы обеспечить одинаковый рандом для всех запусков тестов.
            container.Register<IDice>(factory => new Dice(1), new PerContainerLifetime());
            container.Register<ISchemeLocator>(factory => CreateSchemeLocator(), new PerContainerLifetime());
            container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());
            container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());

            // Для мира
            container.Register<IWorldGenerator, WorldGenerator>(new PerContainerLifetime());

            _generator = container.GetInstance<IWorldGenerator>();
        }

        private FileSchemeLocator CreateSchemeLocator()
        {
            var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];
            var schemeLocator = new FileSchemeLocator(schemePath);
            return schemeLocator;
        }
    }
}
