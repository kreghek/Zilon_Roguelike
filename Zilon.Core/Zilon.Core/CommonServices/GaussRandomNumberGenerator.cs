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
        private readonly double _sigma = 0.5;
        private readonly double _mu = 0.125;
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

            // -1 из-за недостатка Dice.
            // Он добавляет +1.
            var maxDiceVal = int.MaxValue - 1;
            for (int n = 0; n < count; n++)
            {
                double dSumm = 0;
                for (int i = 0; i <= 12; i++)
                {
                    double R = (float)_dice.Roll(maxDiceVal) / (maxDiceVal);
                    dSumm += R;
                }
                double dRandValue = Math.Round((_mu + _sigma * (dSumm - 6)), 3);
                sequence[n] = Math.Min(Math.Abs(dRandValue), 1);
            }

            return sequence;
        }
    }
}
