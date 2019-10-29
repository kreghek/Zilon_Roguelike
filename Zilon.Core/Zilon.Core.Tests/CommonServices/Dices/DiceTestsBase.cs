using System;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tests.CommonServices.Dices
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public abstract class DiceTestsBase
    {
        protected abstract IDice CreateDice(int seed);

        /// <summary>
        /// Тест проверяет, что при разных зернах генерации не происходит ошибки получения случайного числа.
        /// </summary>
        /// <param name="seed"> Зерно рандомизации для кости. </param>
        /// <param name="n"> Количество граней у кости. </param>
        /// <param name="count"> Количество результатов, которое нужнополучить в течении теста. </param>
        [Test]
        public void NextTest([Values(1, 100, 300, int.MaxValue)]int seed,
            [Values(1, 6, 100)] int n,
            [Values(1, 10, 100, 1000)] int count)
        {
            // ARRANGE
            var dice = CreateDice(seed);

            // ACT
            Action act = () =>
            {
                var seq = new int[count];
                for (var i = 0; i < seq.Length; i++)
                {
                    seq[i] = dice.Roll(n);
                }
                var gr = seq.GroupBy(x => x);
                var freq = gr.ToDictionary(x => x.Key, x => x.Count()).OrderBy(x => x.Key);
                foreach (var fr in freq)
                {
                    Console.WriteLine(fr.Key + "\t" + fr.Value);
                }
            };

            // ASSERT
            act.Should().NotThrow();
        }
    }
}