using System.Linq;

using BenchmarkDotNet.Attributes;

namespace Zilon.Core.Benchmark
{
    [ArtifactsPath(@"c:\benchmarkdotnet")]
    //[DryJob]
    public class TheEasiestBenchmark
    {
        [Benchmark(Description = "Summ100")]
        public int Test100()
        {
            return Enumerable.Range(1, 100).Sum();
        }

        [Benchmark(Description = "Summ200")]
        public int Test200()
        {
            return Enumerable.Range(1, 200).Sum();
        }
    }
}
