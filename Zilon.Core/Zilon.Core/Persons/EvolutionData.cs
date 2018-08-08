using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация данных по развитию персонажа.
    /// </summary>
    public sealed class EvolutionData : IEvolutionData
    {
        private readonly ISchemeService _schemeService;

        public EvolutionData(ISchemeService schemeService)
        {
            _schemeService = schemeService;

            UpdatePerks();
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

        private void UpdatePerks()
        {
            var schemes = _schemeService.GetSchemes<PerkScheme>();

            var perks = new List<IPerk>();

            foreach (var perkScheme in schemes)
            {
                var perk = new Perk {
                    Scheme = perkScheme,
                    CurrentLevel = new PerkLevel(null, 0),
                    CurrentJobs = perkScheme.Levels[0].Jobs.Select(x=>new PerkJob(x)).ToArray()
                };
            }

            Perks = perks.ToArray();
        }
    }
}
