using System;
using System.Diagnostics.CodeAnalysis;

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
        public int ArmorRank { get; }

        /// <summary>
        /// Фактический бросок.
        /// </summary>
        public int FactRoll { get; }

        /// <summary>
        /// Бросок, который нужен был для упешного использования брони.
        /// </summary>
        public int SuccessRoll { get; }
    }
}