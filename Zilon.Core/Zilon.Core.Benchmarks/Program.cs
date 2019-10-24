using System;

using BenchmarkDotNet.Running;

using Zilon.CommonUtilities;
using Zilon.Core.Benchmark;

namespace Zilon.Core.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = CreateBenchConfig(args);

            var benchName = ArgumentHelper.GetProgramArgument(args, "BENCH");
            var benchNameNormalized = benchName.Trim().ToUpperInvariant();

            switch (benchNameNormalized)
            {
                case "MOVE":
                    BenchmarkRunner.Run<MoveBench>(config);
                    break;
            }

            Console.ReadLine();
        }

        private static Config CreateBenchConfig(string[] args)
        {
            var buildNumber = ArgumentHelper.GetProgramArgument(args, "BUILD_NUMBER");
            var iterationCount = 1;
            var monoName = "mono";
            var monoPath = ArgumentHelper.GetProgramArgument(args, "MONO_PATH");
            var artifactPath = ArgumentHelper.GetProgramArgument(args, "ARTIFACT_PATH");

            Console.WriteLine($"buildNumber: {buildNumber}");

            var config = new Config(buildNumber, iterationCount, monoName, monoPath, artifactPath);

            return config;
        }
    }
}
