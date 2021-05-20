namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Object on the sector map to select by id.
    /// </summary>
    public interface IIdentifiableMapObject
    {
        /// <summary>
        /// Object's identifier.
        /// </summary>
        /// <remarks>
        /// Used in tests to select and check by id.
        /// </remarks>
        int Id { get; }
    }
}