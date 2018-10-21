namespace Zilon.Core.Schemes
{
    /// <inheritdoc cref="ILocationScheme" />
    /// <summary>
    /// Схема узла на глобальной карте.
    /// </summary>
    public sealed class LocationScheme : SchemeBase, ILocationScheme
    {
        public string MapSid { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
