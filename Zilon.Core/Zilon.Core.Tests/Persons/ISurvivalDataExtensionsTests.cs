using System.Linq;

using FluentAssertions;

using NUnit.Framework;

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
        public void CalcKeyPointsInRange_TestCases(int lesserKeyPoint, int strongKeyPoint, int maxKeyPoint,
            int minValue, int maxValue,
            SurvivalStatHazardLevel passedKeyPoint0,
            SurvivalStatHazardLevel passedKeyPoint1,
            SurvivalStatHazardLevel passedKeyPoint2)
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
            var expectedKeyPoints = expectedKeyPointsWithNulls.Where(x => x != SurvivalStatHazardLevel.Undefined).ToArray();

            var factKeyPoints = crossedKeyPoints.Select(x => x.Level).ToArray();

            factKeyPoints.Should().AllBeEquivalentTo(expectedKeyPoints);
        }

        /// <summary>
        /// Тест проверяет, что при изменении значения в указанном диапазоне
        /// мы проходим через все предполагаемые ключевые точки.
        /// </summary>
        [Test]
        public void CalcKeyPointsInRange_2PositiveKeyPointsRise_RaisedStrongKeyPoint()
        {
            // ARRANGE

            var keyPoints = new[] {
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, 25),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, 50),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, 100)
            };



            // ACT
            var crossedKeyPoints = keyPoints.CalcKeyPointsInRange(0, 51);



            // ASSERT
            var materializedFactCrossedKeyPoints = crossedKeyPoints.ToArray();
            materializedFactCrossedKeyPoints[0].Level.Should().Be(SurvivalStatHazardLevel.Lesser);
            materializedFactCrossedKeyPoints[1].Level.Should().Be(SurvivalStatHazardLevel.Strong);
        }

        /// <summary>
        /// Тест проверяет, что при изменении значения в указанном диапазоне
        /// мы проходим через все предполагаемые ключевые точки.
        /// </summary>
        [Test]
        public void CalcKeyPointsInRange_2PositiveKeyPointsRise_RaisedStrongKeyPoint2()
        {
            // ARRANGE

            var keyPoints = new[] {
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, 25),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, 50),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, 100)
            };



            // ACT
            var crossedKeyPoints = keyPoints.CalcKeyPointsInRange(100, 99);



            // ASSERT
            var materializedFactCrossedKeyPoints = crossedKeyPoints.ToArray();
            materializedFactCrossedKeyPoints[0].Level.Should().Be(SurvivalStatHazardLevel.Max);
        }

        /// <summary>
        /// Тест проверяет, что при изменении значения в указанном диапазоне
        /// мы проходим через все предполагаемые ключевые точки.
        /// </summary>
        [Test]
        public void CalcKeyPointsInRange_2PositiveKeyPointsRise_RaisedStrongKeyPoint3()
        {
            // ARRANGE

            var keyPoints = new[] {
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, 1),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, 2),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, 3)
            };



            // ACT
            var crossedKeyPoints = keyPoints.CalcKeyPointsInRange(0, 3);



            // ASSERT
            var materializedFactCrossedKeyPoints = crossedKeyPoints.ToArray();
            materializedFactCrossedKeyPoints[0].Level.Should().Be(SurvivalStatHazardLevel.Lesser);
            materializedFactCrossedKeyPoints[1].Level.Should().Be(SurvivalStatHazardLevel.Strong);
            materializedFactCrossedKeyPoints[2].Level.Should().Be(SurvivalStatHazardLevel.Max);
        }

        /// <summary>
        /// Тест проверяет, что при изменении значения в указанном диапазоне
        /// мы проходим через все предполагаемые ключевые точки.
        /// </summary>
        [Test]
        public void CalcKeyPointsInRange_2NegativeKeyPointsRise_RaisedStrongKeyPoint()
        {
            // ARRANGE

            var keyPoints = new[] {
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, -25),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, -50),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, -100)
            };



            // ACT
            var crossedKeyPoints = keyPoints.CalcKeyPointsInRange(0, -51);



            // ASSERT
            var materializedFactCrossedKeyPoints = crossedKeyPoints.ToArray();
            materializedFactCrossedKeyPoints[0].Level.Should().Be(SurvivalStatHazardLevel.Lesser);
            materializedFactCrossedKeyPoints[1].Level.Should().Be(SurvivalStatHazardLevel.Strong);
        }
    }
}