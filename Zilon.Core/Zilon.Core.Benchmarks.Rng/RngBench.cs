using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Benchmarks.Rng
{
    public class RngBench
    {
        [Benchmark(Description = "Rng")]
        public void Rng()
        {
            var dice = new LinearDice(1);

            var seq = new int[1000];
            for (var i = 0; i < seq.Length; i++)
            {
                seq[i] = dice.Roll(100);
            }

            var gr = seq.GroupBy(x => x);
            var freq = gr.ToDictionary(x => x.Key, x => x.Count()).OrderBy(x => x.Key);
            foreach (var fr in freq)
            {
                Console.WriteLine(fr.Key + "\t" + fr.Value);
            }
        }

    }
}
