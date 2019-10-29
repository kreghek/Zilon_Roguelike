using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using LightInject;
using Zilon.Core.CommonServices.Dices;
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
            var linearDice = _container.GetInstance<IDice>();
            return new ChestGeneratorRandomSource(linearDice);
        }

        protected override IMonsterGeneratorRandomSource CreateFakeMonsterGeneratorRandomSource()
        {
            var linearDice = _container.GetInstance<IDice>();
            return new MonsterGeneratorRandomSource(linearDice);
        }
    }
}
