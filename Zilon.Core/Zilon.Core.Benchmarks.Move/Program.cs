using System;

using BenchmarkDotNet.Running;

using Zilon.Core.Benchmark;
using Zilon.Core.Benchmarks.Common;

namespace Zilon.Core.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConsoleApplicationConfigCreator.CreateBenchConfig(args);
            BenchmarkRunner.Run<MoveBench>(config);

            Console.ReadLine();
        }
    }
}
