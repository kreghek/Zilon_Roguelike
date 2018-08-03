using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация данных по развитию персонажа.
    /// </summary>
    public sealed class EvolutionData : IEvolutionData
    {
        public EvolutionData()
        {
            ActivePerks = new IPerk[0];
            ArchievedPerks = new IPerk[0];
        }

        public IPerk[] ActivePerks { get; private set; }

        public IPerk[] ArchievedPerks { get; private set; }

        public event EventHandler<PerkEventArgs> PerkArchieved;

        public void ActivePerkArchieved(IPerk perk)
        {
            var activePerkIsValid = ActivePerks.Contains(perk);
            if (!activePerkIsValid)
            {
                throw new InvalidOperationException("Указанный перк не является активным.");
            }

            var activeList = new List<IPerk>(ActivePerks);
            var archievedList = new List<IPerk>(ArchievedPerks);

            activeList.Remove(perk);
            archievedList.Add(perk);

            ActivePerks = activeList.ToArray();
            ArchievedPerks = archievedList.ToArray();
            DoPerkArchieved(perk);
        }

        private void DoPerkArchieved(IPerk perk)
        {
            var eventArgs = new PerkEventArgs(perk);
            PerkArchieved?.Invoke(this, eventArgs);
        }
    }
}
