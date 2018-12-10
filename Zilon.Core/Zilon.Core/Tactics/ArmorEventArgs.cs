using System;

namespace Zilon.Core.Tactics
{
    public sealed class ArmorEventArgs: EventArgs
    {
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
