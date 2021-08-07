using System;
using System.Diagnostics;
using System.Threading;

using Zilon.Core.Persons;
using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels
{
    public static class CombatActHelper
    {
        public static string GetActTitle(ICombatAct combatAct)
        {
            var text = combatAct.Scheme.Name?.En;

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;
            if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                text = combatAct.Scheme.Name?.En;
            }
            else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                text = combatAct.Scheme.Name?.Ru;
            }
            else
            {
                Debug.Fail(
                    $"Unknown language {langName} is selected. All available language must be supported in the client.");
            }

            return text ?? "<Undef>";
        }

        public static string? GetActDescription(ICombatAct combatAct)
        {
            var text = combatAct.Scheme.Name?.En;

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;
            if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                text = combatAct.Scheme.Description?.En;
            }
            else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                text = combatAct.Scheme.Description?.Ru;
            }
            else
            {
                Debug.Fail(
                    $"Unknown language {langName} is selected. All available language must be supported in the client.");
            }

            return text;
        }

        public static string GetActHintText(ICombatAct combatAct)
        {
            var title = GetActTitle(combatAct);
            var description = GetActDescription(combatAct);

            if (!string.IsNullOrWhiteSpace(description))
            {
                return $"{title}\n{new string('-', 8)}\n{description}";
            }
            else
            {
                return title;
            }
        }
    }
}