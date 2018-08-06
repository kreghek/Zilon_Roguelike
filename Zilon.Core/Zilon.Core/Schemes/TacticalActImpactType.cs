using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Тип воздействия.
    /// </summary>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum TacticalActImpactType
    {
        /// <summary>
        /// Не опеределено
        /// </summary>
        Undefined,

        /// <summary>
        /// Кинетическое.
        /// </summary>
        Kinetic,

        /// <summary>
        /// Взрывное.
        /// </summary>
        Explosion,

        /// <summary>
        /// Псионическое.
        /// </summary>
        Psy,

        /// <summary>
        /// Кислотное.
        /// </summary>
        Acid,

        /// <summary>
        /// Термальное.
        /// </summary>
        Termal
    }
}
