namespace Zilon.Core.Persons
{
    public static class PerkJobsEnumerableExtensions
    {
        //public static void UpdateProgress(this IEnumerable<JobSubScheme> jobs, Action<JobSubScheme> progressDelegate)
        //{
        //    if (jobs == null)
        //        return;

        //    foreach (var job in jobs)
        //    {
        //        progressDelegate(job);
        //    }
        //}

        //public static JobSubScheme[] Copy(this IEnumerable<JobSubScheme> jobs)
        //{
        //    if (jobs == null)
        //        return null;

        //    return jobs.Select(x => new JobSubScheme { Type = x.Type, Scope = x.Scope, Value = x.Value, Data = x.Data }).ToArray();
        //}

        //[Obsolete("Вероятно, что метод не пригодится. Удалить в dev10.")]
        //public static JobSubScheme[] CopyForCombat(this JobSubScheme[] jobs)
        //{
        //    if (jobs == null)
        //        return null;

        //    var resultJobs = new JobSubScheme[jobs.Length];
        //    for (var i = 0; i < jobs.Length; i++)
        //    {

        //        resultJobs[i] = new JobSubScheme
        //        {
        //            Type = jobs[i].Type,
        //            Scope = jobs[i].Scope,
        //            Data = jobs[i].Data,
        //            IsComplete = jobs[i].IsComplete
        //        };

        //        if (jobs[i].Scope != JobScope.Scenario)
        //        {
        //            resultJobs[i].Value = jobs[i].Value;
        //        }
        //        else
        //        {
        //            if (!jobs[i].IsComplete)
        //            {
        //                resultJobs[i].Value = 0;
        //            }
        //        }
        //    }

        //    return jobs.Select(x => new JobSubScheme { Type = x.Type, Scope = x.Scope, Value = x.Value, Data = x.Data }).ToArray();
        //}

        //public static JobSubScheme[] CopyWithoutProgress(this IEnumerable<JobSubScheme> jobs)
        //{
        //    if (jobs == null)
        //        return null;

        //    return jobs.Select(x => new JobSubScheme { Type = x.Type, Scope = x.Scope, Data = x.Data, IsComplete = x.IsComplete }).ToArray();
        //}
    }
}
