using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Структура для хранения работы.
    /// </summary>
    /// <remarks>
    /// Используется для хранения требуемых работ перка или квеста.
    /// </remarks>
    public sealed class JobSubScheme
    {
        /// <summary>
        /// Тип работы.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public JobType Type { get; set; }

        /// <summary>
        /// Объём работы.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int Value { get; set; }

        /// <summary>
        /// Область действия работы.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public JobScope Scope { get; set; }

        /// <summary>
        /// Дополнительные данные по работе.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string[] Data { get; set; }

        ////TODO Вынести весь этот подсчёт в отдельный сервис в пакете Persons
        //public void CountDefeat(IActor hitActor)
        //{
        //    //TODO Здесь должно быть извлечение текущей фракции персонажа.
        //    var faction = PersonFaction.Other;
        //    switch (Type)
        //    {
        //        case JobType.Defeats:
        //            Progress++;
        //            break;

        //        case JobType.DefeatGaarns:
        //            if (faction.HasFlag(PersonFaction.Gaarns))
        //                Progress++;
        //            break;

        //        case JobType.DefeatAleberts:
        //            if (faction.HasFlag(PersonFaction.Aleberts))
        //                Progress++;
        //            break;

        //        case JobType.DefeatLegions:
        //            if (faction.HasFlag(PersonFaction.Legion))
        //                Progress++;
        //            break;

        //        case JobType.DefeatDeamons:
        //            if (faction.HasFlag(PersonFaction.Deamons))
        //                Progress++;
        //            break;

        //        case JobType.DefeatTechbots:
        //            if (faction.HasFlag(PersonFaction.Techbots))
        //                Progress++;
        //            break;

        //        case JobType.DefeatCults:
        //            if (faction.HasFlag(PersonFaction.Cult))
        //                Progress++;
        //            break;
        //    }
        //}

        //public void CountKineticDeflection()
        //{
        //    if (Type == JobType.Blocks)
        //        Progress++;
        //}

        //public void CountHit()
        //{
        //    if (Type == JobType.Hits)
        //        Progress++;
        //}

        //public void CountMeleeHits()
        //{
        //    if (Type == JobType.MeleeHits)
        //        Progress++;
        //}

        //public void CountOneUseHits(int count)
        //{
        //    if (Type == JobType.OneUseHits)
        //        Progress = count;
        //}

        //public void CountOneUseDefeats(int count)
        //{
        //    if (Type == JobType.OneUseDefeats)
        //        Progress = count;
        //}

        //public void CountHitsReceived()
        //{
        //    if (Type == JobType.ReceiveHits)
        //        Progress++;
        //}

        //public void CountDamageReceived(int damage)
        //{
        //    if (Type == JobType.ReceiveDamage)
        //        Progress += damage;
        //}

        //public void CountDamageReceivedPercent(int damage)
        //{
        //    if (Type == JobType.ReceiveDamagePercent)
        //        Progress += damage;
        //}

        //public void CountCrits()
        //{
        //    if (Type == JobType.Crits)
        //        Progress++;
        //}

        //public void CountCombat(bool winner)
        //{
        //    if (Type == JobType.Combats)
        //        Progress++;

        //    if (Type == JobType.Victories && winner)
        //        Progress++;
        //}

        //public bool CountCraft(IProp product)
        //{
        //    if (Type == JobType.CraftAssaultRifle)
        //    {
        //        if (product.Scheme.Sid == "assault-rifle" ||
        //            product.Scheme.Sid == "assault-rifle-bs" ||
        //            product.Scheme.Sid == "protbane-rifle")
        //        {
        //            Progress++;
        //            return true;
        //        }
        //    }

        //    if (Type == JobType.Craft)
        //    {
        //        if (Data == null)
        //        {
        //            Progress++;
        //            return true;
        //        }
        //        else
        //        {
        //            const string professionDataKey = "profession";
        //            foreach (var dataItem in Data)
        //            {
        //                if (dataItem.StartsWith(professionDataKey))
        //                {
        //                    var professionKey = dataItem.Remove(0, professionDataKey.Length + 1);
        //                    var professionType = (ProfessionTypes)Enum.Parse(typeof(ProfessionTypes), professionKey);
        //                    var usedProfession = product.Scheme.Craft.Professions.SingleOrDefault(x => x.Profession == professionType);
        //                    if (usedProfession != null)
        //                    {
        //                        switch (product)
        //                        {
        //                            case Equipment equipment:
        //                                Progress++;
        //                                break;

        //                            case Resource resource:
        //                                Progress += resource.Count;
        //                                break;

        //                            default:
        //                                throw new Exception($"Данный продукт {product} не может учитываться в работам по крафту.");
        //                        }

        //                        return true;
        //                    }
        //                }
        //            }
        //        }
        //    }



        //    return false;
        //}

        //public bool CountDefeatGenerations(string defeatedClass)
        //{
        //    if (Type != JobType.DefeatClasses)
        //    {
        //        return false;
        //    }

        //    var jobClasses = Data;
        //    if (jobClasses.Contains(defeatedClass))
        //    {
        //        Progress++;
        //        return true;
        //    }

        //    return false;
        //}

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
