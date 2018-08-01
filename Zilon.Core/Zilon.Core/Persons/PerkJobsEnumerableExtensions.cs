using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public static class PerkJobsEnumerableExtensions
    {
        public static void UpdateProgress(this IEnumerable<PerkJob> jobs, Action<PerkJob> progressDelegate)
        {
            if (jobs == null)
                return;

            foreach (var job in jobs)
            {
                progressDelegate(job);
            }
        }

        public static PerkJob[] Copy(this IEnumerable<PerkJob> jobs)
        {
            if (jobs == null)
                return null;

            return jobs.Select(x => new PerkJob { Type = x.Type, Scope = x.Scope, Progress = x.Progress, Data = x.Data }).ToArray();
        }

        [Obsolete("Вероятно, что метод не пригодится. Удалить в dev10.")]
        public static PerkJob[] CopyForCombat(this PerkJob[] jobs)
        {
            if (jobs == null)
                return null;

            var resultJobs = new PerkJob[jobs.Length];
            for (var i = 0; i < jobs.Length; i++)
            {

                resultJobs[i] = new PerkJob
                {
                    Type = jobs[i].Type,
                    Scope = jobs[i].Scope,
                    Data = jobs[i].Data,
                    IsComplete = jobs[i].IsComplete
                };

                if (jobs[i].Scope != JobScope.Scenario)
                {
                    resultJobs[i].Progress = jobs[i].Progress;
                }
                else
                {
                    if (!jobs[i].IsComplete)
                    {
                        resultJobs[i].Progress = 0;
                    }
                }
            }

            return jobs.Select(x => new PerkJob { Type = x.Type, Scope = x.Scope, Progress = x.Progress, Data = x.Data }).ToArray();
        }

        public static PerkJob[] CopyWithoutProgress(this IEnumerable<PerkJob> jobs)
        {
            if (jobs == null)
                return null;

            return jobs.Select(x => new PerkJob { Type = x.Type, Scope = x.Scope, Data = x.Data, IsComplete = x.IsComplete }).ToArray();
        }
    }
}
