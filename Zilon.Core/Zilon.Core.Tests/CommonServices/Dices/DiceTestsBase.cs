using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tests.CommonServices.Dices
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public abstract class DiceTestsBase
    {
        protected abstract IDice CreateDice(int seed);

        /// <summary>
        ///     Тест проверяет, что при разных зернах генерации не происходит ошибки получения случайного числа.
        /// </summary>
        /// <param name="seed"> Зерно рандомизации для кости. </param>
        /// <param name="n"> Количество граней у кости. </param>
        /// <param name="count"> Количество результатов, которое нужно получить в течении теста. </param>
        [Test]
        public void NextTest([Values(1, 100, 300, int.MaxValue)] int seed,
            [Values(1, 6, 100)] int n,
            [Values(1, 10, 100, 1000)] int count)
        {
            // ARRANGE
            IDice dice = CreateDice(seed);

            // ACT
            int[] seq = new int[count];
            Action act = () =>
            {
                for (int i = 0; i < seq.Length; i++)
                {
                    seq[i] = dice.Roll(n);
                }

                IEnumerable<IGrouping<int, int>> gr = seq.GroupBy(x => x);
                IOrderedEnumerable<KeyValuePair<int, int>> freq = gr.ToDictionary(x => x.Key, x => x.Count())
                    .OrderBy(x => x.Key);
                foreach (KeyValuePair<int, int> fr in freq)
                {
                    Console.WriteLine(fr.Key + "\t" + fr.Value);
                }
            };

            // ASSERT
            act.Should().NotThrow();
            seq.Min().Should().BeGreaterOrEqualTo(1);
            seq.Max().Should().BeLessOrEqualTo(n);
        }
    }
}