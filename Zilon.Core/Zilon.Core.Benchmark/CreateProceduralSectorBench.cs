using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using LightInject;

using Zilon.Core.MapGenerators;

namespace Zilon.Core.Benchmark
{
    public class CreateProceduralSectorBench: CreateSectorBenchBase
    {
        [Benchmark(Description = "CreateProceduralSector")]
        public Task CreateAsync()
        {
            return CreateInnerAsync();
        }

        protected override IChestGeneratorRandomSource CreateChestGeneratorRandomSource()
        {
            return _container.GetInstance<IChestGeneratorRandomSource>();
        }

        protected override IMonsterGeneratorRandomSource CreateFakeMonsterGeneratorRandomSource()
        {
            return _container.GetInstance<IMonsterGeneratorRandomSource>();
        }
    }
}
