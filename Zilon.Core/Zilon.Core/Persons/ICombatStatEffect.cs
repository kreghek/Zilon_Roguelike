namespace Zilon.Core.Persons
{
    /// <summary>
    /// Эффект, который влияет на модуль сражения.
    /// </summary>
    public interface ICombatStatEffect
    {
        /// <summary>
        /// Применение эффекта при получении.
        /// </summary>
        /// <param name="combatStats"></param>
        void ApplyOnce(ICombatStats combatStats);

        /// <summary>
        /// Применение эффекта каждый ход.
        /// </summary>
        /// <param name="combatStats"></param>
        void ApplyEveryTurn(ICombatStats combatStats);
    }
}
