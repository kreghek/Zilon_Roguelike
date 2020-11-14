namespace Zilon.Core.Components
{
    /// <summary>
    /// Структура для хранения характеристики.
    /// </summary>
    public sealed class PersonStat
    {
        public PersonStat(float baseValue) : this(baseValue, 0)
        {
        }

        public PersonStat(float baseValue, float levelInc)
        {
            Base = baseValue;
            LevelInc = levelInc;
        }

        /// <summary>
        /// Базовое значение.
        /// </summary>
        public float Base { get; }

        /// <summary>
        /// Показатель увеличения характеристики в зависимости от уровня.
        /// </summary>
        public float LevelInc { get; }

        /// <summary>
        /// Расчёт текущего значения характеристики.
        /// </summary>
        /// <param name="level"> Актуальный уровень. </param>
        /// <param name="rarityBonus"> Бонус за редкость персонажа. </param>
        /// <param name="bonuses"> Прочие бонусы к текущей характеристике. </param>
        /// <returns> Возвращает актуальное значение характеритсики. </returns>
        public float GetActualValue(int level, float rarityBonus, PersonStat[] bonuses)
        {
            var bonusValue = CalcBonusValue(level, bonuses);

            return ((Base + (LevelInc * (level - 1))) * (1 + rarityBonus)) + bonusValue;
        }

        /// <summary>
        /// Расчёт текущего значения характеристики.
        /// </summary>
        /// <param name="level"> Актуальный уровень. </param>
        /// <param name="rarityBonus"> Бонус за редкость персонажа. </param>
        /// <returns> Возвращает актуальное значение характеритсики. </returns>
        public float GetActualValue(int level, float rarityBonus)
        {
            return GetActualValue(level, rarityBonus, null);
        }

        private static float CalcBonusValue(int level, PersonStat[] bonuses)
        {
            if (bonuses == null)
            {
                return 0;
            }

            return bonuses.Sum(bonus => bonus.GetActualValue(level, 0));
        }
    }
}