using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;

using Moq;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;

namespace Zilon.Core.Benchmark
{
    public class CreateProceduralSectorMaxBench: CreateSectorBenchBase
    {
        [Benchmark(Description = "CreateProceduralMaxSector")]
        public System.Threading.Tasks.Task CreateAsync()
        { 
           return CreateInnerAsync();
        }

        protected override IMonsterGeneratorRandomSource CreateFakeMonsterGeneratorRandomSource()
        {
            var mock = new Mock<IMonsterGeneratorRandomSource>();
            mock.Setup(x => x.RollRegionCount(It.IsAny<int>(), It.IsAny<int>())).Returns(5);
            mock.Setup(x => x.RollMonsterScheme(It.IsAny<IEnumerable<IMonsterScheme>>()))
                .Returns<IEnumerable<IMonsterScheme>>(sids => sids.First());
            mock.Setup(x => x.RollRarity()).Returns(2);
            return mock.Object;
        }

        protected override IChestGeneratorRandomSource CreateChestGeneratorRandomSource()
        {
            var mock = new Mock<IChestGeneratorRandomSource>();
            mock.Setup(x => x.RollChestCount(It.IsAny<int>())).Returns<int>(n => n);
            mock.Setup(x => x.RollNodeIndex(It.IsAny<int>())).Returns(0);
            return mock.Object;
        }
    }
}
