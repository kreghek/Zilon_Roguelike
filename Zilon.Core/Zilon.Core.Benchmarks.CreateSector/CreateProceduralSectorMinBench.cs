using BenchmarkDotNet.Attributes;

using Moq;

using Zilon.Core.MapGenerators;

namespace Zilon.Core.Benchmark
{
    public class CreateProceduralSectorMinBench: CreateSectorBenchBase
    {
        [Benchmark(Description = "CreateProceduralMinSector")]
        public System.Threading.Tasks.Task CreateAsync()
        {
            return CreateInnerAsync();
        }

        protected override IMonsterGeneratorRandomSource CreateFakeMonsterGeneratorRandomSource()
        {
            var mock = new Mock<IMonsterGeneratorRandomSource>();
            mock.Setup(x => x.RollRegionCount(It.IsAny<int>(), It.IsAny<int>())).Returns(0);
            return mock.Object;
        }

        protected override IChestGeneratorRandomSource CreateChestGeneratorRandomSource()
        {
            var mock = new Mock<IChestGeneratorRandomSource>();
            mock.Setup(x => x.RollChestCount(It.IsAny<int>())).Returns(0);
            return mock.Object;
        }
    }
}
