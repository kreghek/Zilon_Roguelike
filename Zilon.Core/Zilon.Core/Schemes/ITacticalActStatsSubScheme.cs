using Newtonsoft.Json;

using Zilon.Core.Common;
using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    public interface ITacticalActStatsSubScheme : ISubScheme
    {
        float? Duration { get; }

        /// <summary>
        /// Эффект, который оказывает действие.
        /// </summary>
        TacticalActEffectType Effect { get; }

        /// <summary>
        /// Эффективность действия.
        /// </summary>
        Roll? Efficient { get; }

        /// <summary>
        /// Количество ударов при совершении действия.
        /// </summary>
        int HitCount { get; }

        /// <summary>
        /// Является ли действие рукопашным.
        /// </summary>
        /// <remarks>
        /// Рукопашные действия переводят актёра в режим рукопашного боя.
        /// Во время рукопашного режима можно использовать только рукопашные действия.
        /// </remarks>
        bool IsMelee { get; }

        /// <summary>
        /// Характеристики атакующей способности действия.
        /// </summary>
        ITacticalActOffenceSubScheme? Offence { get; }

        /// <summary>
        /// Дистанция, в котором возможно использования действия.
        /// </summary>
        Range<int>? Range { get; }

        CombatActRule[] Rules { get; }

        /// <summary>
        /// Tags of the act.
        /// Now used to determine video and audio effect.
        /// </summary>
        string?[]? Tags { get; }

        /// <summary>
        /// Доступные цели действия.
        /// </summary>
        TacticalActTargets Targets { get; }
    }

    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum CombatActRule
    {
        Undefined,
        NormalPush,
        MoveBackward
    }
}