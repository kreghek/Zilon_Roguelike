using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Область действия работы.
    /// </summary>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum JobScope
    {
        /// <summary>
        /// Общая область действия.
        /// Прогресс не будет сбрасываться после окончания боя.
        /// </summary>
        Global,

        /// <summary>
        /// Область действия на высадку.
        /// Если работы в рамках одной высадки не выполнены, то прогресс будет сброшен.
        /// </summary>
        Combat
    }
}
