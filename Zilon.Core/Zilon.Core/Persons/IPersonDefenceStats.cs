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
        PersonArmorItem[] Armors { get; }

        /// <summary>
        /// Виды обороны, которыми владеет персонаж.
        /// </summary>
        PersonDefenceItem[] Defences { get; }

        /// <summary>
        /// Установка показаний брони для характеристик персонажа.
        /// </summary>
        /// <param name="armors"> Набор элементов брони. </param>
        void SetArmors(PersonArmorItem[] armors);
    }
}