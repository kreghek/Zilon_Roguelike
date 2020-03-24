namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема локации в провинции на глобальной карте.
    /// </summary>
    public interface ILocationScheme: IScheme
    {
        /// <summary>
        /// Характеристики секторов по уровням.
        /// Если null, то в данной локации нет сектора.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Используется для десериализации")]
        ISectorSubScheme[] SectorLevels { get; }
    }
}