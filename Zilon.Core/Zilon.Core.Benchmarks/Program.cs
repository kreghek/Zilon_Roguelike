using System;

using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var schemePath = GetProgramArgument(args, "SchemeCatalog");

            var monoPath = GetProgramArgument(args, "MonoPath");
            var artefactsPath = GetProgramArgument(args, "ArtefactPaths");
            var iterationCount = int.Parse(GetProgramArgument(args, "IterationCount"));

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

        private static string GetProgramArgument(string[] args, string testArg)
        {
            if (args == null)
            {
                return null;
            }

            foreach (var arg in args)
            {
                var components = arg.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (string.Equals(components[0], testArg, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (components.Length >= 2)
                    {
                        return components[1];
                    }
                }
            }

            return null;
        }
    }
}
