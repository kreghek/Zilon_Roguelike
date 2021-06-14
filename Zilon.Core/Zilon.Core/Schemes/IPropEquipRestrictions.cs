namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Restrictions to equip a prop.
    /// Example:
    /// - Two handed weapon can be equiped only if two hands if free.
    /// </summary>
    public interface IPropEquipRestrictions
    {
        /// <summary>
        /// Determines rules to keep thing in hands.
        /// By default, all equipment in hand slot is one-handed.
        /// </summary>
        PropHandUsage? PropHandUsage { get; }
    }
}