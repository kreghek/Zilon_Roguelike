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

            // -1 из-за недостатка Dice.
            // Он добавляет +1.
            var maxDiceVal = int.MaxValue - 1;
            for (int n = 0; n < count; n++)
            {
                var dSumm = 0.0;
                for (int i = 0; i < 12; i++)
                {
                    var r = (double)_dice.Roll(maxDiceVal) / maxDiceVal;
                    dSumm += r;
                }
                var a = SIGMA * (dSumm - 6.0f);
                var randomRaw = MV + a;
                var randomAbs = Math.Abs(randomRaw);

                // Нормализауем зерультат, чтобы он был в диапазоне [0..1]
                sequence[n] = randomRaw;
            }

            return sequence;
        }
    }
}
