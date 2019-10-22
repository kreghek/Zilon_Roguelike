using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.CommonServices
{
    /// <summary>
    /// Генератор случайных числе Парка-Миллера.
    /// </summary>
    public sealed class GaussDice : IDice
    {
        /// <summary>
        /// Погрешность.
        /// </summary>
        private const double STDDEV = 0.25;

        /// <summary>
        /// Математическое ожидание.
        /// </summary>
        private const double MEAN = 0.5;
        
        private readonly Random _random;

        /// <summary>
        /// Конструктор генератора.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public GaussDice()
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
        public GaussDice(int seed)
        {
            _random = new Random(seed);
        }


        public int Roll(int n)
        {
            var rand = GetNext(0.0, 1.0);
            var roll = DiceValuesHelper.MapDoubleToDiceEdge(rand, n);
            return roll;
        }

        private double GetNextDouble()
        {
            var next = _random.NextDouble();
            return next;
        }

        private double GetNext(double min, double max)
        {
            double x;
            do
            {
                x = NextGaussian(MEAN, STDDEV);
            } while (x < min || x > max);
            return x;
        }

        private double NextGaussian(double mean, double stdDev)
        {
            var u1 = GetNextDouble(); //these are uniform(0,1) random doubles
            var u2 = GetNextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            var randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }
    }
}
