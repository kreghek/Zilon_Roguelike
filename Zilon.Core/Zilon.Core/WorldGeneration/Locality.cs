using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.WorldGeneration.LocalityHazards;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Город.
    /// </summary>
    public class Locality
    {
        private const float STORAGE_RESOURCE_RESERVE = 0.3f;

        public Locality()
        {
            Regions = new List<LocalityRegion>();
            Stats = new LocalityStats();
            CurrentPopulation = new List<PopulationUnit>();
            Crises = new List<ICrisis>();
        }

        public string Name { get; set; }

        public TerrainCell Cell { get; set; }

        public Realm Owner { get; set; }

        /// <summary>
        /// Текущее население города.
        /// Каждый объект в списке - это единица населения.
        /// Суммарно единиц популяции не должно быть больше, чем места для проживания.
        /// Иначе начнётся перенаселение. Тогда жители могут организовать миграционнуб группу и покинуть город.
        /// </summary>
        public List<PopulationUnit> CurrentPopulation { get; }

        public Dictionary<BranchType, int> Branches { get; set; }

        /// <summary>
        /// Текущие районы города.
        /// Каждый район занимает один узел в провинции.
        /// Каждый район сначала должен быть разработан.
        /// После разработки в районе можно возводить структуры.
        /// </summary>
        public List<LocalityRegion> Regions { get; }

        /// <summary>
        /// Текущее состояние в городе. Харатектиристики города.
        /// </summary>
        public LocalityStats Stats { get; private set; }

        /// <summary>
        /// Текущие кризисы в городе.
        /// </summary>
        public List<ICrisis> Crises { get; }

        /// <summary>
        /// Текущий управляющий в городе.
        /// </summary>
        public Agent Head { get; set; }

        public override string ToString()
        {
            return $"{Name} [{Owner}] ({Branches?.FirstOrDefault().Key})";
        }

        /// <summary>
        /// Обновление состояния города.
        /// </summary>
        public void Update()
        {
            UpdatePopulation();
            UpdateRegions();
        }

        /// <summary>
        /// Информация о потреблении ресурса.
        /// </summary>
        private class Comsumption
        {
            /// <summary>
            /// Объем потребления ресурса текущей структурой или районом.
            /// </summary>
            public float Amount;

            public LocalityRegion Region;

            public ILocalityStructure Structure;
        }

        private struct ConsumerKey
        {
            public ILocalityStructure Structure;
            public LocalityRegion Region;

            public override bool Equals(object obj)
            {
                return obj is ConsumerKey key &&
                       EqualityComparer<ILocalityStructure>.Default.Equals(Structure, key.Structure) &&
                       EqualityComparer<LocalityRegion>.Default.Equals(Region, key.Region);
            }

            public override int GetHashCode()
            {
                var hashCode = 1065899251;
                hashCode = hashCode * -1521134295 + EqualityComparer<ILocalityStructure>.Default.GetHashCode(Structure);
                hashCode = hashCode * -1521134295 + EqualityComparer<LocalityRegion>.Default.GetHashCode(Region);
                return hashCode;
            }

            public static bool operator ==(ConsumerKey left, ConsumerKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ConsumerKey left, ConsumerKey right)
            {
                return !(left == right);
            }
        }

        private class ConsumerEfficient
        {
            public LocalityResource Resource;
            public float ResourceAllocation;
            public float WorkerPower;
            public float RegionMaintance;
        }


        private void UpdateRegions()
        {
            // В этом методе рассчитываем, как отработали районы города.
            // Для этого считаем потребление и расход всех ресурсов от строений района.
            // В итоге получаем баланс ресурсов. В зависимости от баланса в городе и его районах происходят события.

            // Расчёт потребления всех ресурсов, кроме жилых мест, всеми строениями при нормальном снабжении.
            var baseConsumption = new Dictionary<LocalityResource, List<Comsumption>>();
            foreach (var region in Regions)
            {
                AddComsumption(baseConsumption, region, LocalityResource.Money, region.MaintenanceCost);
                foreach (var structure in region.Structures)
                {
                    foreach (var requiredResource in structure.RequiredResources)
                    {
                        AddComsumption(baseConsumption, structure, requiredResource.Key, requiredResource.Value);
                    }

                    AddComsumption(baseConsumption, structure, LocalityResource.Money, structure.MaintenanceCost);
                }
            }

            // Распределение ресурсов между районами и строениями.
            // Для этого сначала получаем все доступные ресурсы на эту итерацию.
            // Доступные ресурсы - это ресурсы с прошлой итерации + (ресурсы со складов * К).
            // Где К - коэффициент, указывающий долю ресурсов от требуемого потребления, которая будет поставляться со складов.
            var availableResources = new Dictionary<LocalityResource, float>();
            foreach (var lastIterationResource in Stats.ResourcesLastIteration)
            {
                availableResources[lastIterationResource.Key] = lastIterationResource.Value;
            }

            // Извлекаем ресурсы из хранилища на основе требований к потреблению.
            foreach (var consumption in baseConsumption)
            {
                if (!availableResources.ContainsKey(consumption.Key))
                {
                    availableResources[consumption.Key] = 0;
                }

                Stats.ResourcesStorage.TryGetValue(consumption.Key, out var availableStorageResource);

                var sumConsumption = consumption.Value.Sum(x => x.Amount);
                var storageResource = Math.Min(sumConsumption * STORAGE_RESOURCE_RESERVE, availableStorageResource);

                availableResources[consumption.Key] += storageResource;
            }

            var resourceAllocation = new Dictionary<LocalityResource, float>();
            foreach (var consumption in baseConsumption)
            {
                // Суммарное потребление нужно, чтобы получить количество долей, 
                // на которые нужно разделить существующие ресурсты.
                var sumConsumptionUnits = consumption.Value.Sum(x => x.Amount);

                var availableResource = availableResources[consumption.Key];
                // Предоставляем ресурсов не больше, чем требуется.
                // Иначе будет сверхпроизводительность. И остатки не будут складироваться.
                var normalizedAvailableResource = Math.Min(availableResource, sumConsumptionUnits);

                // На каждую единицу потреблностей будет предоставлена resourcePerUnit ресурса.
                // Эта величина так же показывает процент обеспечения ресурсами.
                var resourcePerUnit = normalizedAvailableResource / sumConsumptionUnits;
                resourceAllocation[consumption.Key] = resourcePerUnit;
            }

            // Рассчитываем эффективность работы районов и структур с учётом снабжения и населения.
            // Рассчитываем текущую выработку ресурсов городом на следующую итерацию.
            // Выработку сразу помещаем в словарь с выработкой на следующую итерацию.
            foreach (var region in Regions)
            {
                var regionMaintance = resourceAllocation[LocalityResource.Money];

                foreach (var structure in region.Structures)
                {
                    // Снабжение структуры ресурсами.
                    var structureResourceAllocation = 1f;

                    foreach (var requiredResource in structure.RequiredResources)
                    {
                        structureResourceAllocation *= resourceAllocation[requiredResource.Key];
                    }

                    // Обеспечение структуры работниками.
                    // Здесь получаем суммарную эффективность всех бригад на данной структуре
                    // с учётом потребностей.
                    // Суммарная производительность всех бригад делится на требуемое количество бригад.

                    var structurePopulationUnits = CurrentPopulation.Where(x => x.Assigments.Contains(structure));

                    var structurePopulationUnitsBySpecialization = structurePopulationUnits
                        .GroupBy(x => x.Specialization)
                        .ToDictionary(x => x.Key, x => x.ToArray());

                    // Рассчитываем суммарную эффективность населения по каждой специализации.
                    var factPopulationPower = 1f;
                    foreach (var specKey in structurePopulationUnitsBySpecialization)
                    {
                        var requiredSpecialization = specKey.Key;
                        var requiredSpecialistCount = structure.RequiredPopulation[requiredSpecialization];

                        // Рассчитываем суммарную эффективность, которую выдают текущей структуре все
                        // назначенные бригады специалистов.
                        var factSpecialists = specKey.Value;
                        var specialistPower = 0f;
                        foreach (var specialistUnit in factSpecialists)
                        {
                            // Специалисты поровну распределяют свою эффективность между всеми назначенными строениями.
                            var powerPerStructure = specialistUnit.Power / specialistUnit.Assigments.Count();

                            specialistPower += powerPerStructure;
                        }

                        // Насколько эффективно фактически отрабатывают спечиалисты с учётом требуемых мест.
                        var factSpecialistEfficient = specialistPower / requiredSpecialistCount;

                        // Учитываем эффективность бригад текущей специальности в суммарной эффективности всех рабочих.
                        factPopulationPower *= factSpecialistEfficient;
                    }


                    // Итоговая производительность структуры.
                    // Производим продукцию с учётом общей эффективности структуры.
                    var totalStructureEfficient = regionMaintance * structureResourceAllocation * factPopulationPower;

                    // Заполняем ресурсы, добытые за эту итерацию.
                    foreach (var resource in Stats.ResourcesLastIteration)
                    {
                        Stats.ResourcesLastIteration[resource.Key] = 0;
                    }

                    foreach (var production in structure.ProductResources)
                    {
                        var factProduction = production.Value * totalStructureEfficient;

                        if (!Stats.ResourcesLastIteration.ContainsKey(production.Key))
                        {
                            Stats.ResourcesLastIteration[production.Key] = 0;
                        }

                        Stats.ResourcesLastIteration[production.Key] += factProduction;
                    }

                    // Извлекаем ресурсы на производство из доступных ресурсов.
                    foreach (var consumedResource in structure.RequiredResources)
                    {
                        availableResources[consumedResource.Key] -= consumedResource.Value * resourceAllocation[consumedResource.Key];
                    }
                }
            }


            // Всё, что осталось от текущих доступных ресурсов помещается на склады.
            foreach (var remainResource in availableResources)
            {
                if (!Stats.ResourcesStorage.ContainsKey(remainResource.Key))
                {
                    Stats.ResourcesStorage[remainResource.Key] = 0;
                }

                Stats.ResourcesStorage[remainResource.Key] += remainResource.Value;
            }

            //foreach (var region in Regions)
            //{

            //    // Проверяем, хватает ли денег для этого района.
            //    var money = Stats.GetResource(LocalityResource.Money);
            //    if (money >= region.MaintenanceCost)
            //    {
            //        Stats.RemoveResource(LocalityResource.Money, region.MaintenanceCost);
            //    }
            //    else
            //    {
            //        // Не хватает денег на содержание этого района.
            //        // Все его строения не работают.
            //        continue;
            //    }

            //    // Для жилых мест отдельная логика.
            //    // Их потребляет только население, а производят структуры.
            //    // Поэтому зануляем перед обработкой структур города. Далее структуры выставят текущее значение.
            //    Stats.ResourcesLastIteration[LocalityResource.LivingPlaces] = 0;

            //    var suppliedStructures = SupplyStructures(region.Structures);
            //    ProduceResources(suppliedStructures, Stats);

            //    // Реализация производства.
            //    // Избыточное производтство даёт деньги. Предполагается, что избыточное производство
            //    // народ пускает на улучшение благосостояния.
            //    RealizeManufacture();
            //}
        }

        private static void AddComsumption(
            Dictionary<LocalityResource, List<Comsumption>> baseConsumption,
            ILocalityStructure structure,
            LocalityResource resource,
            float resourceConsumption)
        {
            if (!baseConsumption.TryGetValue(resource, out var consumptionList))
            {
                consumptionList = new List<Comsumption>();
                baseConsumption[resource] = consumptionList;
            }

            consumptionList.Add(new Comsumption() { Amount = resourceConsumption, Structure = structure });
        }

        private static void AddComsumption(
            Dictionary<LocalityResource, List<Comsumption>> baseConsumption,
            LocalityRegion region,
            LocalityResource resource,
            float resourceConsumption)
        {
            if (!baseConsumption.TryGetValue(resource, out var consumptionList))
            {
                consumptionList = new List<Comsumption>();
                baseConsumption[resource] = consumptionList;
            }

            consumptionList.Add(new Comsumption() { Amount = resourceConsumption, Region = region });
        }

        private void RealizeManufacture()
        {
            var manufacture = Stats.GetResource(LocalityResource.Manufacture);
            if (manufacture > 0)
            {
                Stats.RemoveResource(LocalityResource.Manufacture, manufacture);
                Stats.AddResource(LocalityResource.Money, manufacture);
            }
        }

        private void UpdatePopulation()
        {
            // Изымаем столько товаров и еды, сколько населения в городе.
            var populationCount = CurrentPopulation.Count();

            Stats.RemoveResource(LocalityResource.Food, populationCount);
            Stats.RemoveResource(LocalityResource.Goods, populationCount);

            // Рассчитываем рост населения
            UpdatePopulationGrowth();
        }

        private void UpdatePopulationGrowth()
        {
            foreach (var unit in CurrentPopulation)
            {
                unit.UpdateGrowth();
            }
        }

        private static void ProduceResources(List<ILocalityStructure> structures, LocalityStats stats)
        {
            // Все структуры, которые получили обеспечение, производят ресурс
            foreach (var structure in structures)
            {
                foreach (var productResource in structure.ProductResources)
                {
                    if (!stats.ResourcesLastIteration.ContainsKey(productResource.Key))
                    {
                        stats.ResourcesLastIteration[productResource.Key] = 0;
                    }

                    stats.ResourcesLastIteration[productResource.Key] += productResource.Value;
                }
            }
        }

        private List<ILocalityStructure> SupplyStructures(List<ILocalityStructure> structures)
        {
            // Структуры, которые получили обеспечение.
            var suppliedStructures = new List<ILocalityStructure>();

            // Изымаем все ресурсы текущй структурой.
            // Струкруты, которые получили обеспечение, затем производят ресурсы.
            foreach (var structure in structures)
            {
                // Проверяем, хватает ли денег для содержанния этой структуры.
                var money = Stats.GetResource(LocalityResource.Money);
                if (money >= structure.MaintenanceCost)
                {
                    Stats.RemoveResource(LocalityResource.Money, structure.MaintenanceCost);
                }
                else
                {
                    // Не хватает денег на содержание этой структуры.
                    // Она ничего не производит в эту итерацию.
                    continue;
                }

                // Проверка наличия необходимых ресурсов.
                foreach (var requiredResource in structure.RequiredResources)
                {
                    var requiredResourceType = requiredResource.Key;
                    if (Stats.ResourcesLastIteration.ContainsKey(requiredResourceType))
                    {
                        if (Stats.ResourcesLastIteration[requiredResourceType] >= requiredResource.Value)
                        {
                            suppliedStructures.Add(structure);
                            Stats.ResourcesLastIteration[requiredResourceType] -= requiredResource.Value;
                        }
                    }

                }
            }

            return suppliedStructures;
        }

        public void AddResource(Dictionary<LocalityResource, float> dict, LocalityResource resource, int amount)
        {
            dict[resource] += amount;
        }

        public float GetResource(Dictionary<LocalityResource, float> dict, LocalityResource resource)
        {
            return dict[resource];
        }

        public void RemoveResource(Dictionary<LocalityResource, float> dict, LocalityResource resource, float amount)
        {
            if (!dict.ContainsKey(resource))
            {
                dict[resource] = 0;
            }

            dict[resource] -= amount;
        }
    }
}
