using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Запись в схеме таблицы дропа.
    /// </summary>
    /// <remarks>
    /// Содержит информацию о том, какой продемет может выпасть,
    /// количество/качество и с какой вероятностью.
    /// </remarks>
    public sealed class DropTableRecordSubScheme : SubSchemeBase, IDropTableRecordSubScheme
    {
        [ExcludeFromCodeCoverage]
        [JsonConstructor]
        private DropTableRecordSubScheme()
        {
        }

        [ExcludeFromCodeCoverage]
        public DropTableRecordSubScheme(string schemeSid, int weight)
        {
            //TODO Этот конструктор должен быть в тестовой реализации записи дропа.
            // После удаления конструктора убрать и безпараметровый и проверить загрузку схем
            SchemeSid = schemeSid ?? throw new System.ArgumentNullException(nameof(schemeSid));

            if (weight <= 0)
            {
                throw new System.ArgumentNullException(nameof(weight),
                    "Вес записи в таблице дропа должен быть положительным.");
            }

            Weight = weight;
        }

        /// <summary>
        /// Схема предмета.
        /// </summary>
        [JsonProperty]
        public string SchemeSid { get; private set; }

        /// <summary>
        /// Вес записи в таблице дропа.
        /// </summary>
        /// <remarks>
        /// Чем выше, тем веротянее будет выбрана данная запись при разрешении дропа.
        /// </remarks>
        [JsonProperty]
        public int Weight { get; private set; }

        /// <summary>
        /// Минимальное количество ресурса.
        /// </summary>
        [JsonProperty]
        public int MinCount { get; private set; }

        /// <summary>
        /// Максимальное количество ресурса.
        /// </summary>
        [JsonProperty]
        public int MaxCount { get; private set; }

        /// <summary>
        /// Дополнительный дроп.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<DropTableScheme[]>))]
        public IDropTableScheme[] Extra { get; private set; }
    }
}