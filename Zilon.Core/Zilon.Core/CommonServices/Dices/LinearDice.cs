using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    /// Реализация игральной кости, которая выбрасывает числа по линейному закону.
    /// </summary>
    public class LinearDice : IDice
    {
        private readonly Random _random;

        /// <summary>
        /// Конструктор кости.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public LinearDice()
        {
            _random = new Random();
        }

        /// <summary>
        /// Конструктор кости.
        /// </summary>
        /// <param name="seed"> Зерно рандомизации. </param>
        /// <remarks>
        /// При одном и том же зерне рандомизации будет генерироваться
        /// одна и та же последовательность случайных чисел.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        public LinearDice(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>
        /// Возвращает результат броска n-гранной кости. Минимальное значение будет 1.
        /// </summary>
        /// <param name="n"> Количество граней кости. </param>
        /// <returns> Результат броска. </returns>
        public int Roll(int n)
        {
            var rollResult = _random.Next(1, n + 1);
            return rollResult;
        }
    }
}