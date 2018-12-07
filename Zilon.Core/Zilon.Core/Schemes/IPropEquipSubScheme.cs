using Zilon.Core.Common;
using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема характеристик предмета, который можно экипировать на персонажа.
    /// </summary>
    public interface IPropEquipSubScheme
    {
        ImpactType Impact { get; }
        PersonRuleLevel AbsorbtionLevel { get; }
        int ArmorRank { get; }

        

        /// <summary>
        /// Ранг брони.
        /// </summary>
        // ReSharper disable once UnusedMemberInSuper.Global
        int ArmorRank { get; }

        /// <summary>
        /// Доля поглощения урона при равном ранге пробития и брони.
        /// </summary>
        /// <remarks>
        /// Зависит от Мощи.
        /// </remarks>
        // ReSharper disable once UnusedMemberInSuper.Global
        float Absorption { get; }

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