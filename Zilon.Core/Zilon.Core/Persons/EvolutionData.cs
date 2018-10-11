using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
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

            Stats = new[] {
                new SkillStatItem{Stat = SkillStatType.Ballistic, Value = 10 },
                new SkillStatItem{Stat = SkillStatType.Melee, Value = 10 }
            };

            UpdatePerks();
        }

        /// <summary>
        /// Перечень навыков.
        /// </summary>
        public SkillStatItem[] Stats { get; }

        public IPerk[] Perks { get; private set; }

        public event EventHandler<PerkEventArgs> PerkLeveledUp;

        public void PerkLevelUp(IPerk perk)
        {
            var activePerkIsValid = Perks.Contains(perk);
            if (!activePerkIsValid)
            {
                throw new InvalidOperationException("Указанный перк не является активным для текущего актёра.");
            }

            var nextLevel = PerkHelper.GetNextLevel(perk.Scheme, perk.CurrentLevel);

            perk.CurrentLevel = nextLevel;

            UpdatePerks();

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
            if (Perks != null)
            {
                perks.AddRange(Perks);
            }

            foreach (var perkScheme in schemes)
            {
                if (Perks != null)
                {
                    var existingPerk = Perks.SingleOrDefault(x => x.Scheme == perkScheme);
                    if (existingPerk != null)
                    {
                        continue;
                    }
                }

                var perk = new Perk
                {
                    Scheme = perkScheme,
                    CurrentLevel = null,
                    CurrentJobs = perkScheme.Levels[0].Jobs.Select(x => new PerkJob(x)).ToArray()
                };

                perks.Add(perk);
            }

            Perks = perks.ToArray();
        }
    }
}
