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

            locality.CurrentPopulation.AddRange(new Population[] {
                new Population{Specialization = PopulationSpecializations.Servants },
                new Population{Specialization = PopulationSpecializations.Peasants },
                new Population{Specialization = PopulationSpecializations.Workers },
                new Population{Specialization = PopulationSpecializations.Servants },
            });

            locality.Stats.Resources[LocalityResource.Food] = -1;


            var crysisRandomSourceMock = new Mock<ICrysisRandomSource>();
            crysisRandomSourceMock.Setup(x => x.RollDeathPass()).Returns(1);
            crysisRandomSourceMock.Setup(x => x.RollDeadPopulationIndex(It.IsAny<IEnumerable<Population>>())).Returns(0);
            var crysisRandomSource = crysisRandomSourceMock.Object;

            var hunger = new HungerCrysis(crysisRandomSource);



            // ACT
            hunger.Update(locality);



            // ASSERT
            locality.CurrentPopulation.Should().HaveCount(3);
        }
    }
}