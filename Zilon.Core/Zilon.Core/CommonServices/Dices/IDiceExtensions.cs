using System;

namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    /// Вспомогательные расширения сервиса для работы с игральной костью.
    /// </summary>
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

            if (dice == null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            if (min == max)
            {
                return min;
            }

            var range = max - min;
            var roll = dice.Roll(range + 1);

            return roll - 1 + min;
        }

        public static int RollD6(this IDice dice)
        {
            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            return dice.Roll(6);
        }

        public static int Roll2D6(this IDice dice)
        {
            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            return dice.Roll(6) + dice.Roll(6);
        }

        public static int RollD3(this IDice dice)
        {
            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            return dice.Roll(6);
        }
    }
}
