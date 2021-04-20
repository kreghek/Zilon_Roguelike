using System;

using JetBrains.Annotations;

namespace Zilon.Core.Persons
{
    public class PerkEventArgs : EventArgs
    {
        public PerkEventArgs(ISkill perk)
        {
            Perk = perk;
        }

        [PublicAPI]
        public ISkill Perk { get; }
    }
}