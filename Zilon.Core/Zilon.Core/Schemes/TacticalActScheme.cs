using Zilon.Core.Common;

namespace Zilon.Core.Schemes
{
    /// <inheritdoc />
    /// <summary>
    /// Схема действия.
    /// </summary>
    public class TacticalActScheme: SchemeBase
    {
        /// <summary>
        /// Тип воздействия.
        /// </summary>
        public TacticalActImpactType Impact { get; set; }

        /// <summary>
        /// Эффект, который оказывает действие.
        /// </summary>
        public TacticalActEffectType Effect { get; set; }

        /// <summary>
        /// Эффективность действия.
        /// </summary>
        public Range<float> Efficient { get; set; }

        /// <summary>
        /// Минимальный радиус, в котором возможно использования действия.
        /// </summary>
        public int MinRange { get; set; }

        /// <summary>
        /// Максимальный радиус, в котором возможно использования действия.
        /// </summary>
        public int MaxRange { get; set; }

        /// <summary>
        /// Количество ударов при совершении действия.
        /// </summary>
        public int HitCount { get; set; }

        /// <summary>
        /// Является ли действие рукопашным.
        /// </summary>
        /// <remarks>
        /// Рукопашные действия переводят актёра в режим рукопашного боя.
        /// Во время рукопашного режима можно использовать только рукопашные действия.
        /// </remarks>
        public bool IsMelee { get; set; }

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

        public TacticalActDependencySubScheme[] Dependency { get; set; }
    }
}
