using BenchmarkDotNet.Running;

using NUnit.Framework;

namespace Zilon.Core.Benchmark
{
    [TestFixture]
    [Category("benckmark")]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            BenchmarkRunner.Run<TheEasiestBenchmark>();
        }
    }
}
