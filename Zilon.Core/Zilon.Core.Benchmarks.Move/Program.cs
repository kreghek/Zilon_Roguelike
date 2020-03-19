using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks.Move
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
