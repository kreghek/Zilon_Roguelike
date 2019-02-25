using System;
using Zilon.Core.Common;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Текущие показатели характеристики модуля выживания.
    /// </summary>
    public class SurvivalStat
    {
        private int _value;

        public SurvivalStat(int startValue, int min, int max)
        {
            _value = startValue;
            Range = new Range<int>(min, max);
        }

        /// <summary>
        /// Тип характеристики.
        /// </summary>
        public SurvivalStatType Type { get; set; }

        /// <summary>
        /// Текущее значение.
        /// </summary>
        public int Value
        {
            get => _value;
            set => _value = Range.GetBounded(value);
        }

        /// <summary>
        /// Минимальное/максимальное значение.
        /// </summary>
        public Range<int> Range { get; private set; }

        /// <summary>
        /// Скорость снижения характеристики за ход.
        /// </summary>
        public int Rate { get; set; }

        public SurvivalStatKeyPoint[] KeyPoints { get; set; }

        /// <summary>
        /// Изменение текущего диапазона характеристики.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public void ChangeStatRange(int min, int max)
        {
            var currentValue = Value;
            var currentMax = Range.Max;
            var newMax = max;
            var newValueRaw = (float) newMax / currentMax * currentValue;
            var newValue = (int)Math.Ceiling(newValueRaw);

            Range = new Range<int>(min, max);
            Value = newValue;
        }
    }
}
