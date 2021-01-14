namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема предмета для хранения характеристик при применении предмета.
    /// </summary>
    public interface IPropUseSubScheme
    {
        /// <summary>
        /// Rules when the prop used.
        /// </summary>
        ConsumeCommonRule[] CommonRules { get; }

        /// <summary>
        /// Rules to use the prop.
        /// </summary>
        IUsageRestrictions Restrictions { get; }

        /// <summary>
        /// Marks that when the resource used it reduce count by 1.
        /// </summary>
        /// <remarks>
        /// Only for <see cref="Resource"/>.
        /// </remarks>
        bool Consumable { get; }
    }

    public interface IUsageRestrictions
    {
        UsageRestrictionItem[] Items { get; }
    }

    public sealed class UsageRestrictionItem
    {
        public UsageRestrictionType Type { get; }
    }

    public sealed class UsageRestrictions : IUsageRestrictions
    {
        public UsageRestrictionItem[] Items { get; }
    }

    public enum UsageRestrictionType
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