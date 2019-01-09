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
using BenchmarkDotNet.Running;

using NUnit.Framework;

namespace Zilon.Core.Benchmark
{
    [TestFixture]
    [Category("benchmark")]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            var buildNumber = TestContext.Parameters["buildNumber"];

            Console.WriteLine($"buildNumber: {buildNumber}");

            var config = new Config(buildNumber);
            BenchmarkRunner.Run<TheEasiestBenchmark>(config);
        }

        private class Config : ManualConfig
        {
            public Config(string buildNumber)
            {
                var monoRuntimeName = ConfigurationManager.AppSettings["MonoRuntimeName"];
                var monoRuntimePath = ConfigurationManager.AppSettings["MonoRuntimePath"];

                Add(Job.Clr);
                Add(Job.ShortRun.With(new MonoRuntime(monoRuntimeName, monoRuntimePath)));

                Add(ConsoleLogger.Default);
                Add(TargetMethodColumn.Method, JobCharacteristicColumn.AllColumns.Single(x=>x.ColumnName == "Runtime"), StatisticColumn.Mean, StatisticColumn.StdDev);
                Add(new JsonExporter(fileNameSuffix: $"-{buildNumber}", indentJson: true, excludeMeasurements: false));
                Add(EnvironmentAnalyser.Default);
                UnionRule = ConfigUnionRule.AlwaysUseLocal;
                ArtifactsPath = ConfigurationManager.AppSettings["BenchArtifactsPath"];
            }

            private object Mono()
            {
                throw new NotImplementedException();
            }
        }
    }
}
