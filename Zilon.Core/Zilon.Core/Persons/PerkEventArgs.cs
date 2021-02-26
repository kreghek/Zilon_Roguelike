using System;

using JetBrains.Annotations;

namespace Zilon.Core.Persons
{
    public class PerkEventArgs : EventArgs
    {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public PerkEventArgs(IPerk perk)
        {
            Perk = perk;
        }

        [PublicAPI]
        public IPerk Perk { get; }
    }
}