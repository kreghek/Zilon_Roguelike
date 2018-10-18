using System;

namespace Zilon.Core.Common
{
    /// <summary>
    ///  Вспомогательный класс для диапазонов.
    /// </summary>
    public static class RangeHelper
    {
        /// <summary>
        /// Создаёт диапазон без учёта порядка указания минимального и максимального значения.
        /// </summary>
        /// <typeparam name="T"> Тип значений диапазона. </typeparam>
        /// <param name="a"> Крайнее значение диапазона. </param>
        /// <param name="b"> Крайнее значение диапазона. </param>
        /// <returns> Создаст диапазон, где минимальное из указанных значений будет минимальным в диапазоне. </returns>
        public static OrientedRange<T> CreateNormalized<T>(T a, T b) where T : IComparable
        {
            T min;
            T max;
            bool isAsc;
            if (a.CompareTo(b) <= -1)
            {
                min = a;
                max = b;
                isAsc = true;
            }
            else
            {
                min = b;
                max = a;
                isAsc = false;
            }

            return new OrientedRange<T>(min, max, isAsc);
        }
    }
}
