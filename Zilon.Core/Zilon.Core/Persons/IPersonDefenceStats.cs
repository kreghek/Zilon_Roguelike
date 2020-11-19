using JetBrains.Annotations;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Характристики обороны персонажа против наступательных действий.
    /// </summary>
    public interface IPersonDefenceStats
    {
        /// <summary>
        /// Виды брони, которые есть у персонажа.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        PersonArmorItem[] Armors { get; }

        /// <summary>
        /// Виды обороны, которыми владеет персонаж.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        PersonDefenceItem[] Defences { get; }

        /// <summary>
        /// Установка показаний брони для характеристик персонажа.
        /// </summary>
        /// <param name="armors"> Набор элементов брони. </param>
        void SetArmors([NotNull][ItemNotNull] PersonArmorItem[] armors);
    }
}