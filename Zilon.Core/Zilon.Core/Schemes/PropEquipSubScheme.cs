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

        [JsonConverter(typeof(ConcreteTypeConverter<PropArmorItemSubScheme[]>))]
        public IPropArmorItemSubScheme[] Armors { get; set; }
    }
}
