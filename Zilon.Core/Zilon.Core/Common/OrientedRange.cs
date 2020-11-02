using System;

namespace Zilon.Core.Common
{
    /// <summary>
    /// Расширенный диапазон значений с указанием направления.
    /// </summary>
    /// <typeparam name="T"> Тип значений. </typeparam>
    public sealed class OrientedRange<T> : Range<T> where T : IComparable
    {
        /// <summary>
        /// Направление диапазона.
        /// </summary>
        public bool IsAcs { get; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="min"> Минимальное значение диапазона. </param>
        /// <param name="max"> Максимальное значение диапазона. </param>
        /// <param name="isAcs"> Указывает направление диапазона. </param>
        public OrientedRange(T min, T max, bool isAcs) : base(min, max)
        {
            IsAcs = isAcs;
        }
    }
}