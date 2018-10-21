using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zilon.Core.Components
{
    /// <summary>
    /// Тип правила персонажа.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    [PublicAPI]
    public enum PersonRuleType
    {
        Undefined,
        Melee,
        Ballistic
    }
}
