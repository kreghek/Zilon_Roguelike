using Zilon.Core.PersonModules;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class PerkResolver : IPerkResolver
    {
        public void ApplyProgress(IJobProgress progress, IEvolutionModule evolutionData)
        {
            if (progress is null)
            {
                throw new System.ArgumentNullException(nameof(progress));
            }

            if (evolutionData == null)
            {
                return;
            }

            foreach (IPerk perk in evolutionData.Perks)
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

                IJob[] affectedJobs = progress.ApplyToJobs(perk.CurrentJobs);

                foreach (IJob job in affectedJobs)
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

        private static bool CheckLevelCap(IPerk perk)
        {
            PerkLevel currentLevel = perk.CurrentLevel;
            if (currentLevel == null)
            {
                return false;
            }

            PerkLevel nextLevel = PerkHelper.GetNextLevel(perk.Scheme, currentLevel);

            PerkLevelSubScheme[] perkLevels = perk.Scheme.Levels;
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