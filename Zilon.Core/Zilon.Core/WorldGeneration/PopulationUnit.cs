namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Единица населения города или группы мигрантов.
    /// Условно одна единица популяции - это 100 человек.
    /// </summary>
    public sealed class PopulationUnit
    {
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
        /// Текущий счётчик роста.
        /// Это прогресс. Когда достигает 1, тогда единица населения готова для роста.
        /// </summary>
        public float PopulationGrowthCounter { get; private set; }

        /// <summary>
        /// Скорость роста населения для данной единицы.
        /// </summary>
        public float PopulationGrowthRate { get; private set; }

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
    }
}
