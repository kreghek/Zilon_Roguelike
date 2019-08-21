using NUnit.Framework;
using Zilon.Core.WorldGeneration.LocalityHazards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.WorldGeneration.LocalityStructures;
using Moq;

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

            var settlerCamp = new BasicLocalityStructure(name: "Settler Camp",
                requiredPopulation: new Dictionary<PopulationSpecializations, int> {
                    { PopulationSpecializations.Workers, 1 },
                    { PopulationSpecializations.Peasants, 1 },
                    { PopulationSpecializations.Servants, 1 }
                },
                requiredResources: new Dictionary<LocalityResource, int> {
                    { LocalityResource.Energy, 1 }
                },
                productResources: new Dictionary<LocalityResource, int> {
                    { LocalityResource.Energy, 1 },
                    { LocalityResource.Food, 3 },
                    { LocalityResource.Goods, 3 },
                    { LocalityResource.LivingPlaces, 3 },
                    { LocalityResource.Manufacture, 1 },
                });

            var region = new LocalityRegion();
            region.Structures.Add(settlerCamp);

            locality.Regions.Add(region);

            locality.CurrentPopulation.AddRange(new Population[] {
                new Population{Specialization = PopulationSpecializations.Servants },
                new Population{Specialization = PopulationSpecializations.Peasants },
                new Population{Specialization = PopulationSpecializations.Workers },
                new Population{Specialization = PopulationSpecializations.Servants },
            });

            locality.Stats.Resources[LocalityResource.Energy] = 1;
            locality.Stats.Resources[LocalityResource.Food] = 3;
            locality.Stats.Resources[LocalityResource.Goods] = 3;
            locality.Stats.Resources[LocalityResource.LivingPlaces] = 3;


            var crysisRandomSourceMock = new Mock<ICrysisRandomSource>();
            var 
            var crysisRandomSource = crysisRandomSourceMock.Object;

            var hunder = new HungerCrysis()
        }

        
    }
}