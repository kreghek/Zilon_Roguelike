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
        ISectorSubScheme[] SectorLevels { get; }
    }
}