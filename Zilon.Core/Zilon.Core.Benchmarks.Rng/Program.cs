using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using Zilon.Core.Benchmarks.Common;

namespace Zilon.Core.Benchmarks.Rng
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConsoleApplicationConfigCreator.CreateBenchConfig(args);
            BenchmarkRunner.Run<RngBench>(config);
        }
    }
}
