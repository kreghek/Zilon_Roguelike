using Newtonsoft.Json;

namespace Zilon.Core.Components
{
    /// <summary>
    /// Цели действия.
    /// </summary>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum TacticalActTargets
    {
        /// <summary>
        /// Персонаж-противник.
        /// </summary>
        Enemy = 1 << 0,

        /// <summary>
        /// Персонаж-союзник.
        /// </summary>
        Ally = 1 << 1,

        /// <summary>
        /// Персонаж из нейтральной команды.
        /// </summary>
        Neutral = 1 << 2,

        /// <summary>
        /// Возможно действовать на себя.
        /// </summary>
        Self = 1 << 3,

        /// <summary>
        /// Действие на узел карты.
        /// </summary>
        Terrain = 1 << 4,

        /// <summary>
        /// Возможно действовать на предметы окружения. Например, сундуки и двери.
        /// </summary>
        Env = 1 << 5,

        /// <summary>
        /// Любой персонаж.
        /// </summary>
        Anybody = Enemy | Ally | Neutral | Self,

        /// <summary>
        /// Неопределено.
        /// </summary>
        /// <remarks>
        /// Обычно это ошибка десериализации схемы.
        /// </remarks>
        Undefined = 0
    }
}
