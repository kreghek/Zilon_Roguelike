using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core
{
    public static class PerkHelper
    {
        //public static Perk CreatePerk(PerkScheme perkScheme, int? perkLevel, int perkSubLevel, JobSubScheme[] doneLevelJobs, PerkState state, bool isLevelPaid,
        //    IDictionary<string, PerkScheme> perkSchemeDict, IDictionary<string, PropScheme> propSchemeDict)
        //{
        //    var perk = new Perk();

        //    perk.Scheme = perkScheme;

        //    perk.CurrentLevel = perkLevel;
        //    perk.CurrentSubLevel = perkSubLevel;

        //    CalcLevelScheme(perkScheme.Levels, perkLevel, perkSubLevel, out PerkLevelSubScheme archievedLevelScheme, out PerkLevelSubScheme nextLevelScheme);
        //    perk.ArchievedLevelScheme = archievedLevelScheme;
        //    perk.TargetLevelScheme = nextLevelScheme;
        //    perk.IsLevelPaid = isLevelPaid;

        //    if (doneLevelJobs != null)
        //    {
        //        perk.CurrentJobs = doneLevelJobs;
        //    }
        //    else
        //    {
        //        if (perk.TargetLevelScheme != null)
        //        {
        //            perk.CurrentJobs = perk.TargetLevelScheme.Jobs.CopyWithoutProgress();
        //        }
        //    }

        //    perk.State = state;

        //    return perk;
        //}

        public static PerkLevel GetNextLevel(PerkScheme perkScheme, PerkLevel level)
        {
            if (level == null)
            {
                return new PerkLevel(0, 0);
            }

            var currentLevel = level.Primary;
            var currentSubLevel = level.Sub;

            var currentLevelScheme = perkScheme.Levels[currentLevel];

            if (currentSubLevel + 1 > currentLevelScheme.MaxValue)
            {
                currentSubLevel = 0;
                currentLevel++;
            }
            else
            {
                currentSubLevel++;
            }

            return new PerkLevel(currentLevel, currentSubLevel);
        }

        //public static PerkRuleSubScheme[] GetAllPerkRules(PerkScheme perkScheme, int? perkLevel, int perkSubLevel)
        //{
        //    if (perkLevel == null)
        //        return new PerkRuleSubScheme[0];

        //    var ruleList = new List<PerkRuleSubScheme>();
        //    for (var i = 0; i <= perkLevel.Value; i++)
        //    {
        //        var archievedSubLevel = 0;
        //        if (perkLevel.Value < i)
        //        {
        //            archievedSubLevel = perkScheme.Levels[i].MaxValue;
        //        }
        //        else
        //        {
        //            archievedSubLevel = Math.Min(perkSubLevel, perkScheme.Levels[i].MaxValue);
        //        }

        //        var levelRules = perkScheme.Levels[i].Rules;
        //        if (levelRules != null)
        //        {
        //            for (var j = 0; j <= archievedSubLevel; j++)
        //            {
        //                ruleList.AddRange(levelRules);
        //            }
        //        }
        //    }

        //    return ruleList.ToArray();
        //}

        //public static string DetectPerkChangesClass(PerkScheme perkScheme, int? currentLevel)
        //{
        //    if (currentLevel == null)
        //        return null;

        //    var rules = perkScheme.Levels[currentLevel.Value].Rules;
        //    if (rules == null)
        //        return null;

        //    var changeClassRule = rules.FirstOrDefault(x => x.ChangeClass != null);
        //    if (changeClassRule != null)
        //        return changeClassRule.ChangeClass;

        //    return null;
        //}

        ///// <summary>
        ///// Вычисляет прокаченную и целевую схему уровня перка.
        ///// </summary>
        ///// <param name="levelSchemes"> Все схемы уровня в порядке достижения. </param>
        ///// <param name="level"> Текущий уровень перка. </param>
        ///// <param name="subLevel"> Текущий подуровень перка. </param>
        ///// <param name="archievedLevelScheme">
        ///// Схема уровня перка, которая считается полученной на текущем уровне.
        ///// </param>
        ///// <param name="nextLevelScheme">
        ///// Схема уровня перка, которая будет следующей после достигнутой.
        ///// </param>
        //public static void CalcLevelScheme(PerkLevelSubScheme[] levelSchemes,
        //    int? level,
        //    int subLevel,
        //    out PerkLevelSubScheme archievedLevelScheme,
        //    out PerkLevelSubScheme nextLevelScheme)
        //{
        //    //Если уровень не определён, то перк ни на сколько не прокачен.
        //    // Значит берём первую схему уровня.
        //    if (level == null)
        //    {
        //        archievedLevelScheme = null;
        //        nextLevelScheme = levelSchemes[0];
        //        return;
        //    }

        //    // Извлекаем текущую схему уровня согласно текущему уровню..
        //    archievedLevelScheme = levelSchemes[level.Value];

        //    // Если все уровни перка прокачены, то возвращаем null.
        //    // Это означает, что дальнейшей прокачки нет. LevelCap
        //    if (level.Value + 1 < levelSchemes.Length)
        //    {
        //        // Если не все подуровни прокачены, то оставляем текущую схему подуровня.
        //        // Иначе возвращаем схему следующего уровеня перка.
        //        if (archievedLevelScheme.MaxValue > subLevel)
        //        {
        //            nextLevelScheme = archievedLevelScheme;
        //        }
        //        else
        //        {
        //            nextLevelScheme = levelSchemes[level.Value + 1];
        //        }
        //    }
        //    else
        //    {
        //        nextLevelScheme = null;
        //    }
        //}

        //public static Perk[] GetAllPerks(IEnumerable<PerkScheme> targetPerkSchemes, IDictionary<string, PerkScheme> perkSchemeDict,
        //     IDictionary<string, PropScheme> propSchemeDict)
        //{
        //    var perks = new List<Perk>();
        //    foreach (var perkScheme in targetPerkSchemes)
        //    {
        //        try
        //        {
        //            var perk = CreatePerk(perkScheme, null, 0, null, PerkState.None, false, perkSchemeDict, propSchemeDict);
        //            perks.Add(perk);
        //        }
        //        catch (Exception exception)
        //        {
        //            Logger.TraceError(LogCodes.ErrorCommon, $"Ошибка при обработки перка {perkScheme.Sid}.", exception);
        //        }
        //    }

        //    return perks.ToArray();
        //}

        ///// <summary>
        ///// Рассчитывает уровень персонажа исходя из прокаченных перков.
        ///// </summary>
        ///// <param name="perks">Прокаченные перки. Внутри всё равно есть проверка на наличие уровня у перка.</param>
        ///// <returns>Уровень персонажа</returns>
        //public static float CalcPersonLevel(IEnumerable<Perk> perks)
        //{
        //    var totalArchievedPersonLevel = 0f;
        //    foreach (var perk in perks)
        //    {
        //        if (perk.CurrentLevel == null)
        //            continue;

        //        var perkScheme = perk.Scheme;
        //        for (var i = 0; i <= perk.CurrentLevel.Value; i++)
        //        {
        //            var levelScheme = perkScheme.Levels[i];

        //            var perkPersonLevel = levelScheme.PersonLevel.Base + levelScheme.PersonLevel.LevelInc * perk.CurrentSubLevel;

        //            totalArchievedPersonLevel += perkPersonLevel;
        //        }
        //    }

        //    return totalArchievedPersonLevel + 1;
        //}

        //public static Perk[] FilterAvailability(IEnumerable<Perk> perks, PerkAvailabilityContext availabilityContext)
        //{
        //    var availablePerks = new List<Perk>();
        //    foreach (var perk in perks)
        //    {
        //        var isAvailable = IsAvailable(perk, availabilityContext);
        //        var isVisible = IsVisible(perk, availabilityContext);

        //        if (isAvailable || isVisible)
        //        {
        //            perk.IsAvailable = isAvailable;
        //            availablePerks.Add(perk);
        //        }
        //    }

        //    return availablePerks.ToArray();
        //}

        //public static bool IsPerkVisible(Perk perk, PerkAvailabilityContext availabilityContext)
        //{
        //    var isAvailableByCondition = IsAvailable(perk, availabilityContext);
        //    var isVisible = IsVisible(perk, availabilityContext);

        //    return isAvailableByCondition || isVisible;
        //}

        //public static bool CheckConditions(PerkConditionSubScheme[] conditions, PerkAvailabilityContext availabilityContext)
        //{
        //    foreach (var condition in conditions)
        //    {
        //        if (!CheckOneCondition(condition, availabilityContext))
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //public static PropSet[] CalcTotalSources(PerkScheme perkScheme, int? perkLevel, int perkSubLevel, IDictionary<string, PropScheme> propSchemeDict)
        //{
        //    // Вычисляем текущий уровень перка
        //    CalcLevelScheme(perkScheme.Levels, perkLevel, perkSubLevel,
        //        out PerkLevelSubScheme archievedLevelScheme, out PerkLevelSubScheme targetLevelScheme);

        //    // Вычисляем стоимость
        //    var totalSources = new List<PropSet>();
        //    if (perkScheme.Sources != null)
        //    {
        //        foreach (var propSet in perkScheme.Sources)
        //        {
        //            //TODO Вынести в отдельный метод глубокое клонирование набора ресурсов
        //            totalSources.Add(propSet.Clone());
        //        }
        //    }

        //    if (targetLevelScheme != null && targetLevelScheme.Sources != null)
        //    {
        //        foreach (var propSet in targetLevelScheme.Sources)
        //        {
        //            if (!propSchemeDict.TryGetValue(propSet.PropSid, out PropScheme propScheme))
        //            {
        //                Logger.TraceError(LogCodes.ErrorCommon, $"Не найден предмет с Sid:{propSet.PropSid} при вычислении стоимости уровня {perkLevel}/{perkSubLevel} в перке {perkScheme.Sid}.");
        //            }

        //            var propClass = propScheme.GetPropClass();
        //            switch (propClass)
        //            {
        //                case PropClass.Equipment:
        //                    //Работаем с экипировкой
        //                    var equipmentPropSet = propSet.Clone();
        //                    equipmentPropSet.Count = 1;
        //                    totalSources.Add(equipmentPropSet);
        //                    break;

        //                case PropClass.Resource:
        //                    // Работает с ресурсами
        //                    var existsResource = totalSources.SingleOrDefault(x => x.PropSid == propSet.PropSid);
        //                    if (existsResource == null)
        //                    {
        //                        totalSources.Add(new PropSet
        //                        {
        //                            PropSid = propSet.PropSid,
        //                            Count = propSet.Count
        //                        });
        //                    }
        //                    else
        //                    {
        //                        existsResource.Count += propSet.Count;
        //                    }
        //                    break;

        //                default:
        //                    throw new AppException($"Для предмета Sid:{propScheme.Sid} класса {propClass} нет возможности учавствовать в качестве ресурса для перка. Sid перка: {perkScheme.Sid}");
        //            }
        //        }
        //    }

        //    return totalSources.ToArray();
        //}

        /// <summary>
        /// Преобразование суммарного уровня в уровень/подуровень для конкретной схемы перка.
        /// </summary>
        /// <param name="perkScheme">Схема.</param>
        /// <param name="totalLevel">Суммарный уровень.</param>
        /// <param name="level">Уровень перка.</param>
        /// <param name="subLevel">Подуровень перка.</param>
        public static void ConvertTotalLevel(PerkScheme perkScheme, int totalLevel, out int? level, out int? subLevel)
        {
            var levelRemains = totalLevel + 1;
            var currentLevelPointer = 0;
            var currentLevelCapability = 0;
            do
            {
                if (currentLevelPointer >= perkScheme.Levels.Length)
                {
                    level = null;
                    subLevel = null;
                    return;
                }

                var levelScheme = perkScheme.Levels[currentLevelPointer];
                level = currentLevelPointer;
                subLevel = levelRemains - 1;
                currentLevelPointer++;
                currentLevelCapability = levelScheme.MaxValue + 1;
                levelRemains -= currentLevelCapability;
            } while (levelRemains >= currentLevelCapability);
        }

        /// <summary>
        /// Преобразование уровня/подуровня в суммарный уровень.
        /// </summary>
        /// <param name="perkScheme">Схема.</param>
        /// <param name="level">Уровень перка.</param>
        /// <param name="subLevel">Подуровень перка.</param>
        /// <returns></returns>
        public static int? ConvertLevel(PerkScheme perkScheme, int? level, int subLevel)
        {
            if (level == null)
                return null;

            var sum = 0;
            for (var i = 0; i <= level; i++)
            {
                if (i < level)
                {
                    sum += perkScheme.Levels[i].MaxValue + 1;
                }
                else
                {
                    sum += subLevel;
                }
            }

            return sum;
        }

      //  /// <summary>
      //  /// Расчёт суммарного бонуса перка с учётом всех полученных уровней/подуровней.
      //  /// </summary>
      //  /// <returns>Объект бонуса.</returns>
      //  public static Bonus CalcDoneBonus(PerkScheme perkScheme, int level, int subLevel)
      //  {
      //      var bonus = new Bonus();

      //      for (var i = 0; i <= level; i++)
      //      {
      //          var levelScheme = perkScheme.Levels[i];
      //          var currentSubLevel = 0;
      //          if (i < level)
      //          {
      //              currentSubLevel = perkScheme.Levels[i].MaxValue + 1;

      //          }
      //          else
      //          {
      //              currentSubLevel = subLevel;
      //          }

      //          if (levelScheme.Rules != null)
      //          {
      //              foreach (var rule in levelScheme.Rules)
      //              {
      //                  if (rule.SkillSid != null)
      //                  {
      //                      if (bonus.SkillSid != null)
      //                      {
      //                          throw new AppException("Еще не реализовано.");
      //                      }

      //                      bonus.SkillSid = rule.SkillSid;
      //                  }

      //                  if (rule.HP != null)
      //                  {
      //                      bonus.Hp += rule.HP.Base;
      //                  }

      //                  if (rule.Str != null)
      //                  {
      //                      bonus.Str += rule.Str.Base;
      //                  }

      //                  if (rule.Agi != null)
      //                  {
      //                      bonus.Agi += rule.Agi.Base;
      //                  }

      //                  if (rule.Int != null)
      //                  {
      //                      bonus.Int += rule.Int.Base;
      //                  }

      //                  if (rule.Ini != null)
      //                  {
      //                      bonus.Ini += rule.Ini.Base;
      //                  }

      //                  if (rule.Eff != null)
      //                  {
      //                      bonus.Eff += rule.Eff.Base;
      //                  }

      //                  if (rule.MinEff != null)
      //                  {
      //                      bonus.MinEff += rule.MinEff.Base;
      //                  }

      //                  if (rule.MaxEff != null)
      //                  {
      //                      bonus.MaxEff += rule.MaxEff.Base;
      //                  }

      //                  if (rule.EffP != null)
      //                  {
      //                      bonus.EffP += rule.EffP.Base;
      //                  }

      //                  if (rule.MinEffP != null)
      //                  {
      //                      bonus.MinEffP += rule.MinEffP.Base;
      //                  }

      //                  if (rule.MaxEffP != null)
      //                  {
      //                      bonus.MaxEffP += rule.MaxEffP.Base;
      //                  }
      //              }
      //          }
      //      }

      //      return bonus;
      //  }

      //  private static bool IsAvailable(Perk perk, PerkAvailabilityContext availabilityContext)
      //  {
      //      if (perk.Scheme.BaseConditions != null)
      //      {
      //          var isAvailable = CheckConditions(perk.Scheme.BaseConditions, availabilityContext);
      //          if (!isAvailable)
      //          {
      //              return false;
      //          }
      //      }

      //      if (perk.TargetLevelScheme != null && perk.TargetLevelScheme.Conditions != null)
      //      {
      //          var isAvailable = CheckConditions(perk.TargetLevelScheme.Conditions, availabilityContext);
      //          if (!isAvailable)
      //          {
      //              return false;
      //          }
      //      }

      //      return true;
      //  }

      //  private static bool IsVisible(Perk perk, PerkAvailabilityContext availabilityContext)
      //  {
      //      if (perk.Scheme.VisibleConditions == null)
      //          return true;

      //      var isVisible = CheckConditions(perk.Scheme.VisibleConditions, availabilityContext);
      //      return isVisible;
      //  }

      //  private static bool CheckOneCondition(PerkConditionSubScheme condition, PerkAvailabilityContext availabilityContext)
      //  {
      //      if (condition.ParentPerkSid != null)
      //      {
      //          if (availabilityContext.ArchievedPerks == null)
      //              return false;

      //          var parentPerk = availabilityContext.ArchievedPerks.SingleOrDefault(x => x.Scheme.Sid == condition.ParentPerkSid);
      //          if (parentPerk == null)
      //              return false;
      //      }

      //      if (condition.ClassesRequired != null)
      //      {
      //          if (!condition.ClassesRequired.Contains(availabilityContext.PersonScheme.Sid))
      //          {
      //              return false;
      //          }
      //      }

      //      /*
      //       public static bool AllParentsAchieved(this PerkScheme perk, PerkScheme[] achieved)
      //      {
      //          if (perk..Parents == null)
      //              return true;

      //          foreach (var parentSid in perk.Parents)
      //          {
      //              if (achieved.SingleOrDefault(x => x.Sid == parentSid) == null)
      //                  return false;
      //          }

      //          return true;
      //      }

		    //public static bool AvailableForGen(this PerkScheme perk, PersonGeneration gen)
		    //{
			   // return perk.Gens == null || perk.Gens.Contains(gen);
		    //}
      //      */


      //      return true;
      //  }

    }
}
