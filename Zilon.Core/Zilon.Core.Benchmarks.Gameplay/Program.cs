using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks.Move
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args,
                config: ManualConfig.Create(DefaultConfig.Instance)
                    .WithOption(ConfigOptions.DisableOptimizationsValidator, true)
                    .WithOption(ConfigOptions.KeepBenchmarkFiles, true));
        }
    }
}