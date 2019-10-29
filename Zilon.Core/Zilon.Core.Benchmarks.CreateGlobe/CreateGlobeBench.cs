using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using LightInject;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.WorldGeneration;
using Zilon.IoC;

namespace Zilon.Core.Benchmark
{
    public class CreateGlobeBench
    {
        private IWorldGenerator _generator;

        [Params(1, 10)]
        public int SequenceSize;

        [Benchmark(Description = "CreateGlobeBench")]
        public async Task Run()
        {
            for (int i = 0; i < SequenceSize; i++)
            {
                await _generator.GenerateGlobeAsync().ConfigureAwait(false);
            }
        }

        [IterationSetup]
        public void IterationSetup()
        {
            using (var container = new ServiceContainer())
            {
                // инстанцируем явно, чтобы обеспечить одинаковый рандом для всех запусков тестов.
                container.Register<IDice>(factory => new LinearDice(1), LightInjectWrapper.CreateSingleton());
                container.Register(factory => BenchHelper.CreateSchemeLocator(), LightInjectWrapper.CreateSingleton());
                container.Register<ISchemeService, SchemeService>(LightInjectWrapper.CreateSingleton());
                container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(LightInjectWrapper.CreateSingleton());

                // Для мира
                container.Register<IWorldGenerator, WorldGenerator>(LightInjectWrapper.CreateSingleton());

                _generator = container.GetInstance<IWorldGenerator>();
            }
        }
    }
}
