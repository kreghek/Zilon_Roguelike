namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Rules to use the prop.
    /// </summary>
    public interface IUsageRestrictions
    {
        /// <summary>
        /// Restriction item.
        /// </summary>
        /// <remarks>
        /// All items must be checked. If no item restrict usage then prop can be used.
        /// </remarks>
        UsageRestrictionItem[] Items { get; }
    }
}