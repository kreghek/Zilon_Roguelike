using Zilon.Core.Schemes;

namespace Assets.Zilon.Scripts.Services
{
    static class LocalizationHelper
    {
        public static string GetValue(Language currentLanguage, LocalizedStringSubScheme localizedString)
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
