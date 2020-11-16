using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks.Fow
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
                             .Run(args);
        }
    }
}