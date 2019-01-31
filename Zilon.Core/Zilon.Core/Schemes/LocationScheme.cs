namespace Zilon.Core.Schemes
{
    /// <inheritdoc cref="ILocationScheme" />
    /// <summary>
    /// Схема узла на глобальной карте.
    /// </summary>
    public sealed class LocationScheme : SchemeBase, ILocationScheme
    {
        /// <summary>
        /// Характеристики секторов по уровням.
        /// Если null, то в данной локации нет сектора.
        /// </summary>
        public ISectorSubScheme[] SectorLevels { get; }
    }
}
