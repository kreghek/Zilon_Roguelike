namespace Zilon.Core.Persons
{
    /// <summary>
    /// Данные персонажа по выживанию.
    /// </summary>
    /// <remarks>
    /// Здесь будут сведения о питании, отдыхе, ранах, эмоциональном состоянии персонажа.
    /// </remarks>
    public interface ISurvivalData
    {
        /// <summary>
        /// Текущий запас сытости.
        /// </summary>
        int Satiety { get; }

        /// <summary>
        /// Текущий запас воды.
        /// </summary>
        int Thirst { get; }

        /// <summary>
        /// Обновление состояния данных о выживании.
        /// </summary>
        void Update();

        /// <summary>
        /// Пополнение запаса сытости.
        /// </summary>
        /// <param name="value"> Значение, на которое увеличивается текущий запас. </param>
        void ReplenishSatiety(int value);

        /// <summary>
        /// Пополнение запаса воды.
        /// </summary>
        /// <param name="value"> Значение, на которое увеличивается текущий запас. </param>
        void ReplenishThirst(int value);
    }
}
