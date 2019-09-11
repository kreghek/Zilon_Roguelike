using NUnit.Framework;
using Zilon.Core.WorldGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.WorldGeneration.LocalityStructures;
using FluentAssertions;
using Zilon.Core.WorldGeneration.LocalityHazards;
using Zilon.Core.CommonServices.Dices;

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

            locality.Stats.ResourcesLastIteration[LocalityResource.Energy] = 1;
            locality.Stats.ResourcesLastIteration[LocalityResource.Food] = 3;
            locality.Stats.ResourcesLastIteration[LocalityResource.Goods] = 3;
            locality.Stats.ResourcesLastIteration[LocalityResource.LivingPlaces] = 3;
            locality.Stats.ResourcesLastIteration[LocalityResource.Money] = 2;



            // ACT
            locality.Update();



            // ASSERT
            locality.Stats.ResourcesLastIteration[LocalityResource.Energy].Should().Be(1);
            locality.Stats.ResourcesLastIteration[LocalityResource.Food].Should().Be(3);
            locality.Stats.ResourcesLastIteration[LocalityResource.Goods].Should().Be(3);
            locality.Stats.ResourcesLastIteration[LocalityResource.LivingPlaces].Should().Be(3);

            locality.Stats.ResourcesLastIteration[LocalityResource.Manufacture].Should().Be(0);
            locality.Stats.ResourcesLastIteration[LocalityResource.Money].Should().Be(2);
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

            locality.Stats.ResourcesLastIteration[LocalityResource.Energy] = 1;
            locality.Stats.ResourcesLastIteration[LocalityResource.Food] = 3;
            locality.Stats.ResourcesLastIteration[LocalityResource.Goods] = 3;
            locality.Stats.ResourcesLastIteration[LocalityResource.LivingPlaces] = 3;
            locality.Stats.ResourcesLastIteration[LocalityResource.Money] = 2;



            // ACT
            locality.Update();
            locality.Update();



            // ASSERT
            locality.Stats.ResourcesLastIteration[LocalityResource.Energy].Should().Be(1);
            locality.Stats.ResourcesLastIteration[LocalityResource.Food].Should().Be(3);
            locality.Stats.ResourcesLastIteration[LocalityResource.Goods].Should().Be(3);
            locality.Stats.ResourcesLastIteration[LocalityResource.LivingPlaces].Should().Be(3);

            locality.Stats.ResourcesLastIteration[LocalityResource.Manufacture].Should().Be(0);
            locality.Stats.ResourcesLastIteration[LocalityResource.Money].Should().Be(2);
        }

        /// <summary>
        /// Тест для разработки. В конце работы над моделью городов - занулить.
        /// </summary>
        [Test()]
        public void UpdateTest()
        {
            var _crysisRandomSource = new CrysisRandomSource(new Dice(1));

            var locality = new Locality();

            var settlerCamp = LocalityStructureRepository.SettlerCamp;

            var region = new LocalityRegion();
            region.Structures.Add(settlerCamp);

            locality.Regions.Add(region);

            locality.CurrentPopulation.AddRange(new PopulationUnit[] {
                new PopulationUnit{Specialization = PopulationSpecializations.Peasants },
                new PopulationUnit{Specialization = PopulationSpecializations.Workers },
                new PopulationUnit{Specialization = PopulationSpecializations.Servants },
            });

            foreach (var populationUnit in locality.CurrentPopulation)
            {
                populationUnit.Age = 25;
                populationUnit.Assigments.Add(settlerCamp);
            }

            locality.Stats.ResourcesLastIteration[LocalityResource.Energy] = 1;
            locality.Stats.ResourcesLastIteration[LocalityResource.Food] = 3;
            locality.Stats.ResourcesLastIteration[LocalityResource.Goods] = 3;
            locality.Stats.ResourcesLastIteration[LocalityResource.LivingPlaces] = 3;
            locality.Stats.ResourcesLastIteration[LocalityResource.Money] = 2;

            for (var i = 0; i < 100; i++)
            {
                locality.Update();

                var crysisMonitors = new ICrisisMonitor[]
                {
                    new HungerMonitor(_crysisRandomSource),
                    new PopulationGrowthMonitor(_crysisRandomSource)
                };

                CrisisMonitoring(locality, crysisMonitors);

                UpdateCrises(locality);
            }
        }

        private static void UpdateCrises(Locality locality)
        {
            foreach (var crisis in locality.Crises)
            {
                crisis.Update(locality);
            }
        }

        private static void CrisisMonitoring(Locality locality, ICrisisMonitor[] crysisMonitors)
        {
            var currentCrisesTypes = locality.Crises.Select(x => x.GetType());

            foreach (var monitor in crysisMonitors)
            {
                if (currentCrisesTypes.Contains(monitor.CrysisType))
                {
                    // Один и тот же кризис у города не может наступить дважды.
                    continue;
                }

                var crysis = monitor.Analyze(locality);
                if (crysis != null)
                {
                    locality.Crises.Add(crysis);
                }
            }
        }


    }
}