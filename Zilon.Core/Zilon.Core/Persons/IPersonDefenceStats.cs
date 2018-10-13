using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Характристики обороны персонажа против наступательных действий.
    /// </summary>
    public interface IPersonDefenceStats
    {
        /// <summary>
        /// Виды обороны, которыми владеет персонаж.
        /// </summary>
        PersonDefenceItem[] Defences { get; }

        /// <summary>
        /// Виды брони, которые есть у персонажа.
        /// </summary>
        PersonArmorItem[] Armors { get; }
    }
}