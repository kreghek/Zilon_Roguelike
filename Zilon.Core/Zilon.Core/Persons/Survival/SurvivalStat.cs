using Zilon.Core.Common;

namespace Zilon.Core.Persons.Survival
{
    /// <summary>
    /// Текущие показатели характеристики модуля выживания.
    /// </summary>
    public class SurvivalStat : Stat
    {
        /// <summary>
        /// Значение броска по умолчанию для снижения характеристики.
        /// Нужно выбросить столько или больше, чтобы характеристика не снижалась.
        /// </summary>
        public const int DEFAULT_DOWN_PASS_VALUE = 4;

        /// <summary>
        /// Конструирует объект статы выживания.
        /// </summary>
        /// <param name="startValue"> Начальное значение. Должно быть в диапазоне [min, max]. </param>
        /// <param name="min"> Минимальное значение статы. </param>
        /// <param name="max"> Минимальное значение статы. </param>
        public SurvivalStat(int startValue, int min, int max) : base(startValue, min, max)
        {
            DownPassRoll = DEFAULT_DOWN_PASS_VALUE;
        }

        /// <summary>
        /// Бросок кости, при проходит тест на снижение характеристики.
        /// То есть характеристики не снижается, если выброшено больше.
        /// При 0 - не снижается никогда.
        /// При 6 - снижается каждый ход.
        /// </summary>
        public int DownPassRoll { get; set; }

        /// <summary>
        /// Ключевые сегменты характеристики.
        /// </summary>
        public SurvivalStatKeySegment[] KeySegments { get; set; }


        /// <summary>
        /// Скорость снижения характеристики за ход.
        /// </summary>
        public int Rate { get; set; }

        /// <summary>
        /// Тип характеристики.
        /// </summary>
        public SurvivalStatType Type { get; set; }
    }
}