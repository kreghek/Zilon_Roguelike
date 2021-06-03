using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема характеристик предмета, который можно экипировать на персонажа.
    /// </summary>
    public interface IPropEquipSubScheme
    {
        /// <summary>
        /// Идентификаторы действий, которые позволяет совершать предмет.
        /// </summary>
        string?[]? ActSids { get; }

        /// <summary>
        /// Характеристики брони, которую даёт предмет при экипировке.
        /// </summary>
        IPropArmorItemSubScheme?[]? Armors { get; }

        /// <summary>
        /// Правила, которые будут срабатывать при экипировке предмета.
        /// </summary>
        PersonRule?[]? Rules { get; }

        /// <summary>
        /// Типы слотов, в которые возможна экипировка предмета.
        /// </summary>
        EquipmentSlotTypes[]? SlotTypes { get; }

        IPropEquipRestrictions? EquipRestrictions { get; }
    }

    public interface IPropEquipRestrictions
    {
        /// <summary>
        /// Determines rules to keep thing in hands.
        /// By default, all equipment in hand slot is one-handed.
        /// </summary>
        PropHandUsage? PropHandUsage { get; }
    }

    public enum PropHandUsage
    {
        Undefined = 0,
        TwoHanded = 1
    }
}