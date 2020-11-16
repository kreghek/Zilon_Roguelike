using System;

namespace Zilon.Core.CommonServices.Dices
{
    internal static class DiceValuesHelper
    {
        /// <summary>
        /// Возвращает случайное число в диапазоне [0..1).
        /// </summary>
        public static double GetNextDouble(Random random)
        {
            var next = random.NextDouble();
            return next;
        }

        /// <summary>
        /// Преобразует выброшенное дробное значение из диапазона [0..1]
        /// в грань n-гранной кости.
        /// </summary>
        /// <param name="rawValue"> Сырое значение в диапазоне [0..1]. </param>
        /// <param name="n"> Количество граней целевой кости. </param>
        /// <returns> Возвращает выбрашенную грань. </returns>
        public static int MapDoubleToDiceEdge(double rawValue, int n)
        {
            return (int)(n * rawValue) + 1;
        }
    }
}