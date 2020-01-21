using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема тарблицы дропа.
    /// </summary>
    public sealed class DropTableScheme : SchemeBase, IDropTableScheme
    {
        /// <summary>
        /// Записи в таблице дропа.
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<DropTableRecordSubScheme[]>))]
        [JsonProperty]
        public IDropTableRecordSubScheme[] Records { get; private set; }

        /// <summary>
        /// Количество бросков на проверку выпавшей записи.
        /// </summary>
        [JsonProperty]
        public int Rolls { get; private set; }
    }
}
