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
        Ballistic,

        /// <summary>
        /// Правило влияет на хитпоинты персонажа.
        /// </summary>
        Health,

        /// <summary>
        /// Правило влияет на урон персонажа действиями.
        /// </summary>
        /// <remarks>
        /// Правило может иметь параметры.
        /// Если параметры не указаны, то повышает любой урон персонажа.
        /// Если указаны теги экипировки, то увеличивает урон дейставий, выполняемых с использованием
        /// экипировки/предетов, имеющих все указанные теги.
        /// </remarks>
        Damage,

        /// <summary>
        /// Правило влияет на бросок на попадание.
        /// </summary>
        /// <remarks>
        /// Правило может иметь параметры.
        /// Если параметры не указаны, то повышает любой бросок на попадание.
        /// Если указаны теги экипировки, то повышает бросок на попадание дейставий, выполняемых с использованием
        /// экипировки/предетов, имеющих все указанные теги.
        /// </remarks>
        ToHit,

        /// <summary>
        /// Влияет на здоровье, если нет доспеха на тело.
        /// </summary>
        HealthIfNoBody,

        /// <summary>
        /// Влияет на шанс снижения характеристики - сытость.
        /// </summary>
        HungerResistance,

        /// <summary>
        /// Влияет на шанс снижения характеристики - вода.
        /// </summary>
        ThristResistance
    }
}