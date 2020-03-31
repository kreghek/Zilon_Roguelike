using Zilon.Core.Localization;

namespace Assets.Zilon.Scripts.Services
{
    static class LocalizationHelper
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
                    return localizedString?.En ?? "[noname]";

                case Language.Russian:
                    return localizedString?.Ru ?? "[не задано]";
            }
        }
    }
}
