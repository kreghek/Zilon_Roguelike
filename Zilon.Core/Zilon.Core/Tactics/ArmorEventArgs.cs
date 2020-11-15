namespace Zilon.Core.Tactics
{
    public sealed class ArmorEventArgs : EventArgs
    {
        [ExcludeFromCodeCoverage]
        public ArmorEventArgs(int armorRank, int successRoll, int factRoll)
        {
            ArmorRank = armorRank;
            SuccessRoll = successRoll;
            FactRoll = factRoll;
        }

        /// <summary>
        /// Ранг брони цели.
        /// </summary>
        [PublicAPI]
        public int ArmorRank { get; }

        /// <summary>
        /// Фактический бросок.
        /// </summary>
        [PublicAPI]
        public int FactRoll { get; }

        /// <summary>
        /// Бросок, который нужен был для упешного использования брони.
        /// </summary>
        [PublicAPI]
        public int SuccessRoll { get; }
    }
}