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
        /// Marks that when the resource used it reduce count by 1.
        /// </summary>
        /// <remarks>
        /// Only for <see cref="Resource" />.
        /// </remarks>
        bool Consumable { get; }

        /// <summary>
        /// Rules to use the prop.
        /// </summary>
        IUsageRestrictions Restrictions { get; }
    }
}