using Zilon.Core.Common;
using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема характеристик предмета, который можно экипировать на персонажа.
    /// </summary>
    public interface IPropEquipSubScheme
    {
        /// <summary>
        /// Характеристики брони, которую даёт предмет при экипировке.
        /// </summary>
        IPropArmorItemSubScheme[] Armors { get; }

        /// <summary>
        /// Идентификаторы действий, которые позволяет совершать предмет.
        /// </summary>
        string[] ActSids { get; }

        /// <summary>
        /// Типы слотов, в которые возможна экипировка предмета.
        /// </summary>
        EquipmentSlotTypes[] SlotTypes { get; }
    }
}