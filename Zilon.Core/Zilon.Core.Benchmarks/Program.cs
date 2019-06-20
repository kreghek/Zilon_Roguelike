using System;
using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = CreateBenchConfig();
            MoveBench.BenchArgs = args;
            BenchmarkRunner.Run<MoveBench>(config);
            BenchmarkRunner.Run<CreateGlobeBench>(config);
        }

        private static Config CreateBenchConfig()
        {
            var config = new Config(buildNumber: null,
                100,
                @"C:\Program Files\Unity\Hub\Editor\2018.4.1f1\Editor\Data\MonoBleedingEdge\bin\mono.exe",
                @"c:\dotnetbenchmark-reports");

            return config;
        }
    }
}
