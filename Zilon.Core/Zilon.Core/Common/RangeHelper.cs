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
        /// <param name="values"> Два значения диапазона. Без учёта порядка. Если указано не 2 числа, то будет выбрашено исключение. </param>
        /// <returns> Создаст диапазон, где минимальное из указанных значений будет минимальным в диапазоне. </returns>
        public static Range<T> CreateNormalized<T>(params T[] values) where T : IComparable
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 2)
            {
                throw new ArgumentException("Нужно указывать строго 2 значения.", nameof(values));
            }

            T min;
            T max;
            if (values[0].CompareTo(values[1]) <= -1)
            {
                min = values[0];
                max = values[1];
            }
            else
            {
                min = values[1];
                max = values[0];
            }

            return new Range<T>(min, max);
        }
    }
}
