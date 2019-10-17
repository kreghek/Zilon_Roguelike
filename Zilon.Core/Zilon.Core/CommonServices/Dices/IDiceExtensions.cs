using System;
using System.Collections;
using System.Collections.Generic;

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

        public static int RollD6(this IDice dice)
        {
            return dice.Roll(6);
        }

        public static int Roll2D6(this IDice dice)
        {
            return dice.Roll(6) + dice.Roll(6);
        }

        public static int RollD3(this IDice dice)
        {
            return dice.Roll(6);
        }

        /// <summary>
        /// Выбирает случайное значение из списка.
        /// </summary>
        /// <typeparam name="T"> Тип элементов списка. </typeparam>
        /// <param name="dice"> Кость, на основе которой делать случайный выбор. </param>
        /// <param name="list"> Список элементов, из которого выбирать элемент. </param>
        /// <returns> Случайный элемент из списка. </returns>
        public static T RollFromList<T>(this IDice dice, IList<T> list)
        {
            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            var rollIndex = dice.Roll(0, list.Count - 1);
            var item = list[rollIndex];
            return item;
        }
    }
}
