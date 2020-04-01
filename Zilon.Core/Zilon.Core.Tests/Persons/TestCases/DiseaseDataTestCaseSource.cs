using System.Collections;

using NUnit.Framework;

using Zilon.Core.Diseases;

namespace Zilon.Core.Tests.Persons.TestCases
{
    public static class DiseaseDataTestCaseSource
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData((object)Single);
                yield return new TestCaseData((object)Multiple);
            }
        }

        private static DiseaseSymptom[] Single
        {
            get => new[] {
                new DiseaseSymptom { Rule = DiseaseSymptomType.BreathDownSpeed }
            };
        }

        private static DiseaseSymptom[] Multiple
        {
            get => new[] {
                new DiseaseSymptom { Rule = DiseaseSymptomType.BreathDownSpeed },
                new DiseaseSymptom { Rule = DiseaseSymptomType.EnegryDownSpeed },
                new DiseaseSymptom { Rule = DiseaseSymptomType.HealthLimit },
            };
        }
    }
}
