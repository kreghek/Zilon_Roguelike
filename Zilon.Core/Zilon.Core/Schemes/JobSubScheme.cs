using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Структура для хранения работы.
    /// </summary>
    /// <remarks>
    /// Используется для хранения требуемых работ перка или квеста.
    /// </remarks>
    public sealed class JobSubScheme : IJobSubScheme
    {
        /// <summary>
        /// Тип работы.
        /// </summary>
        [JsonProperty]
        public JobType Type { get; private set; }

        /// <summary>
        /// Объём работы.
        /// </summary>
        [JsonProperty]
        public int Value { get; private set; }

        /// <summary>
        /// Область действия работы.
        /// </summary>
        [JsonProperty]
        public JobScope Scope { get; private set; }

        /// <summary>
        /// Дополнительные данные по работе.
        /// </summary>
        [JsonProperty]
        public string[] Data { get; private set; }

        //TODO Вернуть получение наименования работ для клиента. Доработать с учётом локализации.

        //public static string GetDescription(PerkJob schemeJob, PerkJob factJob, bool status = false)
        //{
        //    var description = GetDescriptionForType(schemeJob.Type, schemeJob.Data);

        //    if (schemeJob.Scope == JobScope.Scenario)
        //    {
        //        description += $": {schemeJob.Progress} за бой";
        //    }
        //    else if (schemeJob.Scope == JobScope.Global)
        //    {
        //        description += $": {factJob.Progress}/{schemeJob.Progress}";
        //    }

        //    if (factJob.IsComplete && status)
        //    {
        //        description = "!" + description;
        //    }

        //    return description;
        //}

        //public static string[] GetDescriptions(IEnumerable<PerkJob> schemeJobs, IEnumerable<PerkJob> factJobs)
        //{
        //    if (schemeJobs == null)
        //        return null;

        //    var resultList = new List<string>();
        //    foreach (var schemeJob in schemeJobs)
        //    {
        //        var factJob = factJobs.SingleOrDefault(x => x.Type == schemeJob.Type && x.Scope == schemeJob.Scope);
        //        var description = GetDescription(schemeJob, factJob);
        //        resultList.Add(description);
        //    }

        //    return resultList.ToArray();
        //}

        //private static string GetDescriptionForType(JobType jobType, string[] data)
        //{
        //    switch (jobType)
        //    {
        //        case JobType.Defeats:
        //            return "Повергнуть противников";

        //        case JobType.DefeatGaarns:
        //            return "Повергнуть Гаарн";

        //        case JobType.DefeatAleberts:
        //            return "Повергнуть сатиров";

        //        case JobType.DefeatLegions:
        //            return "Повергнуть легионеров";

        //        case JobType.DefeatTechbots:
        //            return "Повергнуть тех-ботов";

        //        case JobType.DefeatCults:
        //            return "Повергнуть культистов";

        //        case JobType.DefeatDeamons:
        //            return "Повергнуть демонов";

        //        case JobType.Blocks:
        //            return "Заблокировать ударов";

        //        case JobType.Hits:
        //            return "Нанести ударов";

        //        case JobType.MeleeHits:
        //            return "Нанести рукопашных ударов";

        //        case JobType.Crits:
        //            return "Получить критических ранений";

        //        case JobType.Combats:
        //            return "Поучавствовать в боях";

        //        case JobType.Victories:
        //            return "Победить в боях";

        //        case JobType.ReceiveHits:
        //            return "Получить ударов";

        //        case JobType.ReceiveDamage:
        //            return "Получить урона";

        //        case JobType.ReceiveDamagePercent:
        //            return "Получить урона (% от HP)";

        //        case JobType.DefeatClasses:
        //            return GetDefeatClassDescriptions(data);

        //        default:
        //            return jobType.ToString();
        //    }
        //}

        //private static string GetDefeatClassDescriptions(string[] data)
        //{
        //    var desc = "Defeat " + string.Join(", ", data);
        //    return desc;
        //}
    }
}
