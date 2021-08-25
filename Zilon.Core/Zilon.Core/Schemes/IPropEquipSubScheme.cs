﻿using Zilon.Core.Components;

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

        IPropEquipRestrictions? EquipRestrictions { get; }

        /// <summary>
        /// Правила, которые будут срабатывать при экипировке предмета.
        /// </summary>
        PersonRule?[]? Rules { get; }

        /// <summary>
        /// Типы слотов, в которые возможна экипировка предмета.
        /// </summary>
        EquipmentSlotTypes[]? SlotTypes { get; }
    }
}