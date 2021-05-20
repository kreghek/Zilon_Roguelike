﻿using Zilon.Core.Localization;

namespace Assets.Zilon.Scripts.Services
{
    public static class LocalizationHelper
    {
        public static string GetValue(Language currentLanguage, ILocalizedString localizedString)
        {
            switch (currentLanguage)
            {
                case Language.English:
                default:
                    return localizedString?.En;

                case Language.Russian:
                    return localizedString?.Ru;
            }
        }

        public static string GetValueOrDefaultNoname(Language currentLanguage, ILocalizedString localizedString)
        {
            switch (currentLanguage)
            {
                case Language.English:
                default:
                    return localizedString?.En ?? GetUndefined(currentLanguage);

                case Language.Russian:
                    return localizedString?.Ru ?? GetUndefined(currentLanguage);
            }
        }

        public static string GetUndefined(Language currentLanguage)
        {
            switch (currentLanguage)
            {
                case Language.English:
                default:
                    return "[noname]";

                case Language.Russian:
                    return "[не задано]";
            }
        }
    }
}
