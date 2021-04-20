using System.Linq;

using Zilon.Core.Persons;

namespace Zilon.Core.Skills
{
    public class PerkResolver<TJobScheme> : IPerkResolver<TJobScheme> where TJobScheme : IMinimalJobSubScheme
    {
        private static bool CheckLevelCap(ISkill<TJobScheme> perk)
        {
            var currentLevel = perk.CurrentLevel;
            if (currentLevel == null)
            {
                return false;
            }

            return !PerkHelper.HasNextLevel(perk.Scheme, currentLevel);
        }

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
    }
}