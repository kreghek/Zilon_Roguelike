﻿using System;

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Zilon.Core.Benchmarks.Fow
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
    }
}