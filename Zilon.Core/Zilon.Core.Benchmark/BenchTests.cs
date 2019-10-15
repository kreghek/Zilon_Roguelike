using System;

using BenchmarkDotNet.Running;

using NUnit.Framework;

namespace Zilon.Core.Benchmark
{
    [TestFixture][Parallelizable(ParallelScope.All)]
    public class BenchTests
    {
        [Test]
        [Category("benchmark")]
        public void Move()
        {
            var config = CreateBenchConfig();
            BenchmarkRunner.Run<MoveBench>(config);
        }

        [Test]
        [Category("benchmark")]
        public void CreateProceduralMinSector()
        {
            var config = CreateBenchConfig();
            BenchmarkRunner.Run<CreateProceduralSectorMinBench>(config);
        }

        [Test]
        [Category("benchmark")]
        public void CreateProceduralSector()
        {
            var config = CreateBenchConfig();
            BenchmarkRunner.Run<CreateProceduralSectorBench>(config);
        }

        [Test]
        [Category("benchmark")]
        public void CreateProceduralMaxSector()
        {
            var config = CreateBenchConfig();
            BenchmarkRunner.Run<CreateProceduralSectorMaxBench>(config);
        }

        [Test]
        [Category("benchmark")]
        public void CreateGlobe()
        {
            var config = CreateBenchConfig();
            BenchmarkRunner.Run<CreateGlobeBench>(config);
        }

        [Test]
        public void Move1_TestAsync()
        {
            var bench = new MoveBench();
            bench.IterationSetup();
            bench.Move1();
        }

        [Test]
        public void Move100_TestAsync()
        {
            var bench = new MoveBench();
            bench.IterationSetup();
            bench.Move100();
        }

        [Test]
        public async System.Threading.Tasks.Task CreateProceduralSector_TestAsync()
        {
            var bench = new CreateProceduralSectorBench();
            bench.IterationSetup();
            await bench.CreateAsync();
        }

        [Test]
        public async System.Threading.Tasks.Task CreateProceduralMinSector_TestAsync()
        {
            var bench = new CreateProceduralSectorMinBench();
            bench.IterationSetup();
            await bench.CreateAsync();
        }

        [Test]
        public async System.Threading.Tasks.Task CreateProceduralMaxSector_TestAsync()
        {
            var bench = new CreateProceduralSectorMaxBench();
            bench.IterationSetup();
            await bench.CreateAsync();
        }

        [Test]
        public async System.Threading.Tasks.Task CreateGlobe_TestAsync()
        {
            var bench = new CreateGlobeBench();
            bench.IterationSetup();
            await bench.Run();
        }

        private Config CreateBenchConfig()
        {
            var buildNumber = TestContext.Parameters["buildNumber"];

            Console.WriteLine($"buildNumber: {buildNumber}");

            var config = new Config(buildNumber);

            return config;
        }
    }
}
