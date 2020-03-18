namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс схемы с возможностью мимикрирования.
    /// </summary>
    public interface IMimicScheme
    {
        /// <summary>
        /// Идентфиикатор схемы другого объекта,
        /// под который будет мимикрировать данный объект.
        /// Используется лже-предметами.
        /// </summary>
        string IsMimicFor { get; }
    }
}