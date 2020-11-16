using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Структура для хранения работы.
    /// </summary>
    /// <remarks>
    /// Используется для хранения требуемых работ перка или квеста.
    /// </remarks>
    public sealed class JobSubScheme : IJobSubScheme
    {
        /// <summary>
        /// Тип работы.
        /// </summary>
        [JsonProperty]
        public JobType Type { get; private set; }

        /// <summary>
        /// Объём работы.
        /// </summary>
        [JsonProperty]
        public int Value { get; private set; }

        /// <summary>
        /// Область действия работы.
        /// </summary>
        [JsonProperty]
        public JobScope Scope { get; private set; }

        /// <summary>
        /// Дополнительные данные по работе.
        /// </summary>
        [JsonProperty]
        public string[] Data { get; private set; }
    }
}