using System;
using BenchmarkDotNet.Running;

namespace Zilon.Core.Benckmarks.Fow
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
