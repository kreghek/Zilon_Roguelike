using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks.Move
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args,
                config: new DebugInProcessConfig());
#else
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
#endif
        }

        static IConfig GetGlobalConfig()
            => DefaultConfig.Instance
            .AddJob(Job.Default
                .WithWarmupCount(1)
                .WithInvocationCount(10)
                .AsDefault()); // the KEY to get it working
    }
}