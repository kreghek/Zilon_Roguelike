using System;
using System.Linq;

using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public class PerkResolver : IPerkResolver
    {
        private readonly IActorManager _actorManager;

        public PerkResolver(IActorManager actorManager)
        {
            _actorManager = actorManager;

            foreach (var actor in actorManager.Actors)
            {
                actor.OnDead += Actor_OnDead;
            }
        }

        private void Actor_OnDead(object sender, EventArgs e)
        {
            
        }

        public void CountDefeat(IActor hitActor, ITacticalAct act)
        {
            throw new NotImplementedException();
        }

        public void ApplyProgress(IJobProgress progress, IEvolutionData evolutionData)
        {
            if (evolutionData == null)
            {
                return;
            }

            foreach (var perk in evolutionData.Perks)
            {
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
