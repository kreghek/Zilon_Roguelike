using Newtonsoft.Json;
using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема характиристик предмета, который можно экипировать на персонажа.
    /// </summary>
    public sealed class PropEquipSubScheme : SubSchemeBase, IPropEquipSubScheme
    {
        /// <summary>
        /// Идентификаторы действий, которые позволяет совершать предмет.
        /// </summary>
        [JsonProperty]
        public string[] ActSids { get; private set; }

        /// <summary>
        /// Типы слотов, в которые возможна экипировка предмета.
        /// </summary>
        [JsonProperty]
        public EquipmentSlotTypes[] SlotTypes { get; private set; }

        /// <summary>
        /// Характеристики брони, которую даёт предмет при экипировке.
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<PropArmorItemSubScheme[]>))]
        [JsonProperty]
        public IPropArmorItemSubScheme[] Armors { get; private set; }

        /// <summary>
        /// Правила, которые будут срабатывать при экипировке предмета.
        /// </summary>
        [JsonProperty]
        public PersonRule[] Rules { get; private set; }
    }
}
