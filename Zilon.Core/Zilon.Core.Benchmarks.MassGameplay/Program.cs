using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks.Move
{
    internal static class Program
    {
        private static IConfig GetConfigInstance()
        {
#if DEBUG
            return new DebugInProcessConfig();
#else
            return DefaultConfig.Instance;
#endif
        }

        private static IConfig GetSpecifiedConfig()
        {
            return GetConfigInstance()
                .AddJob(Job.Default
                    .WithWarmupCount(1)
                    .WithInvocationCount(1)
                    .WithUnrollFactor(1)
                    .WithIterationCount(10)
                    .AsDefault()); // the KEY to get it working
        }

        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args,
                config: GetSpecifiedConfig());
        }
    }
}