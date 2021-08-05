using System;
using System.Diagnostics;
using System.Threading;

using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels
{
    public static class PerkHelper
    {
        public static string GetPropTitle(IPerk perk)
        {
            var text = perk.Scheme.Name?.En;

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;
            if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                text = perk.Scheme.Name?.En;
            }
            else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                text = perk.Scheme.Name?.Ru;
            }
            else
            {
                Debug.Fail(
                    $"Unknown language {langName} is selected. All available language must be supported in the client.");
            }

            return text ?? "<Undef>";
        }

        public static string? GetPerkDescription(IPerk perk)
        {
            var text = perk.Scheme.Name?.En;

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;
            if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                text = perk.Scheme.Description?.En;
            }
            else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                text = perk.Scheme.Description?.Ru;
            }
            else
            {
                Debug.Fail(
                    $"Unknown language {langName} is selected. All available language must be supported in the client.");
            }

            return text;
        }

        public static string GetPerkHintText(IPerk perk)
        {
            var title = GetPropTitle(perk);
            var description = GetPerkDescription(perk);

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