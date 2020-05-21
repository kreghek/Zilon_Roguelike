namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Модуль перемещения персонажа.
    /// </summary>
    public interface IMovingModule: IPersonModule
    {
        /// <summary>
        /// Рассчитать стоимость перемещения на один шаг.
        /// </summary>
        /// <returns> Возращает стоимость перемещения. </returns>
        int CalculateCost();
    }
}