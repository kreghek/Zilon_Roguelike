using System.Linq;

using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;

namespace Zilon.Core.Benchmarks
{
    internal class Config : ManualConfig
    {
        public Config(string buildNumber, int iterationCount, string monoRuntimePath, string benchArtifactsPath)
        {
            var monoRuntimeName = "Mono";

            //Add(Job.Default.With(Runtime.Clr).With(Platform.X64).With(Jit.LegacyJit).WithIterationCount(iterationCount));
            //Add(Job.Default.With(Runtime.Clr).With(Platform.X64).With(Jit.RyuJit).WithIterationCount(iterationCount));
            //Add(Job.Default.With(new MonoRuntime(monoRuntimeName, monoRuntimePath)).WithIterationCount(iterationCount));
            Add(Job.Default.With(Runtime.Core).With(Platform.X64).With(Jit.RyuJit).WithIterationCount(iterationCount));
            //Add(Job.Default.With(Runtime.CoreRT).With(Platform.X64).With(Jit.RyuJit).WithIterationCount(iterationCount));

            Add(ConsoleLogger.Default);
            Add(TargetMethodColumn.Method,
                JobCharacteristicColumn.AllColumns.Single(x => x.ColumnName == "Runtime"),
                JobCharacteristicColumn.AllColumns.Single(x => x.ColumnName == "Jit"),
                StatisticColumn.Mean,
                StatisticColumn.Median,
                StatisticColumn.StdDev);
            Add(new JsonExporter(fileNameSuffix: $"-{buildNumber}", indentJson: true, excludeMeasurements: false));
            Add(EnvironmentAnalyser.Default);
            UnionRule = ConfigUnionRule.AlwaysUseLocal;
            ArtifactsPath = benchArtifactsPath;
        }
    }
}
