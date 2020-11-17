using System;

namespace Zilon.Core.Common
{
    /// <summary>
    /// Класс для представления диапазона значений.
    /// </summary>
    /// <typeparam name="T"> Тип значений. </typeparam>
    public class Range<T> where T : IComparable
    {
        /// <summary>
        /// Минимальное значение диапазона.
        /// </summary>
        public T Min { get; }

        /// <summary>
        /// Максимальное значение диапазона.
        /// </summary>
        public T Max { get; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="min"> Минимальное значение диапазона. </param>
        /// <param name="max"> Максимальное значение диапазона. </param>
        public Range(T min, T max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Проверяет вхождение указанного значения в диапазон.
        /// </summary>
        /// <param name="value"> Проверяемое значение. </param>
        /// <returns>
        /// Возвращает true, если текущее значение входит в диапазон,
        /// включая крайние значения. Иначе - false.
        /// </returns>
        public bool Contains(T value)
        {
            var isMoreThat = value.CompareTo(Min) >= 0;
            var isLessThat = value.CompareTo(Max) <= 0;
            return isMoreThat && isLessThat;
        }

        public override string ToString()
        {
            return $"{Min} - {Max}";
        }
    }
}