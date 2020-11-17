using System;

using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks.Fow
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}