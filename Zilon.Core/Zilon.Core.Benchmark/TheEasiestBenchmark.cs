using System.Linq;

using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;

namespace Zilon.Core.Benchmark
{
    [Config(typeof(Config))]
    public class TheEasiestBenchmark
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                Add(ConsoleLogger.Default);
                Add(TargetMethodColumn.Method, StatisticColumn.Mean);
                Add(CsvExporter.Default);
                Add(EnvironmentAnalyser.Default);
                UnionRule = ConfigUnionRule.AlwaysUseLocal;
                ArtifactsPath = @"c:\benchmarkdotnet";
            }
        }

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
