using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks.Move
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args,
                config: GetSpecifiedConfig());
        }

        static IConfig GetSpecifiedConfig()
            => GetConfigInstance()
            .AddJob(Job.Default
                .WithWarmupCount(1)
                .WithInvocationCount(10)
                .WithUnrollFactor(1)
                .AsDefault()); // the KEY to get it working

        static IConfig GetConfigInstance()
        {
#if DEBUG
            return new DebugInProcessConfig();
#else
            return DefaultConfig.Instance;
#endif
        }
    }
}