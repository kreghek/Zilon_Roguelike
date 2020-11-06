using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Zilon.Core.Common
{
    /// <summary>
    /// Тип воздействия.
    /// </summary>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    [PublicAPI]
    public enum ImpactType
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