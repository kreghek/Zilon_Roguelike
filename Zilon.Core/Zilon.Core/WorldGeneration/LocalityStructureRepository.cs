using System.Collections.Generic;

using Zilon.Core.WorldGeneration.LocalityStructures;

namespace Zilon.Core.WorldGeneration
{
    public static class LocalityStructureRepository
    {
        public static ILocalityStructure SettlerCamp
        {
            get
            {
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
                        // Производит 2 единицы, потому что:
                        // 1 для содержания района.
                        // 1 для содержания самого строения.
                    { LocalityResource.Manufacture, 2 },
                    });

                return settlerCamp;
            }
        }

        /// <summary>
        /// Генератор энергии на основе сжигания древесины.
        /// </summary>
        public static ILocalityStructure LumberGenerator
        {
            get
            {
                var structure = new BasicLocalityStructure(name: "Lumber Generator",
                    requiredPopulation: new Dictionary<PopulationSpecializations, int> {
                        { PopulationSpecializations.Workers, 1 },
                        { PopulationSpecializations.Servants, 1 }
                    },
                    requiredResources: new Dictionary<LocalityResource, int> {
                    { LocalityResource.Manufacture, 1 }
                    },
                    productResources: new Dictionary<LocalityResource, int> {
                    { LocalityResource.Energy, 1 }
                    });

                return structure;
            }
        }

        /// <summary>
        /// Свинная ферма.
        /// </summary>
        public static ILocalityStructure PigFarm
        {
            get
            {
                var structure = new BasicLocalityStructure(name: "Pig Farm",
                    requiredPopulation: new Dictionary<PopulationSpecializations, int> {
                        { PopulationSpecializations.Peasants, 1 },
                        { PopulationSpecializations.Servants, 1 }
                    },
                    requiredResources: new Dictionary<LocalityResource, int> {
                        { LocalityResource.Energy, 1 }
                    },
                    productResources: new Dictionary<LocalityResource, int> {
                        { LocalityResource.Food, 1 }
                    });

                return structure;
            }
        }

        /// <summary>
        /// Швейная фабрика.
        /// </summary>
        public static ILocalityStructure GarmentFactory
        {
            get
            {
                var structure = new BasicLocalityStructure(name: "Garment Factory",
                    requiredPopulation: new Dictionary<PopulationSpecializations, int> {
                    { PopulationSpecializations.Workers, 1 },
                    { PopulationSpecializations.Servants, 1 }
                    },
                    requiredResources: new Dictionary<LocalityResource, int> {
                        { LocalityResource.Energy, 1 },
                        { LocalityResource.Manufacture, 1 }
                    },
                    productResources: new Dictionary<LocalityResource, int> {
                    { LocalityResource.Goods, 1 }
                    });

                return structure;
            }
        }

        /// <summary>
        /// Шахта железной руды.
        /// </summary>
        public static ILocalityStructure IronMine
        {
            get
            {
                var structure = new BasicLocalityStructure(name: "Iron Mine",
                    requiredPopulation: new Dictionary<PopulationSpecializations, int> {
                    { PopulationSpecializations.Workers, 1 },
                    { PopulationSpecializations.Servants, 1 }
                    },
                    requiredResources: new Dictionary<LocalityResource, int> {
                        { LocalityResource.Energy, 1 }
                    },
                    productResources: new Dictionary<LocalityResource, int> {
                    { LocalityResource.Manufacture, 2 }
                    });

                return structure;
            }
        }

        /// <summary>
        /// Жилой сектор.
        /// </summary>
        public static ILocalityStructure LivingSector
        {
            get
            {
                var structure = new BasicLocalityStructure(name: "Living Sector",
                    requiredPopulation: new Dictionary<PopulationSpecializations, int>(),
                    requiredResources: new Dictionary<LocalityResource, int> {
                        { LocalityResource.Energy, 1 }
                    },
                    productResources: new Dictionary<LocalityResource, int> {
                    { LocalityResource.LivingPlaces, 5 }
                    });

                return structure;
            }
        }
    }
}
