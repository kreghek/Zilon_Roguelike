using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Tactics
{
    public sealed class ArmorEventArgs: EventArgs
    {
        [ExcludeFromCodeCoverage]
        public ArmorEventArgs(int armorRank, int successRoll, int factRoll)
        {
            ArmorRank = armorRank;
            SuccessRoll = successRoll;
            FactRoll = factRoll;
        }

        public int ArmorRank { get; }
        public int SuccessRoll { get; }
        public int FactRoll { get; }
    }
}
