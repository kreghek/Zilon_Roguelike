using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels
{
    public static class MonsterHelper
    {
        public static string GetMonsterName(MonsterPerson monster)
        {
            var text = monster.Scheme.Name?.En;

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;
            if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                text = monster.Scheme.Name?.En;
            }
            else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                text = monster.Scheme.Name?.Ru;
            }
            else
            {
                Debug.Fail(
                    $"Unknown language {langName} is selected. All available language must be supported in the client.");
            }

            return text ?? "<Undef>";
        }

        public static string? GetMonsterDescription(MonsterPerson monster)
        {
            var text = monster.Scheme.Name?.En;

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;
            if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                text = monster.Scheme.Description?.En;
            }
            else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                text = monster.Scheme.Description?.Ru;
            }
            else
            {
                Debug.Fail(
                    $"Unknown language {langName} is selected. All available language must be supported in the client.");
            }

            return text;
        }

        public static string GetPerkHintText(MonsterPerson monster)
        {
            var name = GetMonsterName(monster);
            var description = GetMonsterDescription(monster);

            var survivalModule = monster.GetModule<ISurvivalModule>();
            var hpStat = survivalModule.Stats.Single(x => x.Type == SurvivalStatType.Health);
            var healthState = GetHealthState(hpStat.ValueShare);
            if (!string.IsNullOrWhiteSpace(description))
            {
                return $"{name}\n{new string('-', 8)}\n{healthState}\n{new string('-', 8)}\n{description}";
            }
            else
            {
                return $"{name}\n{new string('-', 8)}\n{healthState}";
            }
        }

        private static string GetHealthState(float valueShare)
        {
            if (valueShare >= 0.9f)
            {
                var currentLanguage = Thread.CurrentThread.CurrentUICulture;
                var langName = currentLanguage.TwoLetterISOLanguageName;
                if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "Healthy";
                }
                else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "Здоров";
                }
            }
            else if (valueShare < 0.9f && valueShare > 0.5f)
            {
                var currentLanguage = Thread.CurrentThread.CurrentUICulture;
                var langName = currentLanguage.TwoLetterISOLanguageName;
                if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "Slightly injured";
                }
                else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "Легко ранен";
                }
            }
            else if (valueShare < 0.5f && valueShare > 0.1f)
            {
                var currentLanguage = Thread.CurrentThread.CurrentUICulture;
                var langName = currentLanguage.TwoLetterISOLanguageName;
                if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "Injured";
                }
                else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "Ранен";
                }
            }
            else if (valueShare < 0.1f)
            {
                var currentLanguage = Thread.CurrentThread.CurrentUICulture;
                var langName = currentLanguage.TwoLetterISOLanguageName;
                if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "Near death";
                }
                else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "При смерти";
                }
            }
            else
            {
                Debug.Fail("Unknown state");

                return "<Unknow>";
            }

            Debug.Fail("Unknown state");
            return "<Unknow>";
        }
    }
}