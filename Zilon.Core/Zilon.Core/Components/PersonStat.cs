using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Components
{
    /// <summary>
    /// Структура для хранения характеристики.
    /// </summary>
    public sealed class PersonStat
    {
        /// <summary>
        /// Базовое значение.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public float Base { get; set; }

        /// <summary>
        /// Показатель увеличения характеристки в зависимости от уровеня.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public float LevelInc { get; set; }

        /// <summary>
        /// Расчёт текущего значения характеристики.
        /// </summary>
        /// <param name="level"> Актуальный уровень. </param>
        /// <param name="rarityBonus"> Бонус за редкость персонажа. </param>
        /// <param name="bonuses"> Прочие бонусы к текущей характеристике. </param>
        /// <returns> Возвращает актуальное значение характеритсики. </returns>
        public float GetActualValue(int level, float rarityBonus, PersonStat[] bonuses = null)
        {
            var bonusValue = 0f;

            if (bonuses != null)
            {
                foreach (var bonus in bonuses)
                {
                    bonusValue += bonus.GetActualValue(level, 0);
                }
            }

            return (Base + LevelInc * (level - 1)) * (1 + rarityBonus) + bonusValue;
        }
    }
}
