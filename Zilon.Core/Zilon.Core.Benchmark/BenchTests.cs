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
            var buildNumber = TestContext.Parameters["buildNumber"];

            Console.WriteLine($"buildNumber: {buildNumber}");

            var config = new Config(buildNumber);
            BenchmarkRunner.Run<MoveBench>(config);
        }
    }
}
