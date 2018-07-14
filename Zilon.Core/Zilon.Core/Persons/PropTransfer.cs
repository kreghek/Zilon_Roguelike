namespace Zilon.Core.Persons
{
    /// <summary>
    /// Объект, хранящий данные оп переносе предметов между хранилищами.
    /// </summary>
    /// <remarks>
    /// Используется для формирования намерения переносить предметы между инвентярём,
    /// контейнерами (сундуками) и полом сектора.
    /// </remarks>
    public class PropTransfer
    {
        public IPropStore SourceStorage { get; set; }
        public IPropStore DestinationStorage { get; set; }
    }
}
