namespace Zilon.Core.StaticObjectModules
{
    /// <summary>
    ///     Module to damage static object; removable by brute force.
    /// </summary>
    public interface IDurabilityModule : IStaticObjectModule
    {
        /// <summary>
        ///     Current value of durability. Starting value is 1. If below 0, static object lost all durability.
        /// </summary>
        float Value { get; }

        /// <summary>
        ///     Take damage to static object.
        /// </summary>
        /// <param name="damageValue"> Row damage value. With no absorbtions and penalties from static object side. </param>
        void TakeDamage(int damageValue);
    }
}