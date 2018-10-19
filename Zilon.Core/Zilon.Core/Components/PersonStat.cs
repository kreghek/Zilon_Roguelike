using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Components
{
    /// <summary>
    /// Структура для хранения характеристики.
    /// </summary>
    public sealed class PersonStat
    {
        public PersonStat(float baseValue): this(baseValue, 0)
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
        /// Показатель увеличения характеристки в зависимости от уровеня.
        /// </summary>
        public float LevelInc { get; }

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
