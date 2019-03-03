using System;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Zilon.Core.Common;
using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    public class TacticalActStatsSubScheme : SubSchemeBase, ITacticalActStatsSubScheme
    {
        /// <summary>
        /// Характеристики атакующей способности действия.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<TacticalActOffenceSubScheme>))]
        public ITacticalActOffenceSubScheme Offence { get; private set; }

        /// <summary>
        /// Эффект, который оказывает действие.
        /// </summary>
        [JsonProperty]
        public TacticalActEffectType Effect { get; private set; }

        /// <summary>
        /// Эффективность действия.
        /// </summary>
        [JsonProperty]
        public Roll Efficient { get; private set; }

        /// <summary>
        /// Дистанция, в котором возможно использования действия.
        /// </summary>
        [JsonProperty]
        public Range<int> Range { get; private set; }

        /// <summary>
        /// Количество ударов при совершении действия.
        /// </summary>
        [JsonProperty]
        public int HitCount { get; private set; }

        /// <summary>
        /// Является ли действие рукопашным.
        /// </summary>
        /// <remarks>
        /// Рукопашные действия переводят актёра в режим рукопашного боя.
        /// Во время рукопашного режима можно использовать только рукопашные действия.
        /// </remarks>
        [JsonProperty]
        public bool IsMelee { get; private set; }

        /// <summary>Доступные цели действия.</summary>
        [JsonProperty]
        public TacticalActTargets Targets { get; private set; }
    }
}
