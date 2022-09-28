using System.Linq;

using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;

namespace Zilon.Core.Benchmark
{
    public class Config : ManualConfig
    {
        public Config(string buildNumber, int iterationCount, string artifactPath)
        {
            AddJob(Job.Default.WithRuntime(CoreRuntime.Core60).WithPlatform(Platform.X64).WithJit(Jit.RyuJit)
                .WithIterationCount(iterationCount));

            AddLogger(ConsoleLogger.Default);
            AddColumn(TargetMethodColumn.Method,
                JobCharacteristicColumn.AllColumns.Single(x => x.ColumnName == "Runtime"),
                JobCharacteristicColumn.AllColumns.Single(x => x.ColumnName == "Jit"),
                StatisticColumn.Mean,
                StatisticColumn.Median,
                StatisticColumn.StdDev);
            AddExporter(new JsonExporter(fileNameSuffix: $"-{buildNumber}", indentJson: true, excludeMeasurements: false));
            AddAnalyser(EnvironmentAnalyser.Default);
            UnionRule = ConfigUnionRule.AlwaysUseLocal;
            ArtifactsPath = artifactPath;
        }
    }
}