using Zilon.Core.Diseases;

namespace Assets.Zilon.Scripts.Services
{
    static class DiseaseLocalizationHelper
    {
        public static string GetValueOrDefaultNoname(Language currentLanguage, DiseaseName diseaseName)
        {
            var primaryName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, diseaseName.Primary);
            var prefix = LocalizationHelper.GetValue(currentLanguage, diseaseName.PrimaryPrefix);
            var secondaryName = LocalizationHelper.GetValue(currentLanguage, diseaseName.Secondary);
            var subject = LocalizationHelper.GetValue(currentLanguage, diseaseName.Subject);

            return $"{secondaryName} {prefix}{primaryName} {subject}".Trim();
        }
    }
}
