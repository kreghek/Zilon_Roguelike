namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Restriction item as a element of a restriction set.
    /// </summary>
    public interface IUsageRestrictionItem
    {
        /// <summary>
        /// Restriction rule.
        /// </summary>
        UsageRestrictionRule Type { get; }
    }
}