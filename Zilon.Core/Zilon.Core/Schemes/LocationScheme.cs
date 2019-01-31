namespace Zilon.Core.Schemes
{
    /// <inheritdoc cref="ILocationScheme" />
    /// <summary>
    /// Схема узла на глобальной карте.
    /// </summary>
    public sealed class LocationScheme : SchemeBase, ILocationScheme
    {
        public ISectorSubScheme[] SectorLevels { get; }
    }
}
