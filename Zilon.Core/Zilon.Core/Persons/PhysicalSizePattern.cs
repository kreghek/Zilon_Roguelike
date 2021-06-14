namespace Zilon.Core.Persons
{
    /// <summary>
    /// The size of person says how much nodes e locks.
    /// </summary>
    public enum PhysicalSizePattern
    {
        Undefined = 0,

        /// <summary>
        /// Объект размеров в 1 ячейку.
        /// </summary>
        Size1,

        /// <summary>
        /// Объект размером в ячейку и её окружность.
        /// </summary>
        Size7
    }
}