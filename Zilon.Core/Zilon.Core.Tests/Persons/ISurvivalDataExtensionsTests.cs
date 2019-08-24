using NUnit.Framework;
using Zilon.Core.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture()]
    public class ISurvivalDataExtensionsTests
    {
        [Test()]
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

        [Test()]
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