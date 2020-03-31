using System;
using System.Collections.Generic;
using System.Text;
using Zilon.Core.Localization;
using Zilon.Core.Persons;

namespace Zilon.Core.MapGenerators
{
    public static class DiseaseSymptoms
    {
        public static DiseaseSymptom[] Symptoms { 
            get => new[] { 
                new DiseaseSymptom
                {
                    Name = new LocalizedString{ En = "Sore Throat", Ru = "Воспаление горла" },
                    Rule = DiseaseSymptomType.BreathDownSpeed
                },
                new DiseaseSymptom
                {
                    Name = new LocalizedString{ En = "Sputum in the lungs", Ru = "Слизь в легких" },
                    Rule = DiseaseSymptomType.BreathDownSpeed
                },
                new DiseaseSymptom
                {
                    Name = new LocalizedString{ En = "Body aches", Ru = "Ломота в теле" },
                    Rule = DiseaseSymptomType.HealthLimit
                },
                new DiseaseSymptom
                {
                    Name = new LocalizedString{ En = "Poor digestibility", Ru = "Плохая усвояемость" },
                    Rule = DiseaseSymptomType.HungerSpeed
                },
                new DiseaseSymptom
                {
                    Name = new LocalizedString{ En = "Fast fatigue", Ru = "Быстрая усталость" },
                    Rule = DiseaseSymptomType.EnegryDownSpeed
                },
            };
        }
    }

    public class DiseaseSymptom
    {
        public ILocalizedString Name { get; set; }

        public DiseaseSymptomType Rule { get; set; }
    }

    public enum DiseaseSymptomType
    {
        Undefined,

        BreathDownSpeed,

        EnegryDownSpeed,

        HungerSpeed,

        ThirstSpeed,

        HealthLimit
    }
}
