using NUnit.Framework;
using Zilon.Core.WorldGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.WorldGeneration.LocalityStructures;
using FluentAssertions;

namespace Zilon.Core.WorldGeneration.Tests
{
    [TestFixture]
    public class LocalityTests
    {
        /// <summary>
        /// Тест проверяет, что если в городе есть структура, которая обеспечивает себя и генерирует производство,
        /// то после одного обновления в городе будет единица производства, а остальные ресурсы не изменятся.
        /// </summary>
        [Test]
        public void Update_ProduceOnlyManufactureInOneIteration_ManufactureIncreaseAndOtherResourcesNotChanges()
        {
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
                    { LocalityResource.Manufacture, 2 },
                });

            var region = new LocalityRegion();
            region.Structures.Add(settlerCamp);

            locality.Regions.Add(region);

            locality.CurrentPopulation.AddRange(new PopulationUnit[] {
                new PopulationUnit{Specialization = PopulationSpecializations.Peasants },
                new PopulationUnit{Specialization = PopulationSpecializations.Workers },
                new PopulationUnit{Specialization = PopulationSpecializations.Servants },
            });

            locality.Stats.ResourcesBalance[LocalityResource.Energy] = 1;
            locality.Stats.ResourcesBalance[LocalityResource.Food] = 3;
            locality.Stats.ResourcesBalance[LocalityResource.Goods] = 3;
            locality.Stats.ResourcesBalance[LocalityResource.LivingPlaces] = 3;
            locality.Stats.ResourcesBalance[LocalityResource.Money] = 2;



            // ACT
            locality.Update();



            // ASSERT
            locality.Stats.ResourcesBalance[LocalityResource.Energy].Should().Be(1);
            locality.Stats.ResourcesBalance[LocalityResource.Food].Should().Be(3);
            locality.Stats.ResourcesBalance[LocalityResource.Goods].Should().Be(3);
            locality.Stats.ResourcesBalance[LocalityResource.LivingPlaces].Should().Be(3);

            locality.Stats.ResourcesBalance[LocalityResource.Manufacture].Should().Be(0);
            locality.Stats.ResourcesBalance[LocalityResource.Money].Should().Be(2);
        }

        /// <summary>
        /// Тест проверяет, что если в городе есть структура, которая обеспечивает себя и генерирует производство,
        /// то после одного обновления в городе будет единица производства, а остальные ресурсы не изменятся.
        /// Все в течении двух итераций.
        /// </summary>
        [Test]
        public void Update_ProduceOnlyManufactureIn2Iteration_ManufactureIncreaseAndOtherResourcesNotChanges()
        {
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
                    { LocalityResource.Manufacture, 2 },
                });

            var region = new LocalityRegion();
            region.Structures.Add(settlerCamp);

            locality.Regions.Add(region);

            locality.CurrentPopulation.AddRange(new PopulationUnit[] {
                new PopulationUnit{Specialization = PopulationSpecializations.Peasants },
                new PopulationUnit{Specialization = PopulationSpecializations.Workers },
                new PopulationUnit{Specialization = PopulationSpecializations.Servants },
            });

            locality.Stats.ResourcesBalance[LocalityResource.Energy] = 1;
            locality.Stats.ResourcesBalance[LocalityResource.Food] = 3;
            locality.Stats.ResourcesBalance[LocalityResource.Goods] = 3;
            locality.Stats.ResourcesBalance[LocalityResource.LivingPlaces] = 3;
            locality.Stats.ResourcesBalance[LocalityResource.Money] = 2;



            // ACT
            locality.Update();
            locality.Update();



            // ASSERT
            locality.Stats.ResourcesBalance[LocalityResource.Energy].Should().Be(1);
            locality.Stats.ResourcesBalance[LocalityResource.Food].Should().Be(3);
            locality.Stats.ResourcesBalance[LocalityResource.Goods].Should().Be(3);
            locality.Stats.ResourcesBalance[LocalityResource.LivingPlaces].Should().Be(3);

            locality.Stats.ResourcesBalance[LocalityResource.Manufacture].Should().Be(0);
            locality.Stats.ResourcesBalance[LocalityResource.Money].Should().Be(2);
        }
    }
}