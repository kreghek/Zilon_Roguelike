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
        /// Мощь. Влияет на все характиристики предмета.
        /// </summary>
        [JsonProperty]
        public float Power { get; private set; }

        /// <summary>
        /// Ранг пробития брони.
        /// </summary>
        [JsonProperty]
        public int ApRank { get; private set; }

        /// <summary>
        /// Ранг брони.
        /// </summary>
        [JsonProperty]
        public int ArmorRank { get; private set; }

        /// <summary>
        /// Доля поглощения урона при равном ранге пробития и брони.
        /// </summary>
        /// <remarks>
        /// Зависит от Мощи.
        /// </remarks>
        [JsonProperty]
        public float Absorption { get; private set; }

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
    }
}
