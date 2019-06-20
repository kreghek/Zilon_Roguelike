using System;
using System.Configuration;
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
    internal class Config : ManualConfig
    {
        public Config(string buildNumber)
        {
            var iterationCount = int.Parse(ConfigurationManager.AppSettings["IterationCount"]);
            var monoRuntimeName = ConfigurationManager.AppSettings["MonoRuntimeName"];
            var monoRuntimePath = ConfigurationManager.AppSettings["MonoRuntimePath"];

            Add(Job.Default.With(Runtime.Clr).With(Platform.X64).With(Jit.LegacyJit).WithIterationCount(iterationCount));
            Add(Job.Default.With(Runtime.Clr).With(Platform.X64).With(Jit.RyuJit).WithIterationCount(iterationCount));
            Add(Job.Default.With(new MonoRuntime(monoRuntimeName, monoRuntimePath)).WithIterationCount(iterationCount));

            // В этих бенчах корку не удаётся запустить.
            // Предположительно, не получается найти инструмент для сборки бенча под корку.
            // Раскомментировать, когда будет ясно, как исправить.
            //Add(Job.Default.With(Runtime.Core).With(Platform.X64).With(Jit.RyuJit).WithIterationCount(iterationCount));
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
            ArtifactsPath = ConfigurationManager.AppSettings["BenchArtifactsPath"];
        }
    }
}
