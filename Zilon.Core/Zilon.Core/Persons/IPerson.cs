namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс персонажа.
    /// </summary>
    /// <remarks>
    /// Персонаж - это описание игрового объекта за пределами тактических боёв.
    /// </remarks>
    public interface IPerson
    {
        int Id { get; set; }

        /// <summary>
        /// Хитпоинты персонажа.
        /// </summary>
        float Hp { get; }

        /// <summary>
        /// Экипировка персонажа.
        /// </summary>
        Equipment[] Equipments { get; }
    }
}