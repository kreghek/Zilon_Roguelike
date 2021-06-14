using System;

namespace Zilon.Core.Common
{
    /// <summary>
    /// Вспомогательный класс для работы с отрезками.
    /// </summary>
    public static class SegmentHelper
    {
        /// <summary>
        /// Проверяет пересечение двух 1D отрезков.
        /// </summary>
        /// <param name="x1">Начальная точка отрезка 1.</param>
        /// <param name="x2">Конечная точка отрезка 1.</param>
        /// <param name="y1">Начальная точка отрезка 2.</param>
        /// <param name="y2">Конечная точка отрезка 2.</param>
        /// <returns>Возвращает true, если отрезки пересекаются. Иначе, позращает false.</returns>
        public static bool IsIntersects(float x1, float x2, float y1, float y2)
        {
            if (x1 > x2)
            {
                throw new ArgumentException("Начальная точка отрезка должна быть меньше или равна конечной точке.",
                    nameof(x1));
            }

            if (y1 > y2)
            {
                throw new ArgumentException("Начальная точка отрезка должна быть меньше или равна конечной точке.",
                    nameof(y1));
            }

            return x2 >= y1 && y2 >= x1;
        }
    }
}