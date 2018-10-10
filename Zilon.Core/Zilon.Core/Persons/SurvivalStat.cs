using Zilon.Core.Common;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Текущие показатели характеристики модуля выживания.
    /// </summary>
    public class SurvivalStat
    {
        public SurvivalStat(int startValue, int min, int max)
        {
            _value = startValue;
            Range = new Range<int>(min,max);
        }

        /// <summary>
        /// Тип характеристики.
        /// </summary>
        public SurvivalStatType Type { get; set; }

        private int _value;
        /// <summary>
        /// Текущее значение.
        /// </summary>
        public int Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (value >= Range.Max)
                {
                    _value = Range.Max;
                }
                else if (value <= Range.Min)
                {
                    _value = Range.Min;
                }
                else
                {
                    _value = value;
                }
            }
        }

        /// <summary>
        /// Минимальное/максимальное значение.
        /// </summary>
        public Range<int> Range { get; set; }

        /// <summary>
        /// Скорость снижения характеристики за ход.
        /// </summary>
        public int Rate { get; set; }

        public SurvivalStatKeyPoint[] KeyPoints { get; set; }
    }
}
