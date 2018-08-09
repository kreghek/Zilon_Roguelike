using System.Linq;

namespace Zilon.Core.Persons
{
    public class PerkResolver : IPerkResolver
    {
        public void ApplyProgress(IJobProgress progress, IEvolutionData evolutionData)
        {
            if (evolutionData == null)
            {
                return;
            }

            foreach (var perk in evolutionData.Perks)
            {
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

            var maxLevel = perk.Scheme.Levels.Length - 1;
            var nextLevelOutOfRange = nextLevel.Primary > maxLevel;

            if (nextLevelOutOfRange)
            {
                return true;
            }
            else
            {
                var currentSubLevelIsMax = currentLevel.Sub >= perk.Scheme.Levels[currentLevel.Primary].MaxValue;
                if (currentSubLevelIsMax)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
