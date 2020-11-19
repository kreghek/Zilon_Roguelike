using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <inheritdoc cref="IPropScheme" />
    /// <summary>
    /// Схема предмета. Общая для всех предметов в игре.
    /// </summary>
    public class PropScheme : SchemeBase, IPropScheme
    {
        /// <summary>
        /// Характеристики схемы, связанные с экипировкой предмета персонажем.
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<PropEquipSubScheme>))]
        [JsonProperty]
        public IPropEquipSubScheme Equip { get; private set; }

        /// <summary>
        /// Информация об использовании предмета.
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<PropUseSubScheme>))]
        [JsonProperty]
        public IPropUseSubScheme Use { get; private set; }

        /// <summary>
        /// Информация предмете, как .
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<PropBulletSubScheme>))]
        [JsonProperty]
        public IPropBulletSubScheme Bullet { get; private set; }

        /// <summary>
        /// Информации о создании/разборе предмета.
        /// </summary>
        [JsonProperty]
        public CraftSubScheme Craft { get; private set; }

        /// <summary>
        /// Теги предмета.
        /// </summary>
        /// <remarks>
        /// Теги используются для определения типа предмета. Например, для того, чтобы отследить
        /// возможность парного оружия.
        /// </remarks>
        [JsonProperty]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Используется массив в свойстве для десериализации.")]
        public string[] Tags { get; private set; }

        /// <summary>
        /// Идентфиикаторы схем других предметов,
        /// под которые будет мимикрировать данный предмет.
        /// Используется лже-предметами.
        /// </summary>
        [JsonProperty]
        public string IsMimicFor { get; private set; }
    }
}