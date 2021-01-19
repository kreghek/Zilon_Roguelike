namespace Zilon.Core.Schemes
{
    /// <summary>
    /// The rule of restriction to use prop.
    /// </summary>
    public enum UsageRestrictionRule
    {
        Undefined,

        /// <summary>
        /// Persons can use this prop only when no monsters near.
        /// </summary>
        OnlySafeEnvironment,

        /// <summary>
        /// Persons can use this prop only when they has no hunger in critical state.
        /// </summary>
        NoStarvation,

        /// <summary>
        /// Persons can use this prop only when they has no thrist in critical state.
        /// </summary>
        NoDehydration,

        /// <summary>
        /// Persons can use this prop only when they has no intoxication in critical state.
        /// </summary>
        NoOverdose
    }
}