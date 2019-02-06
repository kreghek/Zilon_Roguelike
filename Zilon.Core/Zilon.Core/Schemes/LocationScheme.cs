using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <inheritdoc cref="ILocationScheme" />
    /// <summary>
    /// Схема узла на глобальной карте.
    /// </summary>
    public sealed class LocationScheme : SchemeBase, ILocationScheme
    {
        /// <summary>
        /// Характеристики секторов по уровням.
        /// Если null, то в данной локации нет сектора.
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<SectorSubScheme[]>))]
        [JsonProperty]
        public ISectorSubScheme[] SectorLevels { get; private set; }
    }
}
