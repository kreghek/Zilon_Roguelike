namespace Zilon.Core.Schemes
{
    /// <inheritdoc cref="IPathScheme" />
    /// <summary>
    /// Схема ребра перехода между узлами на глобальной карте.
    /// </summary>
    public sealed class PathScheme : SchemeBase, IPathScheme
    {
        public string MapSid { get; set; }
        public string Sid1 { get; set; }
        public string Sid2 { get; set; }
    }
}
