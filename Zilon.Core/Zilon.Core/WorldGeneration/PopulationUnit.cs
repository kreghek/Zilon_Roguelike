using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Единица населения города или группы мигрантов.
    /// Условно одна единица популяции - это 10 человек.
    /// </summary>
    public sealed class PopulationUnit
    {
        private const float MIN_AGE_POWER = 0.3f;
        private const float POWER_COEF = 1f;
        private const float MAX_AGE_POWER = 1.3f;
        private const float OLDMAN_AGE_POWER = 0.5f;
        private const int PROF_AGE = 40;
        private const int NOOBIE_AGE = 15;
        private const int OLDMAN_AGE = 60;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public PopulationUnit()
        {
            // Значение сорости роста рассчитано так:
            // Одна итерация - это неделя.
            // В году 52 недели (365/7).
            // Средний возраст, когда население этого мира становится работоспособным - 15 лет.
            // Получаем примерно 782 = 52*15.
            // Средний работоспособный возраст или скорость роста у разных народов будет разниться.
            // Так же на скорость роста могут влиять другие факторы (благосостояние города, кризисы).
            // Но это учесть позже.

            PopulationGrowthRate = 1f / 782;
        }

        /// <summary>
        /// Специализация населения.
        /// </summary>
        public PopulationSpecializations Specialization { get; set; }

        /// <summary>
        /// Раса единицы населения.
        /// </summary>
        public PopulationRace Race { get; set; }

        /// <summary>
        /// Текущий счётчик роста.
        /// Это прогресс. Когда достигает 1, тогда единица населения готова для роста.
        /// </summary>
        public float PopulationGrowthCounter { get; private set; }

        /// <summary>
        /// Скорость роста населения для данной единицы.
        /// </summary>
        public float PopulationGrowthRate { get; private set; }

        /// <summary>
        /// Здоровье единицы населения.
        /// Показывает, насколько жизнеспособна единица населения.
        /// 1f - нормальное здоровье.
        /// Голод, мор, плохая экология, штурм города и т.д. снижает здоровье. Здоровье не восстанавливается со временем.
        /// </summary>
        public float Health { get; set; }

        /// <summary>
        /// Эффективность единицы населения. 
        /// </summary>
        /// <remarks>
        /// Чем выше эффективность, тем больше
        /// продукции производит здание, на которое назначено население.
        /// Если единица населения назначена более чем на одну структуру, то
        /// эффективность делится на несколько зданий.
        /// Базовая эффективность равна 1f. Когда единица населения только сформирована,
        /// эффективность низкая. Предполагается, то единица населения ещё не квалифицирована.
        /// Со временем эффективность вырастает до пиковой, выше базовой. Предполагается, то эта часть населения
        /// обладает наибольшим опытом и достигает высот профессионализма.
        /// Далее, с возрастом, эффективность снижается. Наступает старость.
        /// 
        /// Так же эффективность зависит от здоровья. Чем ниже здоровье, тем ниже эффетивность.
        /// </remarks>
        public float Power => GetAgePower(Age) * Health * POWER_COEF;

        /// <summary>
        /// Средний возраст единицы населения.
        /// </summary>
        /// <remarks>
        /// Возраст влияет на:
        /// - Эффективность работы населения. Минимальный возрасть 15. Пиковый возраст 40. Максимальный возраст 60.
        /// - Возможность репродукции. Репродукция начинается с 20.
        /// - Проверка на урон от старости. Урон от старости начинается после 60.
        /// </remarks>
        public float Age { get; set; }

        /// <summary>
        /// Обновление роста населения.
        /// </summary>
        public void UpdateGrowth()
        {
            PopulationGrowthCounter += PopulationGrowthRate;
        }

        /// <summary>
        /// Сброс счётчика роста населения.
        /// </summary>
        public void DropGrowthCounter()
        {
            PopulationGrowthCounter = 1 - PopulationGrowthCounter;
        }

        /// <summary>
        /// Назначение населения по структурам.
        /// </summary>
        /// <remarks>
        /// Это перечень структур, на которых задействована текущая единица населения.
        /// </remarks>
        public List<ILocalityStructure> Assigments { get; }

        private float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        private float GetAgePower(float age)
        {
            if (age < 40)
            {
                return Lerp(MIN_AGE_POWER, MAX_AGE_POWER, age / (PROF_AGE - NOOBIE_AGE));
            }
            else if (age < 60)
            {
                return Lerp(MAX_AGE_POWER, OLDMAN_AGE_POWER, age / (OLDMAN_AGE - PROF_AGE));
            }
            else
            {
                return OLDMAN_AGE_POWER;
            }
        }
    }
}
