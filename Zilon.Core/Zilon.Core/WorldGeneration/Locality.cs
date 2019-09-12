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
            // Считаем общее потребление всех ресурсов всем источниками:
            // район, структуры района и население.
            // Исходя из общего потребления будут расчитываться ресурсы, изъятые со складов.
            // А так же эффективность работы сначала населения, а затем - структур города.
            var totalConsumption = new Dictionary<LocalityResource, List<Comsumption>>();
            foreach (var region in Regions)
            {
                AddComsumption(totalConsumption, region, LocalityResource.Money, region.MaintenanceCost);
                foreach (var structure in region.Structures)
                {
                    foreach (var requiredResource in structure.RequiredResources)
                    {
                        AddComsumption(totalConsumption, structure, requiredResource.Key, requiredResource.Value);
                    }

                    AddComsumption(totalConsumption, structure, LocalityResource.Money, structure.MaintenanceCost);
                }
            }

            // Нужные населению ресурсы
            AddPopulationComsumption(totalConsumption, LocalityResource.Food, CurrentPopulation.Count());
            AddPopulationComsumption(totalConsumption, LocalityResource.Goods, CurrentPopulation.Count());

            UpdatePopulation(totalConsumption);
            UpdateStructures(totalConsumption);
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

            /// <summary>
            /// Указывает, что потребителем ресурса является население.
            /// </summary>
            public bool IsPopulation;
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


        private void UpdateStructures(Dictionary<LocalityResource, List<Comsumption>> totalConsumption)
        {
            // В этом методе рассчитываем, как отработали все структуры города.
            
            // Распределение ресурсов между районами и строениями.
            // Для этого сначала получаем все доступные ресурсы на эту итерацию.
            // Доступные ресурсы - это ресурсы с прошлой итерации + (ресурсы со складов * К).
            // Где К - коэффициент, указывающий долю ресурсов от требуемого потребления, которая будет поставляться со складов.
            var availableResources = new Dictionary<LocalityResource, float>();
            foreach (var lastIterationResource in Stats.ResourcesLastIteration)
            {
                if (lastIterationResource.Key == LocalityResource.LivingPlaces)
                {
                    // Жилые места не участвуют в производстве.
                    continue;
                }

                availableResources[lastIterationResource.Key] = lastIterationResource.Value;
            }

            // Извлекаем ресурсы из хранилища на основе требований к потреблению.
            foreach (var consumption in totalConsumption)
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

            // Расчитываем фактическое распределение ресурса на единицу требований.
            var resourceAllocationPerUnit = new Dictionary<LocalityResource, float>();
            foreach (var consumption in totalConsumption)
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
                resourceAllocationPerUnit[consumption.Key] = resourcePerUnit;
            }

            // -------- Потребление ресурсов населением
            var populationNeedResources = new[] { LocalityResource.Food, LocalityResource.Goods };
            var populationCount = CurrentPopulation.Count();
            foreach (var needResource in populationNeedResources)
            {
                availableResources[needResource] -= populationCount * resourceAllocationPerUnit[needResource];
            }

            // Рассчитываем эффективность работы районов и структур с учётом снабжения и населения.
            // Рассчитываем текущую выработку ресурсов городом на следующую итерацию.
            // Выработку сразу помещаем в словарь с выработкой на следующую итерацию.
            foreach (var region in Regions)
            {
                var regionMaintance = resourceAllocationPerUnit[LocalityResource.Money];

                foreach (var structure in region.Structures)
                {
                    // Снабжение структуры ресурсами.
                    var structureResourceAllocation = 1f;

                    foreach (var requiredResource in structure.RequiredResources)
                    {
                        structureResourceAllocation *= resourceAllocationPerUnit[requiredResource.Key];
                    }

                    // Обеспечение структуры работниками.
                    // Здесь получаем суммарную эффективность всех бригад на данной структуре
                    // с учётом потребностей.
                    // Суммарная производительность всех бригад делится на требуемое количество бригад.

                    var structurePopulationUnits = CurrentPopulation.Where(x => x.Assigments.Contains(structure));

                    var structurePopulationUnitsBySpecialization = structurePopulationUnits
                        .GroupBy(x => x.Specialization)
                        .ToDictionary(x => x.Key, x => x.ToArray());

                    // Рассчитываем итоговую эффективность специалистов из населения на текущей структуре.
                    // Итоговая эффективность равна минимальной эффетивности бригады.
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
                    foreach (var resource in Stats.ResourcesLastIteration.Keys.ToArray())
                    {
                        if (resource == LocalityResource.LivingPlaces)
                        {
                            // Жилые места не используются для производства.
                            continue;
                        }

                        Stats.ResourcesLastIteration[resource] = 0;
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
                        availableResources[consumedResource.Key] -= consumedResource.Value * resourceAllocationPerUnit[consumedResource.Key];
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

        private static void AddPopulationComsumption(
            Dictionary<LocalityResource, List<Comsumption>> baseConsumption,
            LocalityResource resource,
            float resourceConsumption)
        {
            if (!baseConsumption.TryGetValue(resource, out var consumptionList))
            {
                consumptionList = new List<Comsumption>();
                baseConsumption[resource] = consumptionList;
            }

            consumptionList.Add(new Comsumption() { Amount = resourceConsumption, IsPopulation = true });
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

        private void UpdatePopulation(Dictionary<LocalityResource, List<Comsumption>> totalConsumption)
        {
            // Изымаем столько товаров и еды, сколько населения в городе.
            // Логика аналогична производству товаров.
            // Все доступные для населения ресурсы складываются из произведённых ресурсов в прошлую итерацию
            // и запасов со складов.
            // Население потребляет еду и товары народного потребления (Goods).

            // Из доступных ресурсов - всё, что было добыто на прошлом ходу + половина требуемых ресурсов со склада.
            // Некоторые ресурсы могут быть нужны и населению и производству.
            // В этом случае все доступные ресурсы делим пропорционально.
            
            // ----------
            // Потребление ресурсов населением происходит рядом с потреблением городскими структурами.

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

        public void AddResource(Dictionary<LocalityResource, float> dict, LocalityResource resource, int amount)
        {
            dict[resource] += amount;
        }
    }
}
