using System;

namespace Zilon.Core.Persons
{
    public class PerkEventArgs: EventArgs
    {
        public PerkEventArgs(IPerk perk)
        {
            Perk = perk;
        }

        public IPerk Perk { get; }
    }
}
