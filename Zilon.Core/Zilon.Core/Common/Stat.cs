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
        private float _valueShare;

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
        /// Минимальное/максимальное значение.
        /// </summary>
        public Range<int> Range { get; private set; }

        /// <summary>
        /// Текущее значение.
        /// </summary>
        public int Value
        {
            get
            {
                if (ValueShare >= 1)
                {
                    return Range.Max;
                }

                if (ValueShare <= 0)
                {
                    return Range.Min;
                }

                var result = Math.Round(((Range.Max - Range.Min) * ValueShare) + Range.Min);
                return (int)result;
            }
            set
            {
                if (Range.Max == Range.Min)
                {
                    ValueShare = 1;
                    return;
                }

                var boundedValue = Range.GetBounded(value);
                ValueShare = (boundedValue - Range.Min) / (float)(Range.Max - Range.Min);
            }
        }

        /// <summary>
        /// Значение в долях. Значение [0..1] в текущем диапазоне.
        /// </summary>
        public float ValueShare
        {
            get => _valueShare;
            private set
            {
                var wasChanged = value != _valueShare;
                _valueShare = value;
                if (wasChanged)
                {
                    Changed?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Изменение текущего диапазона характеристики.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public virtual void ChangeStatRange(int min, int max)
        {
            if (min >= max)
            {
                Range = new Range<int>(min, min);
                Value = Range.Min;
                return;
            }

            Range = new Range<int>(min, max);
        }

        /// <summary>
        /// Устанавливает текущее значение в долях.
        /// </summary>
        /// <param name="value"> Значение в диапазоне [0..1]. </param>
        public void SetShare(float value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Значение не может быть меньше 0.", nameof(value));
            }

            if (value > 1)
            {
                throw new ArgumentException("Значение не может быть больше 1.", nameof(value));
            }

            ValueShare = value;
        }

        /// <summary>
        /// Выстреливает каждый раз, когда значение характеристики изменяется.
        /// </summary>
        public event EventHandler Changed;
    }
}