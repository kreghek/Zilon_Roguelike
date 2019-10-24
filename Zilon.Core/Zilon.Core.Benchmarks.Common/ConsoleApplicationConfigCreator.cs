using System;

using Zilon.CommonUtilities;
using Zilon.Core.Benchmark;

namespace Zilon.Core.Benchmarks.Common
{
    public static class ConsoleApplicationConfigCreator
    {
        public static Config CreateBenchConfig(string[] args)
        {
            var buildNumber = ArgumentHelper.GetProgramArgument(args, "BUILD_NUMBER");
            var iterationCountString = ArgumentHelper.GetProgramArgument(args, "ITERATION_COUNT");
            var iterationCount = int.Parse(iterationCountString);
            var monoName = "mono";
            var monoPath = ArgumentHelper.GetProgramArgument(args, "MONO_PATH");
            var artifactPath = ArgumentHelper.GetProgramArgument(args, "ARTIFACT_PATH");

            Console.WriteLine($"buildNumber: {buildNumber}");

            var config = new Config(buildNumber, iterationCount, monoName, monoPath, artifactPath);

            return config;
        }
    }
}
