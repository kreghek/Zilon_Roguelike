using System;

using JetBrains.Annotations;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.CommonServices
{
    /// <summary>
    /// Генератор случайных числе Парка-Миллера.
    /// </summary>
    public sealed class GaussRandomNumberGenerator : IRandomNumberGenerator
    {
        /// <summary>
        /// Погрешность.
        /// </summary>
        private const double SIGMA = 0.25;

        /// <summary>
        /// Математическое ожидание.
        /// </summary>
        private const double MV = 0.5;
        
        private readonly IDice _dice;

        /// <summary>
        /// Конструктор генератора.
        /// </summary>
        /// <param name="dice"> Игральная кость для выбора случайного зерна генератора. </param>
        public GaussRandomNumberGenerator([NotNull] IDice dice)
        {
            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            _dice = dice;
            
        }

        public double[] GetSequence(int count)
        {
            var sequence = new double[count];

            for (int n = 0; n < count; n++)
            {
                var rand = GetNextDouble(_dice);

                sequence[n] = rand;
            }

            return sequence;
        }

        private static double GetNextDouble(IDice dice)
        {
            var maxDiceVal = 1000_000;
            var next = (double)dice.Roll(maxDiceVal) / maxDiceVal;
            return next;
        }

        private static double GetNext(IDice dice)
        {
            double v1, v2, s;
            do
            {
                v1 = 2.0f * GetNextDouble(dice) - 1.0f;
                v2 = 2.0f * GetNextDouble(dice) - 1.0f;
                s = v1 * v1 + v2 * v2;
            } while (s >= 1.0f || s == 0f);

            s = Math.Sqrt((-2.0f * Math.Log(s)) / s);

            return v1 * s;
        }
    }
}
