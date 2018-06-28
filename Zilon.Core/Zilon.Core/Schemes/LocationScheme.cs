namespace Zilon.Core.Schemes
{
    /// <inheritdoc />
    /// <summary>
    /// Схема узла на глобальной карте.
    /// </summary>
    public sealed class LocationScheme: SchemeBase
    {
        public string MapSid { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
