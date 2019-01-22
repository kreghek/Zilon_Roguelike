using System;

using BenchmarkDotNet.Running;

using NUnit.Framework;

namespace Zilon.Core.Benchmark
{
    [TestFixture]
    [Category("benchmark")]
    public class BenchTests
    {
        [Test]
        public void Move()
        {
            var config = CreateBenchConfig();
            BenchmarkRunner.Run<MoveBench>(config);
        }

        [Test]
        public void CreateProceduralMinSector()
        {
            var config = CreateBenchConfig();
            BenchmarkRunner.Run<CreateProceduralSectorMinBench>(config);
        }

        [Test]
        public void CreateProceduralSector()
        {
            var config = CreateBenchConfig();
            BenchmarkRunner.Run<CreateProceduralSectorBench>(config);
        }

        [Test]
        public void CreateProceduralMaxSector()
        {
            var config = CreateBenchConfig();
            BenchmarkRunner.Run<CreateProceduralSectorMaxBench>(config);
        }

        [Test]
        public void CreateProceduralMaxSector2()
        {
            var bench = new CreateProceduralSectorMaxBench();
            bench.IterationSetup();
            bench.Create();
        }

        private Config CreateBenchConfig() {
            var buildNumber = TestContext.Parameters["buildNumber"];

            Console.WriteLine($"buildNumber: {buildNumber}");

            var config = new Config(buildNumber);

            return config;
        }
    }
}
