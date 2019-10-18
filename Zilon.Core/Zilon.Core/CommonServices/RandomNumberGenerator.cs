using System;

using JetBrains.Annotations;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.CommonServices
{
    /// <summary>
    /// Генератор случайных числе Парка-Миллера.
    /// </summary>
    public sealed class RandomNumberGenerator : IRandomNumberGenerator
    {
        /* The original seed used by this number generator */
        private readonly uint _seed;
        private uint _walkingNumber;

        /// <summary>
        /// Конструктор генератора.
        /// </summary>
        /// <param name="dice"> Игральная кость для выбора случайного зерна генератора. </param>
        public RandomNumberGenerator([NotNull] IDice dice)
        {
            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            var seed = dice.Roll(int.MaxValue);
            _seed = (uint)seed;

            Reset();
        }

        /// <summary>
        /// Конструктор генератора.
        /// </summary>
        /// <param name="seed">Зерно генератора.</param>
        public RandomNumberGenerator(uint seed)
        {
            _seed = seed;

            Reset();
        }

        /// <summary>
        /// Получение следующего значения последовательности случайных чисел в диапазоне [0..1].
        /// </summary>
        /// <returns> Возвращает случайное число в диапазоне [0..1]. </returns>
        public double Next()
        {
            unchecked
            {
                _walkingNumber = (_walkingNumber * 16807) % 2147483647;
                var a = (double)_walkingNumber / 0x7FFFFFFF;
                var b = a + 0.000000000233;
                return b;
            }
        }

        /// <summary>
        /// Сброс генератора к стартовому состоянию.
        /// </summary>
        /// <remarks>
        /// После сброса последовательность случайных числе будет генерироваться заново.
        /// </remarks>
        public void Reset()
        {
            _walkingNumber = _seed;
        }
    }
}
