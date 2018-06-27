namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Тип действия.
    /// </summary>
    public enum TacticalActEffectType
    {
        /// <summary>
        /// Не определено.
        /// </summary>
        Undefined,

        /// <summary>
        /// Наносит урон.
        /// </summary>
        Damage = 1 << 0,

        /// <summary>
        /// Восстановление.
        /// </summary>
        Heal = 1 << 1
    }
}
