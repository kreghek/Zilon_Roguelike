namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема для хранения ограничений на использование действия.
    /// </summary>
    /// <remarks>
    /// Используется только актёрами под прямым управлением игрока.
    /// </remarks>
    public class TacticalActConstrainsSubScheme : SubSchemeBase, ITacticalActConstrainsSubScheme
    {
        /// <summary>
        /// Количество использований.
        /// </summary>
        public int? UsageResource { get; set; }

        /// <summary>
        /// Восстановление количества использований.
        /// </summary>
        public int? UsageResourceRegen { get; set; }

        /// <summary>
        /// Куллдаун между использованиями.
        /// </summary>
        public int? Cooldown { get; set; }
    }
}
