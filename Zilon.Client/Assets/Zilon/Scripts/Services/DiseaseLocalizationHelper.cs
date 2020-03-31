using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Assets.Zilon.Scripts.Services
{
    static class DiseaseLocalizationHelper
    {
        public static string GetValueOrDefaultNoname(Language currentLanguage, DiseaseName diseaseName)
        {
            var primaryName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, diseaseName.Primary);
            var prefix = LocalizationHelper.GetValue(currentLanguage, diseaseName.PrimaryPrefix);
            var secondaryName = LocalizationHelper.GetValue(currentLanguage, diseaseName.Secondary);
            var subject = LocalizationHelper.GetValue(currentLanguage, );

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
