using Zilon.Core.Localization;

namespace Zilon.Core.Diseases
{
    /// <summary>
    /// Общее описание симптома.
    /// </summary>
    public class DiseaseSymptom
    {
        public ILocalizedString Name { get; set; }

        public DiseaseSymptomType Rule { get; set; }
    }
}