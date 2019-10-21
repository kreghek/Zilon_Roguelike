using System;

using JetBrains.Annotations;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.CommonServices
{
    /// <summary>
    /// Генератор случайных числе Парка-Миллера.
    /// </summary>
    public sealed class ParkMillerRandomNumberGenerator : IRandomNumberGenerator
    {
        /* The original seed used by this number generator */
        private readonly uint _seed;
        private uint _walkingNumber;

        /// <summary>
        /// Конструктор генератора.
        /// </summary>
        /// <param name="dice"> Игральная кость для выбора случайного зерна генератора. </param>
        public ParkMillerRandomNumberGenerator([NotNull] IDice dice)
        {
            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            var seed = dice.Roll(int.MaxValue - 1);
            _seed = (uint)seed;

            Reset();
        }

        /// <summary>
        /// Конструктор генератора.
        /// </summary>
        /// <param name="seed">Зерно генератора.</param>
        public ParkMillerRandomNumberGenerator(uint seed)
        {
            if (seed == 0)
            {
                throw new ArgumentException("Значение не можут быть меньше или равно 0", nameof(seed));
            }

            _seed = seed;

            Reset();
        }

        /// <summary>
        /// Получение следующего значения последовательности случайных чисел в диапазоне [0..1].
        /// </summary>
        /// <returns> Возвращает случайное число в диапазоне [0..1]. </returns>
        private double Next()
        {
            unchecked
            {
                _walkingNumber = (_walkingNumber * 16807) % 2147483647;
                var a = (double)_walkingNumber / 0x7FFFFFFF;
                var b = a + 0.000000000233;
                return b;
            }
        }

        public double[] GetSequence(int count)
        {
            var sequence = new double[count];
            for (var i = 0; i < count; i++)
            {
                var next = Next();
                sequence[i] = next;
            }

            return sequence;
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
