using System;

using JetBrains.Annotations;

namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    /// Игральная кость, работающая по экпонециальному закону.
    /// </summary>
    public sealed class ExpDice : IDice
    {
        private const double LAMBDA = 0.5;
        private const double MAX = 7;

        private readonly IDice _dice;


        /// <summary>
        /// Конструктор генератора.
        /// </summary>
        /// <param name="dice"> Игральная кость для выбора случайного зерна генератора. </param>
        public ExpDice([NotNull] IDice dice)
        {
            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            _dice = dice;
        }

        public int Roll(int n)
        {
            var rollUnit = GetBounded(_dice, 0, 1);

            var roll = DiceValuesHelper.MapDoubleToDiceEdge(rollUnit, n);

            return roll;
        }

        private static double GetNext(IDice dice)
        {
            var u = GetNextDouble(dice);

            var x = Math.Log(1 - u) / -LAMBDA;

            var mappedX = MapToInterval(
                x,
                sourceMin: 0,
                sourceMax: MAX,
                targetMin: 0,
                targetMax: 1);

            return mappedX;
        }

        private static double GetBounded(IDice dice, double min, double max)
        {
            double x;
            do
            {
                x = GetNext(dice);
            } while (x < min || x > max);
            return x;
        }

        private static double GetNextDouble(IDice dice)
        {
            var maxDiceVal = 1000_000;
            var next = (double)dice.Roll(maxDiceVal) / maxDiceVal;
            return next;
        }

        private static double MapToInterval(double x, double sourceMin, double sourceMax, double targetMin, double targetMax)
        {
            return (targetMax - targetMin) * (x - sourceMin) / (sourceMax - sourceMin) + sourceMin;
        }
    }
}
