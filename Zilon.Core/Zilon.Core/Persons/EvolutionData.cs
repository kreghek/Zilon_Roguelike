using System;
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
            Perks = new IPerk[0];
        }

        public IPerk[] Perks { get; private set; }

        public event EventHandler<PerkEventArgs> PerkLeveledUp;

        public void PerkLevelUp(IPerk perk)
        {
            var activePerkIsValid = Perks.Contains(perk);
            if (!activePerkIsValid)
            {
                throw new InvalidOperationException("Указанный перк не является активным для текущего актёра.");
            }

            var currentLevel = perk.CurrentLevel.Primary;
            var currentSubLevel = perk.CurrentLevel.Sub;

            var nextLevel = PerkHelper.GetNextLevel(perk.Scheme, perk.CurrentLevel);

            perk.CurrentLevel = nextLevel;

            DoPerkArchieved(perk);
        }

        private void DoPerkArchieved(IPerk perk)
        {
            var eventArgs = new PerkEventArgs(perk);
            PerkLeveledUp?.Invoke(this, eventArgs);
        }
    }
}
