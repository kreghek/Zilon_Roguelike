using System;

using BenchmarkDotNet.Running;
using Zilon.CommonUtilities;

namespace Zilon.Core.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var schemePath = ArgumentHelper.GetProgramArgument(args, "SchemeCatalog");

            var monoPath = ArgumentHelper.GetProgramArgument(args, "MonoPath");
            var artefactsPath = ArgumentHelper.GetProgramArgument(args, "ArtefactsPath");
            var iterationCount = int.Parse(ArgumentHelper.GetProgramArgument(args, "IterationCount"));

            var config = CreateBenchConfig(monoPath, artefactsPath, schemePath, iterationCount);
            BenchmarkRunner.Run<MoveBench>(config);
            BenchmarkRunner.Run<CreateGlobeBench>(config);
        }

        private static Config CreateBenchConfig(string monoPath, string artefactsPath, string schemePath, int iterationCount)
        {
            var config = new Config(buildNumber: null,
                iterationCount,
                monoPath,
                artefactsPath,
                schemePath);

            return config;
        }
    }
}
