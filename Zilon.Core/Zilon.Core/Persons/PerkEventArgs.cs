using System;

using JetBrains.Annotations;

namespace Zilon.Core.Persons
{
    public class PerkEventArgs: EventArgs
    {
        public PerkEventArgs(IPerk perk)
        {
            Perk = perk;
        }

        [PublicAPI]
        public IPerk Perk { get; }
    }
}
