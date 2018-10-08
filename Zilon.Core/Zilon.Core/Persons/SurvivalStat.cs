using Zilon.Core.Common;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Текущие показатели характеристики модуля выживания.
    /// </summary>
    public class SurvivalStat
    {
        public SurvivalStat(int startValue)
        {
            Value = startValue;
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
            get
            {
                return Value;
            }
            set
            {
                if (value >= Range.Max)
                {
                    Value = Range.Max;
                }
                else if (value <= Range.Min)
                {
                    Value = Range.Min;
                }
                else
                {
                    Value = value;
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
