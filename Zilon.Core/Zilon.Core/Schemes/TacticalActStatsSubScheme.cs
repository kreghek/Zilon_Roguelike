using Zilon.Core.Common;

namespace Zilon.Core.Schemes
{
    public class TacticalActStatsSubScheme: SubSchemeBase
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
        /// Дистанция, в котором возможно использования действия.
        /// </summary>
        public Range<int> Range { get; set; }

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
    }
}
