using System;
using System.Collections.Generic;

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
                throw new ArgumentException($"Максимальное значение {max} не может быть меньше минимального {min}.",
                    nameof(max));
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

            return dice.Roll(3);
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

        /// <summary>
        /// Выбирает случайное значение из списка.
        /// </summary>
        /// <typeparam name="T"> Тип элементов списка. </typeparam>
        /// <param name="dice"> Кость, на основе которой делать случайный выбор. </param>
        /// <param name="list"> Список элементов, из которого выбирать элемент. </param>
        /// <param name="count">Количество выбранных значений. </param>
        /// <returns> Случайный элемент из списка. </returns>
        public static IEnumerable<T> RollFromList<T>(this IDice dice, IList<T> list, int count)
        {
            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Count < count)
            {
                throw new ArgumentException("Требуемое количество должно быть не меньше размера списка.",
                    nameof(count));
            }

            var openList = new List<T>(list);

            for (var i = 0; i < count; i++)
            {
                var rolledItem = dice.RollFromList(openList);

                yield return rolledItem;

                openList.Remove(rolledItem);
            }
        }

        /// <summary>
        /// Выбирает случайный индекс из набора.
        /// </summary>
        /// <typeparam name="T"> Тип элементов списка. </typeparam>
        /// <param name="dice"> Кость, на основе которой делать случайный выбор. </param>
        /// <param name="list"> Список элементов, из которого выбирать элемент. </param>
        /// <returns> Случайный элемент из списка. </returns>
        public static int RollArrayIndex<T>(this IDice dice, IList<T> list)
        {
            if (dice is null)
            {
                throw new ArgumentNullException(nameof(dice));
            }

            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            var rollIndex = dice.Roll(0, list.Count);
            return rollIndex;
        }
    }
}