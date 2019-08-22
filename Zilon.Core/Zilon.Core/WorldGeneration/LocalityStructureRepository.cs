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
    }
}
