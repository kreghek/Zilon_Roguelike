using System.Collections.Generic;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public class DefeatActorJobProgress : IJobProgress
    {
        public DefeatActorJobProgress(IActor actor)
        {
            Actor = actor;
        }

        public IActor Actor { get; }

        public JobSubScheme[] ApplyToJobs(IEnumerable<JobSubScheme> currentJobs)
        {
            var modifiedJobs = new List<JobSubScheme>();
            foreach (var job in currentJobs)
            {
                if (job.Type == JobType.Defeats)
                {
                    job.Value++;
                    modifiedJobs.Add(job);
                }
            }

            return modifiedJobs.ToArray();
        }
    }
}
