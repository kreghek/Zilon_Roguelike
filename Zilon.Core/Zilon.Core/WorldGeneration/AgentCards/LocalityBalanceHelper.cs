using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    public static class LocalityBalanceHelper
    {
        public static Dictionary<LocalityResource, int> CalcBalance(Locality locality)
        {
            // Считаем всё потребление
            var consumptionDict = CreateBalanceDict();
            FillConsumption(locality, consumptionDict);

            // Считаем всё производство
            var productionDict = CreateBalanceDict();
            FillProduction(locality, productionDict);

            var balanceDict = CalcDifference(productionDict, consumptionDict);

            // Учитываем, что излишки промышленности реализуются в деньги
            AddResource(balanceDict, LocalityResource.Money, balanceDict[LocalityResource.Manufacture]);
            balanceDict[LocalityResource.Manufacture] = 0;

            return balanceDict;
        }

        private static Dictionary<LocalityResource, int> CalcDifference(Dictionary<LocalityResource, int> productionDict,
            Dictionary<LocalityResource, int> consumptionDict)
        {
            var diffDict = CreateBalanceDict();
            foreach (var resource in productionDict)
            {
                diffDict[resource.Key] = resource.Value - consumptionDict[resource.Key];
            }

            return diffDict;
        }

        private static Dictionary<LocalityResource, int> CreateBalanceDict()
        {
            var resourceDict = new Dictionary<LocalityResource, int>();

            FillResources(resourceDict);

            return resourceDict;
        }

        /// <summary>
        /// Этот метод выполняем для того, чтобы словарь всегда содержал все ресурсы.
        /// Чтобы не нужно было каждый раз проверять наличие ключа.
        /// </summary>
        private static void FillResources(Dictionary<LocalityResource, int> resourceDict)
        {
            var allAvailableResources = Enum.GetValues(typeof(LocalityResource))
                            .Cast<LocalityResource>()
                            .Where(x => x != LocalityResource.Undefined);
            foreach (var resource in allAvailableResources)
            {
                resourceDict[resource] = 0;
            }
        }

        private static void FillConsumption(Locality locality, Dictionary<LocalityResource, int> dict)
        {
            // Население потребляем жилые места, еду и товары
            var populationCount = locality.CurrentPopulation.Count();
            AddResource(dict, LocalityResource.LivingPlaces, populationCount);
            AddResource(dict, LocalityResource.Goods, populationCount);
            AddResource(dict, LocalityResource.Food, populationCount);

            foreach (var region in locality.Regions)
            {
                AddResource(dict, LocalityResource.Money, region.MaintenanceCost);

                foreach (var structure in region.Structures)
                {
                    AddResource(dict, LocalityResource.Money, structure.MaintenanceCost);

                    foreach (var requiredResource in structure.RequiredResources)
                    {
                        AddResource(dict, requiredResource.Key, requiredResource.Value);
                    }
                }
            }
        }

        private static void FillProduction(Locality locality, Dictionary<LocalityResource, int> dict)
        {
            foreach (var region in locality.Regions)
            {
                foreach (var structure in region.Structures)
                {
                    foreach (var requiredResource in structure.ProductResources)
                    {
                        AddResource(dict, requiredResource.Key, requiredResource.Value);
                    }
                }
            }
        }

        private static void AddResource(Dictionary<LocalityResource, int> dict, LocalityResource resource, int value)
        {
            if (!dict.ContainsKey(resource))
            {
                dict[resource] = 0;
            }

            dict[resource] += value;
        }
    }
}
