using System;

namespace Zilon.Core.Common
{
    /// <summary>
    /// Общая характеристика.
    /// </summary>
    /// <remarks>
    /// Используется:
    /// - в модуле выживания для хп, голода и жажды.
    /// - как прочность предмета.
    /// </remarks>
    public class Stat
    {
        private float _rawValue;

        /// <summary>
        /// Конструирует объект статы.
        /// </summary>
        /// <param name="startValue"> Начальное значение. Должно быть в диапазоне [min, max]. </param>
        /// <param name="min"> Минимальное значение статы. </param>
        /// <param name="max"> Минимальное значение статы. </param>
        public Stat(int startValue, int min, int max)
        {
            Range = new Range<int>(min, max);

            Value = startValue;
        }

        /// <summary>
        /// Текущее значение.
        /// </summary>
        public int Value
        {
            get
            {
                if (_rawValue == 1)
                {
                    return Range.Max;
                }

                if (_rawValue == 0)
                {
                    return Range.Min;
                }

                var result = Math.Round((Range.Max - Range.Min) * _rawValue + Range.Min);
                return (int)result;
            }
            set
            {
                if (Range.Max == Range.Min)
                {
                    _rawValue = 1;
                    return;
                }

                var boundedValue = Range.GetBounded(value);
                _rawValue = (boundedValue - Range.Min) / (float)(Range.Max - Range.Min);
            }
        }

        /// <summary>
        /// Минимальное/максимальное значение.
        /// </summary>
        public Range<int> Range { get; private set; }

        /// <summary>
        /// Изменение текущего диапазона характеристики.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public void ChangeStatRange(int min, int max)
        {
            if (min >= max)
            {
                Range = new Range<int>(min, min);
                Value = Range.Min;
                return;
            }

            Range = new Range<int>(min, max);
        }
    }
}
