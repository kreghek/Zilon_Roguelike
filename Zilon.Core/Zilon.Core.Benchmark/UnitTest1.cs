using System;
using System.Configuration;

using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
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
                Add(ConsoleLogger.Default);
                Add(TargetMethodColumn.Method, StatisticColumn.Mean, StatisticColumn.StdDev);
                Add(new JsonExporter(fileNameSuffix: $"-{buildNumber}"));
                Add(EnvironmentAnalyser.Default);
                UnionRule = ConfigUnionRule.AlwaysUseLocal;
                ArtifactsPath = ConfigurationManager.AppSettings["BenchArtifactsPath"];

            }
        }
    }
}
