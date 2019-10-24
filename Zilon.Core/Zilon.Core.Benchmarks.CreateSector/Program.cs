using BenchmarkDotNet.Running;

using Zilon.Core.Benchmark;
using Zilon.Core.Benchmarks.Common;

namespace Zilon.Core.Benchmarks.CreateSector
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConsoleApplicationConfigCreator.CreateBenchConfig(args);
            BenchmarkRunner.Run<CreateProceduralSectorMinBench>(config);
            BenchmarkRunner.Run<CreateProceduralSectorBench>(config);
            BenchmarkRunner.Run<CreateProceduralSectorMaxBench>(config);
        }
    }
}
