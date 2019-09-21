using System.IO;
using FluentAssertions;

using Newtonsoft.Json;

using NUnit.Framework;

using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Schemes
{
    [TestFixture]
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
            factPersonScheme.SurvivalStats[0].MaxValue.Should().Be(150);
            factPersonScheme.SurvivalStats[0].StartValue.Should().Be(50);

            var expectedKeyPoints = new[] {
                new TestPersonSurvivalStatKeySegmentSubScheme{
                    Level = PersonSurvivalStatKeypointLevel.Lesser,
                    Start = -60
                },
                new TestPersonSurvivalStatKeySegmentSubScheme{
                    Level = PersonSurvivalStatKeypointLevel.Strong,
                    Start = -360
                },
                new TestPersonSurvivalStatKeySegmentSubScheme{
                    Level = PersonSurvivalStatKeypointLevel.Max,
                    Start = -2520
                },
            };
            factPersonScheme.SurvivalStats[0].KeyPoints.Should().BeEquivalentTo(expectedKeyPoints);
        }
    }
}
