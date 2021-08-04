using System;
using System.Diagnostics;
using System.Threading;

using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels
{
    public static class PropHelper
    {
        public static string GetPropTitle(IProp prop)
        {
            var text = prop.Scheme.Name?.En;

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;
            if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                text = prop.Scheme.Name?.En;
            }
            else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                text = prop.Scheme.Name?.Ru;
            }
            else
            {
                Debug.Fail(
                    $"Unknown language {langName} is selected. All available language must be supported in the client.");
            }

            return text ?? "<Undef>";
        }

        public static string? GetPropDescription(IProp prop)
        {
            var text = prop.Scheme.Name?.En;

            var currentLanguage = Thread.CurrentThread.CurrentUICulture;
            var langName = currentLanguage.TwoLetterISOLanguageName;
            if (string.Equals(langName, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                text = prop.Scheme.Description?.En;
            }
            else if (string.Equals(langName, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                text = prop.Scheme.Description?.Ru;
            }
            else
            {
                Debug.Fail(
                    $"Unknown language {langName} is selected. All available language must be supported in the client.");
            }

            return text;
        }

        public static string GetPropHintText(IProp prop)
        {
            var title = GetPropTitle(prop);
            var description = GetPropDescription(prop);

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