using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема слота экипировки у персонажа.
    /// </summary>
    public class PersonSlotSubScheme : SubSchemeBase
    {
        /// <summary>
        /// Типы слота.
        /// </summary>
        public EquipmentSlotTypes Types { get; set; }

        /// <summary>
        /// Specify main slot.
        /// Main slot used to equip two-handed weapon.
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// The name of slot displayed in UI or logs.
        /// </summary>
        public LocalizedStringSubScheme? Name { get; set; }

        /// <summary>
        /// Строкове представление объекта.
        /// </summary>
        /// <returns> Возвращает строкове представелние объекта. </returns>
        public override string ToString()
        {
            var mainHandMarker = IsMain ? "-main" : string.Empty;
            return $"{Types}{mainHandMarker}";
        }
    }
}