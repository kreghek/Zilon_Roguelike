using Zilon.Core.Common;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Текущие показатели характеристики модуля выживания.
    /// </summary>
    public sealed class SurvivalStat: Stat
    {
        /// <summary>
        /// Конструирует объект статы выживания.
        /// </summary>
        /// <param name="startValue"> Начальное значение. Должно быть в диапазоне [min, max]. </param>
        /// <param name="min"> Минимальное значение статы. </param>
        /// <param name="max"> Минимальное значение статы. </param>
        public SurvivalStat(int startValue, int min, int max): base(startValue, min, max)
        {

        }

        /// <summary>
        /// Тип характеристики.
        /// </summary>
        public SurvivalStatType Type { get; set; }


        /// <summary>
        /// Скорость снижения характеристики за ход.
        /// </summary>
        public int Rate { get; set; }

        /// <summary> Набор ключевых точек характеристики. </summary>
        public SurvivalStatKeyPoint[] KeyPoints { get; set; }
    }
}
