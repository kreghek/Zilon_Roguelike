namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс для работы с экипировкой.
    /// </summary>
    public interface IEquipCarrier
    {
        /// <summary>
        /// Экипировка персонажа.
        /// </summary>
        Equipment[] Equipments { get; }
    }
}
