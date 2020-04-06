using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks.CreateSector
{
    static class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
