namespace Zilon.Core.Diseases
{
    /// <summary>
    ///     Правила влияния симптомов.
    /// </summary>
    public enum DiseaseSymptomType
    {
        Undefined,

        /// <summary>
        ///     Снижает DownPassRoll для дыхания.
        ///     То есть симптом со временем делает так, что перестаёт хватать воздуха - одышка.
        /// </summary>
        BreathDownSpeed,

        /// <summary>
        ///     Снижает DownPassRoll для восстановления энергии.
        ///     То есть персонаж будет быстрее уставать и медленнее востанавливаться. Утомляемость.
        /// </summary>
        EnegryDownSpeed,

        /// <summary>
        ///     Увеличивает скорость роста голода.
        /// </summary>
        HungerSpeed,

        /// <summary>
        ///     Увеличивает скорость роста жажды.
        ///     Провоцирует постоянную жажду.
        /// </summary>
        ThirstSpeed,

        /// <summary>
        ///     Снижает лимит здоровья персонажа.
        /// </summary>
        HealthLimit
    }
}