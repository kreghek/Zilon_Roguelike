using Zilon.Core.Localization;

namespace Zilon.Core.Diseases
{
    /// <summary>
    /// Каталог симптомов.
    /// </summary>
    public static class DiseaseSymptoms
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Справочные данные.")]
        public static DiseaseSymptom[] Symptoms =>
            new[]
            {
                new DiseaseSymptom
                {
                    Name = new LocalizedString
                    {
                        En = "Sore Throat", Ru = "Воспаление горла"
                    },
                    Rule = DiseaseSymptomType.BreathDownSpeed
                },
                new DiseaseSymptom
                {
                    Name = new LocalizedString
                    {
                        En = "Sputum in the lungs", Ru = "Слизь в легких"
                    },
                    Rule = DiseaseSymptomType.BreathDownSpeed
                },
                new DiseaseSymptom
                {
                    Name = new LocalizedString
                    {
                        En = "Dyspnea", Ru = "Одышка"
                    },
                    Rule = DiseaseSymptomType.BreathDownSpeed
                },
                new DiseaseSymptom
                {
                    Name = new LocalizedString
                    {
                        En = "Body aches", Ru = "Ломота в теле"
                    },
                    Rule = DiseaseSymptomType.HealthLimit
                },
                new DiseaseSymptom
                {
                    Name = new LocalizedString
                    {
                        En = "Poor digestibility", Ru = "Плохая усвояемость"
                    },
                    Rule = DiseaseSymptomType.HungerSpeed
                },
                new DiseaseSymptom
                {
                    Name = new LocalizedString
                    {
                        En = "Fast fatigue", Ru = "Быстрая усталость"
                    },
                    Rule = DiseaseSymptomType.EnegryDownSpeed
                }
            };
    }
}