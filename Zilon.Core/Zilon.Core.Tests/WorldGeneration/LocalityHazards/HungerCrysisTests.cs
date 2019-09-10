using System.Collections.Generic;

using FluentAssertions;

using Moq;

using NUnit.Framework;

namespace Zilon.Core.WorldGeneration.LocalityHazards.Tests
{
    [TestFixture()]
    public class HungerCrysisTests
    {
        [Test()]
        public void UpdateTest()
        {
            // ARRANGE

            var locality = new Locality();

            locality.CurrentPopulation.AddRange(new PopulationUnit[] {
                new PopulationUnit{Specialization = PopulationSpecializations.Servants },
                new PopulationUnit{Specialization = PopulationSpecializations.Peasants },
                new PopulationUnit{Specialization = PopulationSpecializations.Workers },
                new PopulationUnit{Specialization = PopulationSpecializations.Servants },
            });

            locality.Stats.ResourcesBalance[LocalityResource.Food] = -1;


            var crysisRandomSourceMock = new Mock<ICrisisRandomSource>();
            crysisRandomSourceMock.Setup(x => x.RollDeathPass()).Returns(1);
            crysisRandomSourceMock.Setup(x => x.RollDeadPopulationIndex(It.IsAny<IEnumerable<PopulationUnit>>())).Returns(0);
            var crysisRandomSource = crysisRandomSourceMock.Object;

            var hunger = new HungerCrisis(crysisRandomSource);



            // ACT
            hunger.Update(locality);



            // ASSERT
            locality.CurrentPopulation.Should().HaveCount(3);
        }
    }
}