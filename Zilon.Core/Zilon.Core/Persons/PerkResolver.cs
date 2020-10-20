using System.Linq;

using Zilon.Core.PersonModules;

namespace Zilon.Core.Persons
{
    public class PerkResolver : IPerkResolver
    {
        public void ApplyProgress(IJobProgress progress, IEvolutionModule evolutionData)
        {
            if (evolutionData == null)
            {
                return;
            }

            foreach (var perk in evolutionData.Perks)
            {
                if (perk.CurrentJobs is null)
                {
                    // Перки у которых нет работ, не могут развиваться.

                    // Некоторые перки (например врождённые таланты), не прокачиваются.
                    // Сразу игнорируем их.
                    continue;
                }

                var isPerkLevelCap = CheckLevelCap(perk);
                if (isPerkLevelCap)
                {
                    continue;
                }

                var affectedJobs = progress.ApplyToJobs(perk.CurrentJobs);

                foreach (var job in affectedJobs)
                {
                    // Опеределяем, какие из прогрессировавших работ завершены.
                    // И фиксируем их состояние завершения.
                    if (job.Progress >= job.Scheme.Value)
                    {
                        job.IsComplete = true;
                    }
                }

                // Опеределяем, все ли работы выполнены.
                var allJobsAreComplete = perk.CurrentJobs.All(x => x.IsComplete);

                if (allJobsAreComplete)
                {
                    evolutionData.PerkLevelUp(perk);
                }
            }
        }

        private bool CheckLevelCap(IPerk perk)
        {
            var currentLevel = perk.CurrentLevel;
            if (currentLevel == null)
            {
                return false;
            }

            var nextLevel = PerkHelper.GetNextLevel(perk.Scheme, currentLevel);

            var perkLevels = perk.Scheme.Levels;
            var maxLevel = perkLevels.Length - 1;
            var nextLevelOutOfRange = nextLevel.Primary > maxLevel;

            if (nextLevelOutOfRange)
            {
                return true;
            }

            var maxSubLevel = perkLevels[currentLevel.Primary].MaxValue;
            var currentSubLevelIsMax = currentLevel.Sub >= maxSubLevel;
            return currentSubLevelIsMax;
        }
    }
}