using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks.Move
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