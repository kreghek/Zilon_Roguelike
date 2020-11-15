using System.IO;

using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Schemes
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PersonSchemeTests
    {
        /// <summary>
        /// Тест проверяет, что работы корректно десериализуется.
        /// </summary>
        [Test]
        public void Deserialization_SurvivalStats_ValueRangeAndKeyPointsAreCorrect()
        {
            // ARRANGE
            var assembly = GetType().Assembly;
            var resourceName = "Zilon.Core.Tests.Schemes.human-person.json";

            string sourceText;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                sourceText = reader.ReadToEnd();
            }

            // ACT
            var factPersonScheme = JsonConvert.DeserializeObject<PersonScheme>(sourceText);

            // ASSERT

            // Значение взяты из схемы, которая приложена, как ресурс.
            // Может не совпадать с фактическими значениями в боевой схеме.
            factPersonScheme.SurvivalStats[0].Type.Should().Be(PersonSurvivalStatType.Satiety);
            factPersonScheme.SurvivalStats[0].MinValue.Should().Be(-3000);
            factPersonScheme.SurvivalStats[0].MaxValue.Should().Be(500);
            factPersonScheme.SurvivalStats[0].StartValue.Should().Be(250);

            var expectedKeySegments = new[]
            {
                new TestPersonSurvivalStatKeySegmentSubScheme
                {
                    Level = PersonSurvivalStatKeypointLevel.Max, Start = 0, End = 0.14f
                },
                new TestPersonSurvivalStatKeySegmentSubScheme
                {
                    Level = PersonSurvivalStatKeypointLevel.Strong, Start = 0.14f, End = 0.75f
                },
                new TestPersonSurvivalStatKeySegmentSubScheme
                {
                    Level = PersonSurvivalStatKeypointLevel.Lesser, Start = 0.75f, End = 0.86f
                }
            };
            factPersonScheme.SurvivalStats[0].KeyPoints.Should().BeEquivalentTo(expectedKeySegments);
        }
    }
}