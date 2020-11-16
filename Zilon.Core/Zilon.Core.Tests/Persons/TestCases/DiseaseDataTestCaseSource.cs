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
                yield return new TestCaseData(arg: Single);
                yield return new TestCaseData(arg: Multiple);
            }
        }

        private static DiseaseSymptom[] Multiple =>
            new[]
            {
                new DiseaseSymptom
                {
                    Rule = DiseaseSymptomType.BreathDownSpeed
                },
                new DiseaseSymptom
                {
                    Rule = DiseaseSymptomType.EnegryDownSpeed
                },
                new DiseaseSymptom
                {
                    Rule = DiseaseSymptomType.HealthLimit
                }
            };

        private static DiseaseSymptom[] Single =>
            new[]
            {
                new DiseaseSymptom
                {
                    Rule = DiseaseSymptomType.BreathDownSpeed
                }
            };
    }
}