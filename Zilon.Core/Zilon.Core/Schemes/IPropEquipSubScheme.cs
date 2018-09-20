using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема характиристик предмета, который можно экипировать на персонажа.
    /// </summary>
    public interface IPropEquipSubScheme
    {
        /// <summary>
        /// Мощь. Влияет на все характиристики предмета.
        /// </summary>
        float Power { get; }

        /// <summary>
        /// Ранг пробития брони.
        /// </summary>
        int ApRank { get; }

        /// <summary>
        /// Ранг брони.
        /// </summary>
        int ArmorRank { get; }

        /// <summary>
        /// Доля поглощения урона при равном ранге пробития и брони.
        /// </summary>
        /// <remarks>
        /// Зависит от Мощи.
        /// </remarks>
        float Absorbtion { get; }

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