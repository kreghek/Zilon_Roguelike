using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    ///     Схема слота экипировки у персонажа.
    /// </summary>
    public class PersonSlotSubScheme : SubSchemeBase
    {
        /// <summary>
        ///     Типы слота.
        /// </summary>
        public EquipmentSlotTypes Types { get; set; }

        /// <summary>
        ///     Строкове представление объекта.
        /// </summary>
        /// <returns> Возвращает строкове представелние объекта. </returns>
        public override string ToString()
        {
            return Types.ToString();
        }
    }
}