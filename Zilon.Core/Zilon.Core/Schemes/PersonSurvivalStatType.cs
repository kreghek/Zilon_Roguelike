using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Перечисление характеристик выживания для персонажа.
    /// Служит только для загрузки схемы из json.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PersonSurvivalStatType
    {
        /// <summary>
        /// Не определена. Скорее всего, ошибка.
        /// </summary>
        Undefined,

        /// <summary>
        /// Сытость.
        /// </summary>
        Satiety,

        /// <summary>
        /// Достаточность воды.
        /// </summary>
        Hydration
    }
}
