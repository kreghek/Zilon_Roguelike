namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Base implemenetation of restriction set.
    /// </summary>
    public sealed class UsageRestrictions : IUsageRestrictions
    {
        public UsageRestrictionItem[] Items { get; }
    }
}