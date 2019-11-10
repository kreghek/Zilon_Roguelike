using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    /// Игральная кость, работающая по экпонециальному закону.
    /// </summary>
    public sealed class InversedExpDice : IDice
    {
        private const double LAMBDA = 0.5;
        private const double MAX = 7;

        private readonly Random _random;

        /// <summary>
        /// Конструктор генератора.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public InversedExpDice()
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
        public InversedExpDice(int seed)
        {
            _random = new Random(seed);
        }

        public int Roll(int n)
        {
            var rollUnit = GetBounded(0, 1);

            var roll = DiceValuesHelper.MapDoubleToDiceEdge(rollUnit, n);

            var inversedRoll = n - roll + 1;
            return inversedRoll;
        }

        private double GetNext()
        {
            var u = GetNextDouble();

            var x = ExponentialAlgorithms.MapToExpo(u, LAMBDA);

            var mappedX = MapToInterval(
                x,
                sourceMin: 0,
                sourceMax: MAX,
                targetMin: 0,
                targetMax: 1);

            return mappedX;
        }

        private double GetBounded(double min, double max)
        {
            double x;
            do
            {
                x = GetNext();
            } while (x < min || x > max);
            return x;
        }

        private double GetNextDouble()
        {
            var next = _random.NextDouble();
            return next;
        }

        private static double MapToInterval(double x, double sourceMin, double sourceMax, double targetMin, double targetMax)
        {
            var targetDiff = targetMax - targetMin;
            var sourceDiff = sourceMax - sourceMin;
            var shifftedX = x - sourceMin;
            var sourceRatioX = shifftedX / sourceDiff;
            var xNormalized = targetDiff * sourceRatioX;

            return xNormalized + sourceMin;
        }
    }
}
