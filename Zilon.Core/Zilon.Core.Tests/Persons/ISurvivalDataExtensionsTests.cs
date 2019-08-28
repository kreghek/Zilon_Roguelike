using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Tests.Persons.TestCases;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture]
    public class ISurvivalDataExtensionsTests
    {
        /// <summary>
        /// Тест проверяет, что при изменении значения в указанном диапазоне
        /// мы проходим через все предполагаемые ключевые точки.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(ISurvivalDataExtensionsTestCaseSource), nameof(ISurvivalDataExtensionsTestCaseSource.CalcKeyPointTestCases))]
        public void CalcKeyPointsInRange_TestCases(int lesserKeyPoint, int strongKeyPoint, int maxKeyPoint,
            int minValue, int maxValue,
            SurvivalStatHazardLevel? passedKeyPoint0,
            SurvivalStatHazardLevel? passedKeyPoint1,
            SurvivalStatHazardLevel? passedKeyPoint2)
        {
            // ARRANGE

            var keyPoints = new[] {
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, lesserKeyPoint),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, strongKeyPoint),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, maxKeyPoint)
            };



            // ACT

            var crossedKeyPoints = keyPoints.CalcKeyPointsInRange(minValue, maxValue);



            // ASSERT
            var expectedKeyPointsWithNulls = new[] { passedKeyPoint0, passedKeyPoint1, passedKeyPoint2 };
            var expectedKeyPoints = expectedKeyPointsWithNulls.Where(x => x != null).Select(x => x.Value).ToArray();

            var factKeyPoints = crossedKeyPoints.Select(x => x.Level).ToArray();

            factKeyPoints.Should().BeEquivalentTo(expectedKeyPoints);
        }
    }
}