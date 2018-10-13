using System;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема действия.
    /// </summary>
    public class TacticalActScheme : SchemeBase, ITacticalActScheme
    {
        [JsonConstructor]
        public TacticalActScheme(TacticalActStatsSubScheme stats, TacticalActConstrainsSubScheme constrains)
        {
            Stats = stats ?? throw new ArgumentNullException(nameof(stats));
            Constrains = constrains;
        }

        /// <summary>
        /// Основные характеристики действия.
        /// </summary>
        public ITacticalActStatsSubScheme Stats { get; }

        /// <summary>
        /// Ограничения на использование действия.
        /// </summary>
        public ITacticalActConstrainsSubScheme Constrains { get; }
    }
}
