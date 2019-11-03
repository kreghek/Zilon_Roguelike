using BenchmarkDotNet.Running;

using Zilon.Core.Benchmarks.Common;

namespace Zilon.Core.Benchmarks.NetCore.Move
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConsoleApplicationConfigCreator.CreateBenchConfig(args);
            BenchmarkRunner.Run<MoveBench>(config);
        }
    }
}
