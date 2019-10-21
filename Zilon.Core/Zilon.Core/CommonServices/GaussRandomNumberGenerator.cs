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
        private const double STDDEV = 0.25;

        /// <summary>
        /// Математическое ожидание.
        /// </summary>
        private const double MEAN = 0.5;
        
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
                var rand = GetNext(_dice, 0.0, 1.0);

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

        private static double GetNext(IDice dice, double min, double max)
        {
            double x;
            do
            {
                x = NextGaussian(dice, MEAN, STDDEV);
            } while (x < min || x > max);
            return x;
        }

        private static double NextGaussian(IDice dice, double mean, double stdDev)
        {
            var u1 = GetNextDouble(dice); //these are uniform(0,1) random doubles
            var u2 = GetNextDouble(dice);
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            var randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }
    }
}
