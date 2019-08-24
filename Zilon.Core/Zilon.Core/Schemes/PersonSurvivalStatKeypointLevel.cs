using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Уровень ключевой точки в характеристиках выживания.
    /// Обычно, тем выше уровень, тем хуже персонажу, больше штрафы.
    /// Служит только для загрузки схемы из json.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PersonSurvivalStatKeypointLevel
    {
        /// <summary>
        /// Не определено. Скорее всего ошибка при десериализации или при создании.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Слабая нужда в чём-либо.
        /// </summary>
        Lesser,

        /// <summary>
        /// Сильная нужда в чём-либо.
        /// </summary>
        Strong,

        /// <summary>
        /// Критический уровнь нужны в чём-либо.
        /// </summary>
        Max
    }
}
