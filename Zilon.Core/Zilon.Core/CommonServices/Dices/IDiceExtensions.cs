using System;

namespace Zilon.Core.CommonServices.Dices
{
    public static class DiceExtensions
    {
        /// <summary>
        /// Получение случайного числа в указаном диапазоне [min, max].
        /// </summary>
        /// <param name="dice"> Используемая для броска кость. </param>
        /// <param name="min"> Минимальное значение. </param>
        /// <param name="max"> Максимальное значение. </param>
        /// <returns> Возвращает случайное число в указанном диапазоне [min, max]. </returns>
        public static int Roll(this IDice dice, int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException($"Максимальное значение {max} не может быть меньше минимального {min}.", nameof(max));
            }

            if (min == max)
            {
                return min;
            }

            var range = max - min;
            var roll = dice.Roll(range + 1);

            return roll - 1 + min;
        }
    }
}
